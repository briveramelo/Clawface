using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using UnityEngine.Assertions;

public class EatingState : IPlayerState
{

    #region Serialized Fields
    [SerializeField]
    private ClawArmController clawArmController;
    #endregion

    #region Private Fields
    private bool isAnimating;
    private Transform clawTransform;
    private GameObject grabObject;
    private GameObject dummyObject;
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        dummyObject = new GameObject();
        dummyObject.name = "Dummy";
        Assert.IsNotNull(clawArmController);
    }

    private void OnEnable()
    {        
        EventSystem.Instance.RegisterEvent(Strings.Events.FACE_OPEN, DoArmExtension);
        EventSystem.Instance.RegisterEvent(Strings.Events.CAPTURE_ENEMY, CaptureEnemy);
        EventSystem.Instance.RegisterEvent(Strings.Events.ARM_ANIMATION_COMPLETE, EndState);
    }
    private void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.FACE_OPEN, DoArmExtension);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CAPTURE_ENEMY, CaptureEnemy);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.ARM_ANIMATION_COMPLETE, EndState);
        }
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
        if (!isAnimating)
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

    protected override void ResetState()
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


    private void DoArmExtension(params object[] parameters)
    {

        if (isAnimating)
        {
            if (stateVariables.eatTargetEnemy)
            {
                stateVariables.statsManager.MakeHappy();
                IEatable eatable = stateVariables.eatTargetEnemy.GetComponent<IEatable>();
                if (eatable != null)
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
            if (eatable != null)
            {
                stateVariables.eatTargetEnemy.transform.position = clawTransform.position;
                eatable.DisableCollider();
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
            if (eatable != null)
            {
                int health;
                eatable.Eat(out health);
                stateVariables.statsManager.TakeHealth(health);
                Stats stats = GetComponent<Stats>();
                EventSystem.Instance.TriggerEvent(Strings.Events.UPDATE_HEALTH, stats.GetHealthFraction());
                GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXMallCopExplosion);
                if (skinningEffect && clawTransform)
                {
                    skinningEffect.transform.position = clawTransform.position;
                }


                GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.VFXHealthGain);
                if (healthJuice)
                {
                    healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f, coroutineName);
                }
                SFXManager.Instance.Play(stateVariables.EatSFX, transform.position);
            }

            else
            {

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
