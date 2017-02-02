// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    [SerializeField] private Transform foot;
    [SerializeField] private float speed;
    [SerializeField] private bool axisInput;
    [SerializeField] private bool rightAxisInput;
    [SerializeField] private bool isSidescrolling;
    [SerializeField] private bool canMove = true;
    [SerializeField] private Vector3 lastMovement;
    [SerializeField] private Vector3 rightJoystickMovement;
    


    private Dictionary<ModSpot, bool> modSpotConstantForceIndices = new Dictionary<ModSpot, bool>() {
        {ModSpot.Head, false},
        {ModSpot.Legs, false},
        {ModSpot.Arm_L, false},
        {ModSpot.Arm_R, false}
    };


    #region Privates

    [SerializeField]
    public Vector3 velocity;
    private Rigidbody rigid;

    private RaycastHit hitInfo;
    private bool isGrounded;
    private bool isFalling = false;
    private float sphereRadius = 0.1f;

    private Vector3 movement;

    private List<Vector3> externalForces;
    #endregion

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start() {
        externalForces = new List<Vector3>();
        for (int i = 0; i < 100; i++) {
            externalForces.Add(Vector3.zero);
        }

    }
    

    
    // Update is called once per frame
    void Update() {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float rightH = Input.GetAxis("Camera X");
        float rightV = Input.GetAxis("Camera Y");

        axisInput = CheckForAxisInput(h, v);
        rightAxisInput = CheckForAxisInput(rightH, rightV);

        float hModified = h;
        float vModified = v;

        float rightHModified = rightH;
        float rightVModified = rightV;

        if (isSidescrolling)
        {
            vModified = 0f;
            rightVModified = 0f;
        }

        movement = new Vector3(hModified, 0.0f, vModified);
        rightJoystickMovement = new Vector3(rightHModified, 0.0f, rightVModified);

        if (!canMove) {
            movement = Vector3.zero;
        }

        velocity = rigid.velocity;


        movement = Camera.main.transform.TransformDirection(movement);
        rightJoystickMovement = Camera.main.transform.TransformDirection(rightJoystickMovement);

        movement.y = 0f;
        rightJoystickMovement.y = 0f;

        movement = movement.normalized;
        rightJoystickMovement = rightJoystickMovement.normalized;

        if (movement != Vector3.zero)
        {
            lastMovement = movement;
        }

        isGrounded = IsGrounded();
    }

    private void FixedUpdate()
    {
        rigid.velocity = movement * speed * Time.fixedDeltaTime + GetExternalForceSum();
        if (!axisInput)
        {
            if (rightAxisInput)
            {
                transform.forward = rightJoystickMovement;
            }
            else
            {
                transform.forward = lastMovement;
            }
        }
        else
        {   if (rightAxisInput)
            {
                transform.forward = rightJoystickMovement;
            }
            else
            {
                transform.forward = movement;
            }
        }
    }

    private Vector3 GetExternalForceSum() {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }

    public void AddExternalForce(Vector3 forceVector, float decay = 0.1f) {
        if (canMove) {
            StartCoroutine(AddPsuedoForce(forceVector, decay));
        }
    }

    public void AddJetForce(Vector3 constantForce, ModSpot modSpot) {
        modSpotConstantForceIndices[modSpot] = true;
        if (modSpot == ModSpot.Legs) {
            isFalling = true;
        }
        StartCoroutine(ApplyJetForce(constantForce, modSpot));
    }

    public void StopConstantForce(ModSpot modSpot) {
        modSpotConstantForceIndices[modSpot] = false;
        if (modSpot == ModSpot.Legs) {
            isFalling = false;
        }
    }

    
    private IEnumerator ApplyJetForce(Vector3 constantForce, ModSpot modSpot) {
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);        
        while (modSpotConstantForceIndices[modSpot] && externalForces[currentIndex].magnitude < constantForce.magnitude - 0.1f) {
            externalForces[currentIndex] = Vector3.Lerp(externalForces[currentIndex], constantForce, 0.1f);
            yield return null;
        }
        StartCoroutine(FadeTargetForce(currentIndex));

        float period = 5f;
        float timePassed = 0f;
        currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);
        while (modSpotConstantForceIndices[modSpot]) {
            externalForces[currentIndex] = 0.3f*constantForce * -Mathf.Sin((Mathf.PI*2/period) * timePassed);
            timePassed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeTargetForce(currentIndex));
    }

    private IEnumerator FadeTargetForce(int targetIndex) {
        while (externalForces[targetIndex].magnitude > 0.1f) {
            externalForces[targetIndex] = Vector3.Lerp(externalForces[targetIndex], Vector3.zero, 0.1f);
            yield return null;
        }
    }

    
    private IEnumerator AddPsuedoForce(Vector3 forceVector, float decay) {
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);

        externalForces[currentIndex] = forceVector;
        while (externalForces[currentIndex].magnitude > .2f) {
            externalForces[currentIndex] = Vector3.Lerp(externalForces[currentIndex], Vector3.zero, decay);
            yield return null;
        }
        externalForces[currentIndex] = Vector3.zero;
    }

    private IEnumerator ApplyGravity() {
        isFalling = true;
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);
        float timeElapsed = 0f;
        float gravity = 9.81f;
        while (!isGrounded && isFalling) {
            externalForces[currentIndex] = Vector3.down * (gravity * timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isFalling = false;
        externalForces[currentIndex] = Vector3.zero;
    }

    
    public bool IsGrounded()
    {

        Collider[] cols = Physics.OverlapSphere(foot.transform.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++) {
            if (cols[i].gameObject.layer != (int)Layers.ModMan) {
                return true;
            }
        }
        if (!isFalling) {
            StartCoroutine(ApplyGravity());
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(foot.transform.position, sphereRadius);
    }

    public void SetSidescrolling(bool mode)
    {
        isSidescrolling = mode;
    }

    private bool CheckForAxisInput(float h, float v)
    {
        if (Mathf.Approximately(h, 0f))
        {
            if (Mathf.Approximately(v, 0f))
            {
                return false;
            }

        }
        return true;
    }
}
