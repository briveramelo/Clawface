
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour {

    #region Public fields
    public StateVariables stateVariables;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private MoveState moveState;
    [SerializeField]
    private SkinningState skinningState;
    [SerializeField]
    private LockOnScript lockOnScript;
    [SerializeField]
    private PlayerModAnimationManager modAnimationManager;
    [SerializeField]
    private ModStateMapping[] modStateMappings;
    #endregion

    #region Private Fields
    private IPlayerState movementState;
    private IPlayerState alternateState;
    private Dictionary<ModType, IPlayerState> modStateDictionary;
    private IPlayerState previousMovementState;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        stateVariables.stateFinished = true;
        moveState.Init(ref stateVariables);
        skinningState.Init(ref stateVariables);
        movementState = moveState;
        alternateState = null;
        modStateDictionary = new Dictionary<ModType, IPlayerState>();
        foreach(ModStateMapping mapping in modStateMappings)
        {
            mapping.state.Init(ref stateVariables);
            modStateDictionary.Add(mapping.modType, mapping.state);
        }
    }
	
	// Update is called once per frame
	void Update () {        
        if(lockOnScript != null)
        {
            stateVariables.currentEnemy = lockOnScript.GetCurrentEnemy();            
        }
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.ACTION_SKIN, ButtonMode.DOWN))
        {
            if (stateVariables.currentEnemy != null && stateVariables.currentEnemy.GetComponent<ISkinnable>().IsSkinnable())
            {
                SwitchState(skinningState);                
            }
        }
        if (movementState != null)
        {
            movementState.StateUpdate();
        }
        if(alternateState != null)
        {
            if (stateVariables.stateFinished)
            {
                movementState = previousMovementState;
                previousMovementState = null;
                alternateState = null;
            }
            else
            {
                alternateState.StateUpdate();
            }
        }
    }

    void FixedUpdate()
    {
        if (movementState != null)
        {
            movementState.StateFixedUpdate();
        }
        if (alternateState != null)
        {
            alternateState.StateFixedUpdate();
        }
    }
    #endregion

    #region Public Methods
    public void Attack(Mod mod)
    {
        if(mod.getModCategory() == ModCategory.Melee)
        {
            if (modStateDictionary[mod.getModType()] != null)
            {
                if (movementState != null)
                {
                    SwitchState(modStateDictionary[mod.getModType()]);
                }                
            }            
        }
        PlayAnimation(mod);
    }
    #endregion

    #region Private Methods
    private void PlayAnimation(Mod mod)
    {
        if (!modAnimationManager.GetIsPlaying())
        {
            if (stateVariables.movementMode == MovementMode.PRECISE && stateVariables.rb.velocity != Vector3.zero)
            {
                modAnimationManager.PlayModAnimation(mod, true);
            }
            else
            {
                modAnimationManager.PlayModAnimation(mod, false);
            }
        }
    }

    private void SwitchState(IPlayerState newState)
    {
        if (movementState != null)
        {
            previousMovementState = movementState;
            movementState = null;
            stateVariables.rb.velocity = Vector3.zero;
        }
        alternateState = newState;
        stateVariables.stateFinished = false;
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
        public float iceForceMultiplier;
        public float manualDrag;
        [Range (0.01f,.2f)] public float axisThreshold;
        public float meleePounceMaxDistance;
        public float meleePounceMinDistance;
        public float meleePounceVelocity;
        public MovementMode movementMode;
        public Rigidbody rb;        
        public PlayerStatsManager statsManager;
        public Animator animator;        
    }

    [System.Serializable]
    public struct ModStateMapping
    {
        public ModType modType;
        public AttackState state;
    }
    #endregion

}
