using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMovementTest : MonoBehaviour {

    [SerializeField]
    private Transform foot;
    [SerializeField]
    private float speed;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private bool axisInput;
    [SerializeField]
    private bool rightAxisInput;
    [SerializeField]
    private bool isSidescrolling;
    [SerializeField]
    private bool canMove = true;
    [SerializeField]
    private Vector3 lastMovement;
    [SerializeField]
    private Vector3 rightJoystickMovement;

    [SerializeField]
    private Vector3 velocity;

    private Rigidbody rigid;

    private RaycastHit hitInfo;
    private bool isGrounded;
    private bool isFalling = false;
    private float sphereRadius = 0.1f;

    private Vector3 movement;
    private List<Vector3> externalForces;

    [SerializeField]
    private float currentSpeed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
        externalForces = new List<Vector3>();
        for (int i = 0; i < 100; i++)
        {
            externalForces.Add(Vector3.zero);
        }
    }
	
	// Update is called once per frame
	void Update () {

        // FIX LATER
        //float h = Input.GetAxis(Strings.MOVEX);
        //float v = Input.GetAxis(Strings.MOVEY);

        //float rightH = Input.GetAxis(Strings.AIMX);
        //float rightV = Input.GetAxis(Strings.AIMY);

        //axisInput = CheckForAxisInput(h, v);
        //rightAxisInput = CheckForAxisInput(rightH, rightV);

        //float hModified = h;
        //float vModified = v;

        //float rightHModified = rightH;
        //float rightVModified = rightV;



        //movement = new Vector3(hModified, 0.0f, vModified);
        //rightJoystickMovement = new Vector3(rightHModified, 0.0f, rightVModified);

        if (!canMove)
        {
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
        rigid.AddForce(movement * speed * Time.fixedDeltaTime);

        float currentSpeed = Vector3.Magnitude(rigid.velocity);  // test current object speed

        if (currentSpeed > maxSpeed)
        {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }

        this.currentSpeed = rigid.velocity.magnitude;

        if (!axisInput)
        {
            if (rightAxisInput && rightJoystickMovement != Vector3.zero)
            {
                transform.forward = rightJoystickMovement;
            }
            else if (lastMovement != Vector3.zero)
            {
                if (lastMovement != Vector3.zero)
                {
                    transform.forward = lastMovement;
                }
            }
        }
        else
        {
            if (rightAxisInput && rightJoystickMovement != Vector3.zero)
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

    public bool IsGrounded()
    {

        Collider[] cols = Physics.OverlapSphere(foot.transform.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer != (int)Layers.ModMan)
            {
                return true;
            }
        }
        if (!isFalling)
        {
            StartCoroutine(ApplyGravity());
        }
        return false;
    }

    private IEnumerator ApplyGravity()
    {
        isFalling = true;
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);
        float timeElapsed = 0f;
        float gravity = 9.81f;
        while (!isGrounded && isFalling)
        {
            externalForces[currentIndex] = Vector3.down * (gravity * timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isFalling = false;
        externalForces[currentIndex] = Vector3.zero;
    }
}
