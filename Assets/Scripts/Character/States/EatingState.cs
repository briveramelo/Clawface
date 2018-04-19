using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EatingState : IPlayerState
{

    #region Serialized Fields
    [SerializeField]
    private ClawArmController clawArmController;

    [SerializeField]
    private Turing.VFX.PlayerFaceController playerFaceController;

    [SerializeField]
    private float happyTimeAfterEating;
    #endregion

    #region Private Fields
    private bool isAnimating;
    private Transform clawTransform;
    private GameObject grabObject;
    private GameObject dummyObject;
    #endregion


    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.EnableDisable; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
                { Strings.Events.FACE_OPEN, DoArmExtension},
                { Strings.Events.CAPTURE_ENEMY, CaptureEnemy},
                { Strings.Events.ARM_ANIMATION_COMPLETE, EndState},
            };
        }
    }
    #endregion
    #region Unity Lifecycle

    protected override void Start()
    {
        base.Start();
        dummyObject = new GameObject("Dummy");
        Assert.IsNotNull(clawArmController);
    }
    
    #endregion

    #region Public Methods

    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
        isAnimating = false;
    }   

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        if (!isAnimating && stateVariables.playerCanMove)
        {
            if (stateVariables.eatTargetEnemy && stateVariables.eatTargetEnemy.activeSelf)
            {
                
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.OpenFace);
                isAnimating = true;
            }
            else
            {
                EventSystem.Instance.TriggerEvent(Strings.Events.DEACTIVATE_MOD);
                ScoreManager.Instance.ResetCombo();
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.OpenFace);
                isAnimating = true;
                //ResetState();
            }
        }
    }
    
    public override void StateLateUpdate()
    {
        LookAtEnemy();
    }

    public override void ResetState()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.FINISHED_EATING);
        clawArmController.ResetClawArm();
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.CloseFace);
        stateVariables.modelHead.transform.LookAt(stateVariables.playerTransform.forward);
        stateVariables.eatTargetEnemy = null;
        clawTransform = null;
        grabObject = null;
        isAnimating = false;
        stateVariables.stateFinished = true;
    }
    #endregion

    #region Private Methods
    private void LookAtEnemy()
    {
        if (stateVariables.eatTargetEnemy)
        {
            Vector3 enemyPosition = stateVariables.eatTargetEnemy.transform.position;
            stateVariables.modelHead.transform.LookAt(new Vector3(enemyPosition.x, 0f, enemyPosition.z));
        }
    }

    private void DoArmExtension(params object[] parameters)
    {

        if (isAnimating)
        {
            if (stateVariables.eatTargetEnemy)
            {
                // stateVariables.statsManager.MakeHappy();
                IEatable eatable = stateVariables.eatTargetEnemy.GetComponent<IEatable>();
                if (!eatable.IsNull())
                {
                    Assert.IsNotNull(eatable);
                    clawArmController.StartExtension(eatable.GetGrabObject(), stateVariables.clawExtensionTime, stateVariables.clawRetractionTime);
                    SFXManager.Instance.Play(stateVariables.ArmExtensionSFX, transform.position);
                }
            }

            else
            {
                stateVariables.eatTargetEnemy = dummyObject;
                dummyObject.transform.position = stateVariables.modelHead.transform.position + stateVariables.modelHead.transform.forward * 5.0f;
                clawArmController.StartExtension(dummyObject, stateVariables.clawExtensionTime, stateVariables.clawRetractionTime);
                SFXManager.Instance.Play(stateVariables.ArmExtensionSFX, transform.position);
            }
        }
        
    }

    private void CaptureEnemy(params object[] parameters)
    {
        if (stateVariables.eatTargetEnemy.activeSelf)
        {
            clawTransform = parameters[0] as Transform;
            IEatable eatable = stateVariables.eatTargetEnemy.GetComponent<IEatable>();
            if (!eatable.IsNull())
            {
                stateVariables.eatTargetEnemy.transform.position = clawTransform.position;                
                eatable.ToggleCollider(false);
                eatable.EnableRagdoll();
            }
        }
        SFXManager.Instance.Play(stateVariables.ArmEnemyCaptureSFX, transform.position);
        //clawArmController.StartRetraction(stateVariables.clawRetractionTime);
    }

    private void DoEating()
    {
        //Check if enemy is still alive
        if (stateVariables.eatTargetEnemy.activeSelf)
        {
            IEatable eatable = stateVariables.eatTargetEnemy.GetComponent<IEatable>();
            if (!eatable.IsNull())
            {
                int health;
                eatable.Eat(out health);
                stateVariables.statsManager.TakeHealth(health);
                Stats stats = GetComponent<Stats>();

                EventSystem.Instance.TriggerEvent(Strings.Events.UPDATE_HEALTH, stats.GetHealthFraction());
                EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_DEATH_BY_EATING);

                GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXMallCopExplosion);
                playerFaceController.SetTemporaryEmotion(Turing.VFX.PlayerFaceController.Emotion.Happy, happyTimeAfterEating);

                if (skinningEffect && clawTransform)
                {
                    skinningEffect.transform.position = clawTransform.position;
                }

                
                GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.VFXHealthGain);
                if (healthJuice)
                {
                    healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f, CoroutineName);
                }
                SFXManager.Instance.Play(stateVariables.EatSFX, transform.position);
            }      
        }
    }

    private void EndState(params object[] parameters)
    {
        DoEating();
        ResetState();
    }
    #endregion

}
