using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class VelocityBody : MonoBehaviour, IMovable{

    public Transform foot;
    [SerializeField] private Rigidbody rigbod;

    public Vector3 velocity {
        get {
            return rigbod.velocity;
        }
        set {
            rigbod.velocity = value;
            if (movementMode==MovementMode.PRECISE) {
                rigbod.velocity += GetExternalForceSum();
            }
        }
    }
    public bool isKinematic {
        get { return rigbod.isKinematic; }
        set { rigbod.isKinematic = value; }
    }
    public bool useGravity {
        get { return rigbod.useGravity; }
        set { rigbod.useGravity = value;}
    }
    protected bool isGrounded;

    private bool isFalling;
    private const float iceForceMultiplier = 50f;
    private const float footSphereRadius= 0.1f;
    private List<Vector3> externalForces;
    private MovementMode movementMode;

    void Start() {        
        InitializeExternalForces();
    }

    protected virtual void Update() {
        isGrounded = CheckIsGrounded();
    }

    public void InitializeExternalForces() {
        externalForces = new List<Vector3>();
        for (int i = 0; i < 100; i++) {
            externalForces.Add(Vector3.zero);
        }
    }

    public void LookAt(Transform target) {
        Vector3 lookAtPosition = new Vector3(target.position.x, 0, target.position.z);
        transform.LookAt(lookAtPosition);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    public void AddDecayingForce(Vector3 force, float decay = 0.1f) {
        if (gameObject.activeInHierarchy) {
            if (movementMode == MovementMode.PRECISE) {
                Timing.RunCoroutine(IEAddDecayingForce(force, decay), GetInstanceID().ToString());
            }
            else{
                rigbod.AddForce(force * iceForceMultiplier);
            }
        }    
    }

    public bool IsGrounded() {
        return isGrounded;
    }
    public void SetMovementMode(MovementMode movementMode) {
        this.movementMode = movementMode;
        useGravity = movementMode == MovementMode.ICE;
        if (movementMode == MovementMode.PRECISE) {
            AddDecayingForce(rigbod.velocity*.95f, 0.075f);            
        }
    }
    
    public MovementMode GetMovementMode() {
        return movementMode;
    }
    public Vector3 GetForward() {
        return transform.forward;
    }

    public void ResetForRebirth() {
        Timing.KillCoroutines(GetInstanceID().ToString());
        externalForces.ForEach(force => force = Vector3.zero);
        isFalling = false;
        isGrounded = false;
        isKinematic = false;
    }

    private IEnumerator<float> IEAddDecayingForce(Vector3 forceVector, float decay) {
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);

        externalForces[currentIndex] = forceVector;

        while (externalForces[currentIndex].magnitude > .2f) {
            externalForces[currentIndex] = Vector3.Lerp(externalForces[currentIndex], Vector3.zero, decay);
            yield return Timing.WaitForOneFrame;
        }
        externalForces[currentIndex] = Vector3.zero;
    }

    private bool CheckIsGrounded() {
        Collider[] cols = Physics.OverlapSphere(foot.transform.position, footSphereRadius);
        for (int i = 0; i < cols.Length; i++) {
            if (cols[i].gameObject.layer == (int)Layers.Ground) {
                return true;
            }
        }
        if (!isFalling) {
            Timing.RunCoroutine(ApplyGravity(), GetInstanceID().ToString());
        }
        return false;
    }

    private IEnumerator<float> ApplyGravity() {
        if (!useGravity) {
            isFalling = true;
            int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);
            float timeElapsed = 0f;
            float gravity = 9.81f;
            while (!isGrounded && isFalling && !useGravity) {
                externalForces[currentIndex] = Vector3.down * (gravity * timeElapsed);
                timeElapsed += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }
            isFalling = false;
            externalForces[currentIndex] = Vector3.zero;
        }
    }

    private Vector3 GetExternalForceSum() {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }            
}
