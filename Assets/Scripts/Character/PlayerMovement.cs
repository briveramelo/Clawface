// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour, IDamageable, IMovable
{

    

    [SerializeField] private Transform foot;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField]
    private float iceForceMultiplier;
    [SerializeField]
    private float manualDrag;
    [SerializeField] private bool axisInput;
    [SerializeField] private bool rightAxisInput;
    [SerializeField] private bool isSidescrolling;
    [SerializeField] private bool canMove = true;
    [SerializeField] private Vector3 lastMovement;
    [SerializeField] private Vector3 rightJoystickMovement;
    [SerializeField]
    private float currentSpeed;

    [SerializeField] private MovementMode movementMode;

    private Stats stats;


    private Dictionary<ModSpot, bool> modSpotConstantForceIndices = new Dictionary<ModSpot, bool>()
    {
        {ModSpot.Head, false},
        {ModSpot.Legs, false},
        {ModSpot.ArmL, false},
        {ModSpot.ArmR, false}
    };


    #region Privates

    [SerializeField]
    private Vector3 velocity;
    private Rigidbody rigid;

    private RaycastHit hitInfo;
    public bool isGrounded;
    private bool isFalling = false;
    private float sphereRadius = 0.1f;

    private Vector3 movement;

    private List<Vector3> externalForces;
    private List<Vector3> externalForcesToAdd;
    #endregion

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        stats = GetComponent<Stats>();
    }

    // Use this for initialization
    void Start() {
        externalForces = new List<Vector3>();
        externalForcesToAdd = new List<Vector3>();
        for (int i = 0; i < 100; i++) {
            externalForces.Add(Vector3.zero);
        }
        movementMode = MovementMode.PRECISE;
    }
    

    
    // Update is called once per frame
    void Update() {

        float h = Input.GetAxis(Strings.MOVEX);
        float v = Input.GetAxis(Strings.MOVEY);

        float rightH = Input.GetAxis(Strings.AIMX);
        float rightV = Input.GetAxis(Strings.AIMY);

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
        maxSpeed = stats.GetStat(StatType.MoveSpeed);
    }

    private void FixedUpdate()
    {
        switch (movementMode)
        {
            case MovementMode.PRECISE:
                // Do I even need fixedDeltaTime here if I'm changing the velocity of the rigidbody directly?
                //  rigid.velocity = movement * stats.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime + GetExternalForceSum();  
                rigid.velocity = movement * stats.GetStat(StatType.MoveSpeed) + GetExternalForceSum();
                break;
            case MovementMode.ICE:

                rigid.AddForce(movement * acceleration * Time.fixedDeltaTime);

                foreach (Vector3 vector in externalForcesToAdd)
                {
                    rigid.AddForce(vector * Time.fixedDeltaTime);
                }

                
                Vector3 flatMovement = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
                currentSpeed = flatMovement.magnitude;

                if (currentSpeed > stats.GetStat(StatType.MoveSpeed))
                {
                    Vector3 currentFlatVelocity = flatMovement;
                    currentFlatVelocity = -currentFlatVelocity;
                    currentFlatVelocity = currentFlatVelocity.normalized;
                    currentFlatVelocity *= (currentSpeed - stats.GetStat(StatType.MoveSpeed));
                    currentFlatVelocity *= manualDrag;
                    rigid.AddForce(currentFlatVelocity);
                }
                externalForcesToAdd.Clear();

                break;
            default:
                rigid.velocity = movement * stats.GetStat(StatType.MoveSpeed) + GetExternalForceSum();
                break;
        }

        if (!axisInput)
        {
            if (rightAxisInput && rightJoystickMovement != Vector3.zero)
            {
                transform.forward = rightJoystickMovement;
            }
            else if(lastMovement != Vector3.zero)
            {
                if (lastMovement != Vector3.zero)
                {
                    transform.forward = lastMovement;
                }
            }
        }
        else
        {   if (rightAxisInput && rightJoystickMovement != Vector3.zero)
            {
                transform.forward = rightJoystickMovement;
            }
            else
            {
                if (movement != Vector3.zero)
                {
                    transform.forward = movement;
                }
            }
        }

        velocity = rigid.velocity;
        currentSpeed = rigid.velocity.magnitude;
    }

    private Vector3 GetExternalForceSum() {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }

    public void AddExternalForce(Vector3 forceVector, float decay=0.1f) {
        switch (movementMode)
        {
            case MovementMode.PRECISE:
            default:

                if (canMove)
                {
                    StartCoroutine(AddPsuedoForce(forceVector, decay));
                }
                break;
            case MovementMode.ICE:
                if (canMove)
                {
                    externalForcesToAdd.Add(forceVector * iceForceMultiplier);
                }
                break;
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
            if (cols[i].gameObject.layer == (int)Layers.Ground) {
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

    public void TakeDamage(float damage)
    {
        stats.TakeDamage(damage);
        if(stats.GetStat(StatType.Health) <= 0)
        {
            Destroy(gameObject);
        }

    public void SetMovementMode(MovementMode mode)
    {
        movementMode = mode;
        rigid.useGravity = mode == MovementMode.ICE ? true : false;
    }
}
