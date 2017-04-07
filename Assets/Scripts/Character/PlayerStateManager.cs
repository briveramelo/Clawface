
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
    private SkinningState skinningState;
    [SerializeField]
    private LockOnScript lockOnScript;
    [SerializeField]
    private PlayerStatsManager playerStatsManager;
    [SerializeField]
    private PlayerModAnimationManager modAnimationManager;
    [SerializeField]
    private ModStateMapping[] modStateMappings;
    [SerializeField]
    private float holdAttackSlowDown;
    [SerializeField]
    private float dashPower = 100.0F;
    [SerializeField]
    private float dashDecay = 0.25F;
    [SerializeField]
    private float dashTime = 1.0F; // seconds
    [SerializeField]
    private float dashIFrameStart = 0.25F;
    [SerializeField]
    private float dashIFrameEnd = 0.75F;
    #endregion

    #region Private Fields
    private IPlayerState movementState;
    private IPlayerState alternateState;
    private Dictionary<ModType, IPlayerState> modStateDictionary;
    private IPlayerState previousMovementState;
    private List<IPlayerState> playerStates;
    private bool isHoldAttack;
    private bool canDash = true;
    private bool stateChanged;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        isHoldAttack = false;
        stateChanged = false;
        stateVariables.stateFinished = true;
        stateVariables.playerTransform = transform;
        stateVariables.modAnimationManager = modAnimationManager;
        defaultState.Init(ref stateVariables);
        skinningState.Init(ref stateVariables);
        movementState = defaultState;
        alternateState = null;
        modStateDictionary = new Dictionary<ModType, IPlayerState>();
        foreach(ModStateMapping mapping in modStateMappings)
        {
            mapping.state.Init(ref stateVariables);
            modStateDictionary.Add(mapping.modType, mapping.state);
        }
        playerStates = new List<IPlayerState>();
        playerStates.Add(defaultState);
    }
	
	// Update is called once per frame
	void Update () {
        if (stateChanged && stateVariables.stateFinished)
        {
            ResetState();
        }
        if (lockOnScript != null)
        {
            stateVariables.currentEnemy = lockOnScript.GetCurrentEnemy();
        }
        //if (!modAnimationManager.GetIsPlaying())
        //{
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_SKIN, ButtonMode.DOWN))
            {
                if (stateVariables.currentEnemy != null && stateVariables.currentEnemy.GetComponent<ISkinnable>().IsSkinnable())
                {
                    SwitchState(skinningState);
                }
            } else if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN) && canDash) // do dodge / dash
            {
                ResetState();
                Vector2 dir = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
                Vector3 force = Camera.main.transform.TransformDirection(new Vector3(dir.x, 0, dir.y));
                force.y = 0;
                force.Normalize();
                stateVariables.velBody.AddDecayingForce(dashPower * force, dashDecay);
                StartCoroutine(DashController());
            }
            foreach (IPlayerState state in playerStates)
            {
                state.StateUpdate();
            }
        //}
    }

    void FixedUpdate()
    {
        //if (!modAnimationManager.GetIsPlaying())
        //{
            foreach (IPlayerState state in playerStates)
            {
                state.StateFixedUpdate();
            }            
        //}
    }
    #endregion

    #region Public Methods
    public void PrimaryAttack(Mod mod)
    {
        if(stateVariables.currentMod != mod)
        {
            stateVariables.currentMod = mod;
        }
        if (stateVariables.currentMod.hasState)
        {
            
            if (modStateDictionary[mod.getModType()] != null)
            {
                if (movementState != null)
                {                    
                    SwitchState(modStateDictionary[mod.getModType()]);
                    ((AttackState)modStateDictionary[mod.getModType()]).Attack();
                }                
            }
        }else
        {
            stateVariables.currentMod.Activate();
        }
    }

    public void SecondaryAttack(Mod mod, bool isHeld, float holdTime)
    {
        if (isHeld && !isHoldAttack)
        {
            isHoldAttack = true;
            stateVariables.velBody.velocity = Vector3.zero;
            stateVariables.statsManager.ModifyStat(StatType.MoveSpeed, 1.0f / holdAttackSlowDown);
        }
        else if(!isHeld)
        {
            isHoldAttack = false;
            stateVariables.velBody.velocity = Vector3.zero;
            stateVariables.statsManager.ModifyStat(StatType.MoveSpeed, holdAttackSlowDown);
        }
        mod.AlternateActivate(isHeld, holdTime);
    }
    #endregion

    #region Private Methods
    private void SwitchState(IPlayerState newState)
    {
        stateVariables.velBody.velocity = Vector3.zero;
        playerStates.Clear();
        playerStates.Add(newState);
        stateVariables.stateFinished = false;
        stateChanged = true;
    }

    private void ResetState()
    {
        stateVariables.animator.Play(PlayerAnimationStates.Idle.ToString());
        playerStates.Clear();
        playerStates.Add(defaultState);
        stateChanged = false;
    }

    private IEnumerator DashController()
    {
        canDash = false;
        yield return new WaitForSeconds(dashIFrameStart);
        playerStatsManager.damageModifier = 0.0F;
        yield return new WaitForSeconds(dashIFrameEnd - dashIFrameStart);
        playerStatsManager.damageModifier = 1.0F;
        yield return new WaitForSeconds(dashTime = dashIFrameEnd);
        canDash = true;
    }
    #endregion

    #region Private Structures
    [System.Serializable]
    public class StateVariables
    {
        [HideInInspector]
        public GameObject currentEnemy;
        [HideInInspector]
        public Mod currentMod;
        [HideInInspector]
        public bool stateFinished;
        public Transform foot;
        public float acceleration;
        public float manualDrag;
        [Range (0.01f,.2f)] public float axisThreshold;
        public float meleePounceMaxDistance;
        public float meleePounceMinDistance;
        public float meleePounceVelocity;
        public VelocityBody velBody;        
        public PlayerStatsManager statsManager;
        public Animator animator;
        [HideInInspector]
        public Transform playerTransform;
        [HideInInspector]
        public PlayerModAnimationManager modAnimationManager;
    }

    [System.Serializable]
    public struct ModStateMapping
    {
        public ModType modType;
        public AttackState state;
    }
    #endregion

}
