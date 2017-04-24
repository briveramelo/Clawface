using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class VelocityBody : MonoBehaviour, IMovable{

    public Transform foot;
    [SerializeField] private Rigidbody rigbod;
    [SerializeField] private Rigidbody ragdollRigBod;
    [SerializeField]
    private float iceForceMultiplier;
    [SerializeField]private MovementMode movementMode;

    public bool isGrounded;

    private bool isFalling;
    
    private const float footSphereRadius= 0.2f;
    private List<Vector3> externalForces= new List<Vector3>();
    private Collider[] groundColliders = new Collider[10];
    private int groundMask {
        get {
            if (gameObject.CompareTag(Strings.Tags.PLAYER)) {
                return LayerMasker.GetLayerMask(new List<Layers>() { Layers.Ground, Layers.Enemy });
            }
            return LayerMasker.GetLayerMask(Layers.Ground);
        }
    }

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

    void Start() {        
        InitializeExternalForces();
    }

    protected virtual void Update() {
        if (!useGravity) {
            isGrounded = CheckIsGrounded();
            if (!isGrounded && !isFalling) {
                Timing.RunCoroutine(ApplyGravity(), Segment.FixedUpdate, GetInstanceID().ToString());
            }
        }
        //if (movementMode==MovementMode.ICE) {            
        //    float x = Mathf.Clamp(rigbod.velocity.x, -20f, 20f);
        //    float y = Mathf.Clamp(rigbod.velocity.y, -20f, 20f);
        //    float z = Mathf.Clamp(rigbod.velocity.z, -20f, 20f);
        //    rigbod.velocity = new Vector3(x, y, z);
        //}
    }

    public void StopVerticalMovement() {
        externalForces.ForEach(force => {
            force.y = 0;
        });
        Vector3 vel = rigbod.velocity;
        vel.y = 0;
        rigbod.velocity = vel;
    }

    public void InitializeExternalForces() {
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
            switch (movementMode) {
                case MovementMode.PRECISE:
                    Timing.RunCoroutine(IEAddDecayingForce(force, decay), Segment.FixedUpdate, GetInstanceID().ToString());
                    break;
                case MovementMode.ICE:
                    rigbod.AddForce(force * iceForceMultiplier);  
                    break;
                case MovementMode.RAGDOLL:
                    ragdollRigBod.AddForce (force * iceForceMultiplier);
                    break;
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
        } else if (movementMode == MovementMode.RAGDOLL) {
            AddDecayingForce(ragdollRigBod.velocity*.95f, 0.075f);            
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
        velocity = Vector3.zero;
        rigbod.velocity = Vector3.zero;
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
        groundColliders = Physics.OverlapSphere(foot.transform.position, footSphereRadius, groundMask);
        for (int i = 0; i < groundColliders.Length; i++) {
            if (groundColliders[i].gameObject.layer == (int)Layers.Ground || groundColliders[i].gameObject.layer == (int)Layers.Enemy) {
                return true;
            }
        }        
        return false;
    }

    private IEnumerator<float> ApplyGravity() {        
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

    private Vector3 GetExternalForceSum() {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
