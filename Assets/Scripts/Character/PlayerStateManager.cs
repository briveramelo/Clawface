
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class PlayerStateManager : RoutineRunner {

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
    [SerializeField]
    private float eatCoolDown;

    [SerializeField] private EatingState eatingState;
    [SerializeField] private SphereCollider eatCollider;

    [SerializeField] private float tutorialSlowDownRate = 0.05f;
    [SerializeField] private float tutorialSpeedUpRate = 0.05f;
    #endregion

    #region Private Fields
    private IPlayerState movementState;
    private List<IPlayerState> playerStates;
    private bool canDash = true;
    private bool canEat = true;
    private bool stateChanged;
    private bool playerCanMove = true;
    private bool isTutorialDone;
    private bool isInTutorial;
    private bool isDashTutorialDone;
    private bool isInDashTutorial;
    private bool isSlowDownFinished;
    private bool isSpeedUpFinished;

    const string SlowTime = "SlowTime";
    const string SpeedTime = "SpeedTime";
    const float TutorialRadiusMultiplier = 3f;
    private float tutorialTimeScale = 1.0f;
    #endregion

    #region Unity Lifecycle
    protected override void OnDisable() {
        Timing.KillCoroutines(SlowTime);
        Timing.KillCoroutines(SpeedTime);
        base.OnDisable();
    }

    // Use this for initialization
    void Start () {
        stateChanged = false;
        stateVariables.stateFinished = true;
        stateVariables.playerTransform = transform;
        stateVariables.statsManager = playerStatsManager;
        stateVariables.defaultState = defaultState;
        InitializeStates();

        movementState = defaultState;
        playerStates = new List<IPlayerState>(){ defaultState};
        eatCollider.radius = stateVariables.eatRadius;

        //for input blocking 
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, BlockInput);        
        EventSystem.Instance.RegisterEvent(Strings.Events.FINISHED_EATING, FinishedEat);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, OnPlayerKilled);
        EventSystem.Instance.RegisterEvent(Strings.Events.ENEMY_DEATH_BY_EATING, OnEnemyDeathByEating);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCanMove && !isInTutorial && !isInDashTutorial)
        {
            if (stateChanged && stateVariables.stateFinished)
            {
                ResetState();
            }
            if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN) && canDash) // do dodge / dash
            {
                SwitchState(dashState);
                dashState.StartDash();
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY), LayerMask.NameToLayer(Strings.Layers.MODMAN), true);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY_BODY), LayerMask.NameToLayer(Strings.Layers.MODMAN), true);
                canDash = false;
                StartCoroutine(WaitForDashCoolDown());
            }
            else if (InputManager.Instance.QueryAction(Strings.Input.Actions.EAT, ButtonMode.DOWN) && 
                !playerStates.Contains(dashState) && !playerStates.Contains(eatingState) && canEat)
            {
                CheckForEatableEnemy();
                SwitchState(eatingState);
                canEat = false;
            }

            playerStates.ForEach(state => state.StateUpdate());
        }else if (isInTutorial)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.Actions.EAT, ButtonMode.DOWN) && isSlowDownFinished)
            {
                if (CheckForEatableEnemy())
                {
                    SwitchState(eatingState);
                }
                FinishTutorial();
            }

            else
            {
                SwitchState(defaultState);
                defaultState.StateUpdate();
            }
        }
        else if (isInDashTutorial)
        {

            if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN) && isSlowDownFinished)
            {
                SwitchState(dashState);
                dashState.StartDash();
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY), LayerMask.NameToLayer(Strings.Layers.MODMAN), true);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY_BODY), LayerMask.NameToLayer(Strings.Layers.MODMAN), true);
                FinishDashTutorial();
            }

            else
            {
                SwitchState(defaultState);
                defaultState.StateUpdate();
            }
        }
    }


    void FixedUpdate()
    {
        if (playerCanMove && !isInTutorial)
        {
            if (stateChanged && stateVariables.stateFinished) {
                ResetState();
            }
            playerStates.ForEach(state => state.StateFixedUpdate());
        }
    }

    private void LateUpdate()
    {
        if (playerCanMove && !isInTutorial)
        {
            playerStates.ForEach(state => state.StateLateUpdate());
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.gameObject.CompareTag(Strings.Tags.WALL)) {
            stateVariables.velBody.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SetEnemyCloseToDash(other, true);
        SetEnemyCloseToEat(other, true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isDashTutorialDone && !isInDashTutorial)
        {
            SetEnemyCloseToDash(other, true);
        }
        if (!isTutorialDone && !isInTutorial)
        {
            SetEnemyCloseToEat(other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SetEnemyCloseToDash(other, false);
        SetEnemyCloseToEat(other, false);
    }

    private void OnDestroy()
    {
        EventSystem instance = EventSystem.Instance;

        if(instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, BlockInput);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.FINISHED_EATING, FinishedEat);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, OnPlayerKilled);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.DEATH_ENEMY, OnEnemyDeathByEating);
        }
        
    }

    private void OnEnemyDeathByEating(object[] parameters)
    {
        
    }
    #endregion

    #region Private Methods

    private void SetEnemyCloseToEat(Collider other, bool state)
    {
        if (other.tag.Equals(Strings.Tags.ENEMY) && !isInDashTutorial && isSpeedUpFinished)
        {
            IEatable skinnable = other.GetComponent<IEatable>();
            if (skinnable != null && skinnable.IsEatable())
            {
                EnemyBase enemyBase = other.GetComponent<EnemyBase>();
                if (enemyBase)
                {
                    enemyBase.CloserToEat(state);

                    if (state)
                    {
                        StartTutorial();
                    }
                }
            }
        }
    }

    private void SetEnemyCloseToDash(Collider other, bool state)
    {
        if (other.tag.Equals(Strings.Tags.ENEMY))
        {
            StartDashTutorial();
        }
    }

    private void InitializeStates()
    {
        defaultState.Init(ref stateVariables);
        dashState.Init(ref stateVariables);
        eatingState.Init(ref stateVariables);
    }

    private void StartTutorial()
    {
        if (SettingsManager.Instance.Tutorial && !isTutorialDone && !isInTutorial)
        {
            isInTutorial = true;
            EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_INVINCIBLE, true);
            EventSystem.Instance.TriggerEvent(Strings.Events.GAME_CAN_PAUSE, false);
            eatCollider.radius *= TutorialRadiusMultiplier;
            stateVariables.eatRadius *= TutorialRadiusMultiplier;
            Timing.RunCoroutine(StartTutorialSlowDown(1), SlowTime);
        }
    }

    private void StartDashTutorial()
    {
        if (SettingsManager.Instance.Tutorial && !isDashTutorialDone && !isInDashTutorial)
        {
            isInDashTutorial = true;
            EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_INVINCIBLE, true);
            EventSystem.Instance.TriggerEvent(Strings.Events.GAME_CAN_PAUSE, false);
            //Using the same eat collider to trigger dash tutorial
            eatCollider.radius *= TutorialRadiusMultiplier;
            stateVariables.eatRadius *= TutorialRadiusMultiplier;
            Timing.RunCoroutine(StartTutorialSlowDown(2), SlowTime);
        }
    }

    private IEnumerator<float> StartTutorialSlowDown(int i_eatOrDash)
    {        
        while (tutorialTimeScale > 0.3f)
        {
            tutorialTimeScale = Mathf.Lerp(tutorialTimeScale, 0.2f, tutorialSlowDownRate);
            Time.timeScale = tutorialTimeScale;
            yield return 0f;
        }
        tutorialTimeScale = 0.2f;
        Time.timeScale = tutorialTimeScale;
        EventSystem.Instance.TriggerEvent(Strings.Events.SHOW_TUTORIAL_TEXT,i_eatOrDash);
        EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_INVINCIBLE, false);
        isSlowDownFinished = true;
    }

    private void FinishTutorial()
    {
        if (!isTutorialDone)
        {
            isTutorialDone = true;
            EventSystem.Instance.TriggerEvent(Strings.Events.GAME_CAN_PAUSE, true);
            eatCollider.radius /= TutorialRadiusMultiplier;
            stateVariables.eatRadius /= TutorialRadiusMultiplier;            
            Timing.RunCoroutine(StartTutorialSpeedUp(), SpeedTime);
        }
    }

    private void FinishDashTutorial()
    {
        if (!isDashTutorialDone)
        {
            isDashTutorialDone = true;
            EventSystem.Instance.TriggerEvent(Strings.Events.GAME_CAN_PAUSE, true);
            eatCollider.radius /= TutorialRadiusMultiplier;
            stateVariables.eatRadius /= TutorialRadiusMultiplier;
            Timing.RunCoroutine(StartTutorialSpeedUp(), SpeedTime);
            //Reset the bool to false for the eat tutorial
            isSlowDownFinished = false;
        }
    }

    private IEnumerator<float> StartTutorialSpeedUp()
    {        
        isInTutorial = false;
        isInDashTutorial = false;
        while (tutorialTimeScale < 0.9f)
        {
            tutorialTimeScale = Mathf.Lerp(tutorialTimeScale, 1.0f, tutorialSpeedUpRate);
            Time.timeScale = tutorialTimeScale;
            yield return 0f;
        }
        tutorialTimeScale = 1.0f;
        Time.timeScale = tutorialTimeScale;
        EventSystem.Instance.TriggerEvent(Strings.Events.HIDE_TUTORIAL_TEXT);
        isSpeedUpFinished = true;
    }

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

    private IEnumerator WaitForEatCoolDown()
    {
        while (!stateVariables.stateFinished)
        {
            yield return null;
        }
        yield return new WaitForSeconds(eatCoolDown);
        canEat = true;
        EventSystem.Instance.TriggerEvent(Strings.Events.ACTIVATE_MOD);
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
                stateVariables.eatTargetEnemy.GetComponent<EnemyBase>().MakeIndestructable();
                return true;
            }
        }
        return false;
    }

    private GameObject GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, stateVariables.eatRadius, LayerMask.GetMask(Strings.Tags.ENEMY));
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

    private void FinishedEat(object[] parameters)
    {
        StartCoroutine(WaitForEatCoolDown());
    }

    private void OnPlayerKilled(object[] parameters)
    {
        if (eatCollider)
        {
            eatCollider.enabled = false;
        }
        BlockInput(parameters);
    }
    #endregion

    #region Public Structures
    [Serializable]
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
        public float eatRadius;
        public SFXType ArmExtensionSFX;
        public SFXType ArmEnemyCaptureSFX;
        public SFXType EatSFX;        
        public float dashEnemyCheckRadius = 0.5f;
        public float dashEnemyPushForce = 5.0f;
    }
    #endregion

}
