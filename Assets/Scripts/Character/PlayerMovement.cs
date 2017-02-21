// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour, IMovable
{

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Transform foot;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float iceForceMultiplier;
    [SerializeField]
    private float manualDrag;
    [SerializeField]
    private bool axisInput;
    [SerializeField]
    private bool isSidescrolling;
    [SerializeField]
    private bool canMove = true;
    [SerializeField]
    private Vector3 lastMovement;
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float meleePounceRange;
    [SerializeField]
    private MovementMode movementMode;
    [SerializeField]
    private Vector3 velocity;
    #endregion

    #region Private Fields
    GameObject currentEnemy;
    Vector3 currentEnemyVector;
    LockOnScript lockOnScript;
    Animator animator;    
    private PlayerModAnimationManager modAnimationManager;
    private Rigidbody rigid;
    private RaycastHit hitInfo;
    public bool isGrounded;
    private bool isFalling = false;
    private float sphereRadius = 0.1f;
    private Vector3 movement;
    private List<Vector3> externalForces;
    private List<Vector3> externalForcesToAdd;
    private PlayerStatsManager statsManager;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();        
    }

    // Use this for initialization
    void Start()
    {
        statsManager = GetComponent<PlayerStatsManager>();
        lockOnScript = GetComponent<LockOnScript>();
        currentEnemy = null;        
        externalForces = new List<Vector3>();
        externalForcesToAdd = new List<Vector3>();
        for (int i = 0; i < 100; i++)
        {
            externalForces.Add(Vector3.zero);
        }
        movementMode = MovementMode.PRECISE;
        animator = GetComponent<Animator>();
        modAnimationManager = GetComponent<PlayerModAnimationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lockOnScript != null)
        {
            currentEnemy = lockOnScript.GetCurrentEnemy();
            if (currentEnemy != null)
            {
                currentEnemyVector = currentEnemy.transform.position;
            }
        }

		float h = Input.GetAxis(Strings.MOVEX);
		float v = Input.GetAxis(Strings.MOVEY);
		axisInput = CheckForAxisInput(h, v);

		float hModified = h;
		float vModified = v;

		if (isSidescrolling)
		{
			vModified = 0f;
		}

		movement = new Vector3(hModified, 0.0f, vModified);

		if (!canMove)
		{
			movement = Vector3.zero;
		}

		velocity = rigid.velocity;


		movement = Camera.main.transform.TransformDirection(movement);

		movement.y = 0f;

		movement = movement.normalized;

		if (movement != Vector3.zero)
		{
			lastMovement = movement;
		}

		isGrounded = IsGrounded();
		maxSpeed = statsManager.GetStat(StatType.MoveSpeed);
    }

    private void FixedUpdate()
    {

		switch (movementMode)
		{
			case MovementMode.PRECISE:
				// Do I even need fixedDeltaTime here if I'm changing the velocity of the rigidbody directly?
				//  rigid.velocity = movement * stats.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime + GetExternalForceSum();  
				rigid.velocity = movement * statsManager.GetStat(StatType.MoveSpeed) + GetExternalForceSum();
				if (!modAnimationManager.GetIsPlaying())
				{
					if (rigid.velocity != Vector3.zero)
					{
						if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Running)
						{
							animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Running);
						}
					}
					else
					{
						if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
						{
							animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
						}
					}
				}
				break;
			case MovementMode.ICE:
				if (!modAnimationManager.GetIsPlaying())
				{
					if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
					{
						animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
					}
				}
				rigid.AddForce(movement * acceleration * Time.fixedDeltaTime);

				foreach (Vector3 vector in externalForcesToAdd)
				{
					rigid.AddForce(vector * Time.fixedDeltaTime);
				}


				Vector3 flatMovement = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
				currentSpeed = flatMovement.magnitude;

				if (currentSpeed > statsManager.GetStat(StatType.MoveSpeed))
				{
					Vector3 currentFlatVelocity = flatMovement;
					currentFlatVelocity = -currentFlatVelocity;
					currentFlatVelocity = currentFlatVelocity.normalized;
					currentFlatVelocity *= (currentSpeed - statsManager.GetStat(StatType.MoveSpeed));
					currentFlatVelocity *= manualDrag;
					rigid.AddForce(currentFlatVelocity);
				}
				externalForcesToAdd.Clear();

				break;
			default:
				rigid.velocity = movement * statsManager.GetStat(StatType.MoveSpeed) + GetExternalForceSum();
				break;
		}

		if (!axisInput)
		{
			if (currentEnemy != null)
			{
				transform.LookAt(currentEnemyVector, Vector3.up);
				Quaternion rotation = transform.rotation;
				rotation.x = 0;
				rotation.z = 0;
				transform.rotation = rotation;
			}
			else if (lastMovement != Vector3.zero)
			{
				if (lastMovement != Vector3.zero)
				{
					transform.forward = lastMovement;
				}
			}
		}
		else if (movement != Vector3.zero)
		{
			if (currentEnemy != null)
			{
				transform.LookAt(currentEnemyVector, Vector3.up);
				Quaternion rotation = transform.rotation;
				rotation.x = 0;
				rotation.z = 0;
				transform.rotation = rotation;
			}
			else
			{
				transform.forward = movement;
			}
		}
		velocity = rigid.velocity;
		currentSpeed = rigid.velocity.magnitude;
    }
    #endregion

    #region Public Methods
    public void AddExternalForce(Vector3 forceVector, float decay = 0.1f)
    {
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
    public void SetSidescrolling(bool mode)
    {
        isSidescrolling = mode;
    }
   
    public void SetMovementMode(MovementMode mode)
    {
        movementMode = mode;
        rigid.useGravity = mode == MovementMode.ICE ? true : false;
    }

    public void PlayAnimation(Mod mod)
    {
        if (!modAnimationManager.GetIsPlaying())
        {
            if (movementMode == MovementMode.PRECISE && rigid.velocity != Vector3.zero)
            {
                modAnimationManager.PlayModAnimation(mod, true);
            }
            else
            {
                modAnimationManager.PlayModAnimation(mod, false);
            }
        }
    }

    public void DoMeleePounce()
    {

    }
    #endregion

    #region Private Methods
    private Vector3 GetExternalForceSum()
    {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }
    private IEnumerator AddPsuedoForce(Vector3 forceVector, float decay)
    {
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);

        externalForces[currentIndex] = forceVector;
        while (externalForces[currentIndex].magnitude > .2f)
        {
            externalForces[currentIndex] = Vector3.Lerp(externalForces[currentIndex], Vector3.zero, decay);
            yield return null;
        }
        externalForces[currentIndex] = Vector3.zero;
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


    private bool IsGrounded()
    {

        Collider[] cols = Physics.OverlapSphere(foot.transform.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer == (int)Layers.Ground)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(foot.transform.position, sphereRadius);
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
    #endregion

    #region Private Structures
    private Dictionary<ModSpot, bool> modSpotConstantForceIndices = new Dictionary<ModSpot, bool>()
    {
        {ModSpot.Head, false},
        {ModSpot.Legs, false},
        {ModSpot.ArmL, false},
        {ModSpot.ArmR, false}
    };
    #endregion

}
