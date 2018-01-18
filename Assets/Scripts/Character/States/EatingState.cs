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
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
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
                ResetState();
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
        else
        {
            ResetState();
        }
    }

    protected override void ResetState()
    {
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
            stateVariables.statsManager.MakeHappy();
            IEatable eatable = stateVariables.eatTargetEnemy.GetComponent<IEatable>();
            Assert.IsNotNull(eatable);
            clawArmController.StartExtension(eatable.GetGrabObject(), stateVariables.clawExtensionTime, stateVariables.clawRetractionTime);
            SFXManager.Instance.Play(stateVariables.ArmExtensionSFX, transform.position);
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
        //clawArmController.StartRetraction(stateVariables.clawRetractionTime);
        SFXManager.Instance.Play(stateVariables.ArmEnemyCaptureSFX, transform.position);
    }

    private void DoEating()
    {
        //Check if enemy is still alive
        if (stateVariables.eatTargetEnemy.activeSelf)
        {
            IEatable eatable = stateVariables.eatTargetEnemy.GetComponent<IEatable>();
            int health = eatable.Eat();
            stateVariables.statsManager.TakeHealth(health);
            Stats stats = GetComponent<Stats>();
            EventSystem.Instance.TriggerEvent(Strings.Events.UPDATE_HEALTH, stats.GetHealthFraction());
            GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXMallCopExplosion);
            if (skinningEffect) {
                skinningEffect.transform.position = clawTransform.position;
            }


            GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.VFXHealthGain);
            if (healthJuice)
            {
                healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f);
            }
            SFXManager.Instance.Play(stateVariables.EatSFX, transform.position);
        }
    }

    private void EndState(params object[] parameters)
    {
        DoEating();
        ResetState();
    }
    #endregion

}
