
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour {

    #region Public fields
    public StateVariables stateVariables;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private MoveState defaultState;
    [SerializeField]
    private PlayerStatsManager playerStatsManager;
    [SerializeField]
    private float holdAttackSlowDown;
    [SerializeField]
    private DashState dashState;
    [SerializeField]
    private float dashCoolDown;

    [SerializeField] private EatingState eatingState;
    [SerializeField] private float eatRadius;
    #endregion

    #region Private Fields
    private IPlayerState movementState;
    public List<IPlayerState> playerStates;
    private bool canDash = true;
    private bool stateChanged;
    private bool playerCanMove = true;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        stateChanged = false;
        stateVariables.stateFinished = true;
        stateVariables.playerTransform = transform;
        stateVariables.statsManager = playerStatsManager;
        stateVariables.defaultState = defaultState;
        defaultState.Init(ref stateVariables);
        dashState.Init(ref stateVariables);
        eatingState.Init(ref stateVariables);
        movementState = defaultState;
        playerStates = new List<IPlayerState>(){ defaultState};

        //for input blocking 
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, BlockInput);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, BlockInput);
    }
	
	// Update is called once per frame
	void Update () {
        if (playerCanMove)
        {
            if (stateChanged && stateVariables.stateFinished)
            {
                ResetState();
            }
            if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN) && canDash) // do dodge / dash
            {
                SwitchState(dashState);
                canDash = false;
                StartCoroutine(WaitForDashCoolDown());
            }
            else if (InputManager.Instance.QueryAction(Strings.Input.Actions.EAT, ButtonMode.DOWN) && !playerStates.Contains(dashState) && !playerStates.Contains(eatingState))
            {
                if (CheckForEatableEnemy())
                {
                    SwitchState(eatingState);
                }
            }

            playerStates.ForEach(state => state.StateUpdate());
        }
    }

    void FixedUpdate()
    {
        if (playerCanMove)
        {
            playerStates.ForEach(state => state.StateFixedUpdate());
        }
    }

    private void LateUpdate()
    {
        if (playerCanMove)
        {
            playerStates.ForEach(state => state.StateLateUpdate());
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.gameObject.CompareTag(Strings.Tags.WALL)) {
            stateVariables.velBody.velocity = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        EventSystem instance = EventSystem.Instance;

        if(instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, BlockInput);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, BlockInput);
        }
        
    }
    #endregion

    #region Private Methods

    private void BlockInput(params object[] parameter)
    {
        playerCanMove = !playerCanMove;
        if(!playerCanMove)
        {
            ResetState();
        }
    }
    private void SwitchState(IPlayerState newState)
    {
        if(!playerStates.Contains(newState) && playerStates[0] == defaultState) {
            if (newState.IsBlockingState())
            {
                stateVariables.velBody.velocity = Vector3.zero;
                playerStates.Clear();
            }        
            playerStates.Add(newState);
            stateVariables.stateFinished = false;
            stateChanged = true;
        }
    }

    private void ResetState()
    {
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
        playerStates.Clear();
        playerStates.Add(defaultState);
        stateChanged = false;
    }

    private IEnumerator WaitForDashCoolDown()
    {
        while (!stateVariables.stateFinished)
        {
            yield return null;
        }
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;        
    }

    private bool CheckForEatableEnemy()
    {
        GameObject potentialEatableEnemy = GetClosestEnemy();
        if (potentialEatableEnemy)
        {
            IEatable skinnable = potentialEatableEnemy.GetComponent<IEatable>();
            if (skinnable != null && skinnable.IsEatable())
            {
                stateVariables.eatTargetEnemy = potentialEatableEnemy;
                return true;
            }
        }
        return false;
    }

    private GameObject GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, eatRadius, LayerMask.GetMask(Strings.Tags.ENEMY));
        if (enemies != null)
        {
            Collider closestEnemy = null;
            float closestDistance = Mathf.Infinity;
            foreach (Collider enemy in enemies)
            {
                IEatable eatable = enemy.GetComponent<IEatable>();
                if (eatable != null && eatable.IsEatable())
                {
                    float distance = Vector3.Distance(enemy.ClosestPoint(transform.position), transform.position);
                    if (closestDistance > distance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
            if (closestEnemy != null)
            {
                return closestEnemy.gameObject;
            }
        }
        return null;
    }
    #endregion

    #region Public Structures
    [System.Serializable]
    public class StateVariables
    {
        [HideInInspector]
        public bool stateFinished;
        public float acceleration;
        public float manualDrag;
        [Range (0.01f,.2f)] public float axisThreshold;
        public VelocityBody velBody;        
        public PlayerStatsManager statsManager;
        public Animator animator;
        [HideInInspector]
        public Transform playerTransform;
        [HideInInspector]
        public IPlayerState defaultState;
        [HideInInspector]
        public GameObject eatTargetEnemy;        
        public GameObject modelHead;
        public float clawExtensionTime;
        public float clawRetractionTime;

        public SFXType ArmExtensionSFX;
        public SFXType ArmEnemyCaptureSFX;
        public SFXType EatSFX;
    }
    #endregion

}
