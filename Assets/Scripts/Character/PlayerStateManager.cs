
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
    private DashState dashState;
    [SerializeField]
    private float dashCoolDown;
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
        stateChanged = false;
        stateVariables.stateFinished = true;
        stateVariables.playerTransform = transform;
        stateVariables.modAnimationManager = modAnimationManager;
        stateVariables.statsManager = playerStatsManager;
        stateVariables.defaultState = defaultState;
        defaultState.Init(ref stateVariables);
        skinningState.Init(ref stateVariables);
        dashState.Init(ref stateVariables);
        movementState = defaultState;
        alternateState = null;
        modStateDictionary = new Dictionary<ModType, IPlayerState>();
        foreach(ModStateMapping mapping in modStateMappings)
        {
            mapping.state.Init(ref stateVariables);
            modStateDictionary.Add(mapping.modType, mapping.state);
        }
        playerStates = new List<IPlayerState>(){ defaultState};
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
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.SKIN, ButtonMode.DOWN))
        {           
            SwitchState(skinningState);
        } else if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN) && canDash && stateVariables.stateFinished) // do dodge / dash
        {
            SwitchState(dashState);
            canDash = false;
            StartCoroutine(WaitForDashCoolDown());
        }            
        playerStates.ForEach(state=>state.StateUpdate());
    }    

    void FixedUpdate()
    {
        playerStates.ForEach(state=>state.StateFixedUpdate());
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.gameObject.CompareTag(Strings.Tags.WALL)) {
            stateVariables.velBody.velocity = Vector3.zero;
        }
    }
    #endregion

    #region Public Methods
    public void Attack(Mod mod){
        if (stateVariables.currentMod != mod){
            stateVariables.currentMod = mod;
        }
        if (stateVariables.currentMod.hasState){
            if (modStateDictionary[mod.getModType()] != null){
                if (movementState != null){
                    SwitchState(modStateDictionary[mod.getModType()]);
                    modStateDictionary[mod.getModType()].Attack();
                }                
            }
        }
    }

    public void Charge(Mod mod) {
        if (mod.modEnergySettings.timeCharged > 0.5f) {

        }
    }

    public void SecondaryAttack(Mod mod, bool isHeld)
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
    }
    #endregion

    #region Private Methods
    private void SwitchState(IPlayerState newState)
    {
        if(playerStates[0] == defaultState) {
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
        stateVariables.animator.Play(PlayerAnimationStates.Idle.ToString());
        playerStates.Clear();
        playerStates.Add(defaultState);
        stateChanged = false;
    }

    private IEnumerator WaitForDashCoolDown()
    {
        yield return new WaitForSeconds(dashCoolDown);
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
        [HideInInspector]
        public IPlayerState defaultState;
    }

    [System.Serializable]
    public struct ModStateMapping
    {
        public ModType modType;
        public IPlayerState state;
    }
    #endregion

}
