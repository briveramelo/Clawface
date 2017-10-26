using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class EatingState : IPlayerState
{

    #region Private Fields
    private bool isAnimating;
    #endregion

    #region Unity Lifecycle 
    private void OnEnable()
    {        
        EventSystem.Instance.RegisterEvent(Strings.Events.FACE_OPEN, DoArmExtension);
        EventSystem.Instance.RegisterEvent(Strings.Events.ARM_EXTENDED, CaptureEnemy);
        EventSystem.Instance.RegisterEvent(Strings.Events.ARM_ANIMATION_COMPLETE, EndState);
    }
    private void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.FACE_OPEN, DoArmExtension);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.ARM_EXTENDED, CaptureEnemy);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.ARM_ANIMATION_COMPLETE, EndState);
        }
    }

    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
        isAnimating = false;
    }

    private void LookAtEnemy()
    {
        Vector3 enemyPosition = stateVariables.skinTargetEnemy.transform.position;
        stateVariables.modelHead.transform.LookAt(new Vector3(enemyPosition.x, 0f, enemyPosition.z));
    }

    public override void StateFixedUpdate()
    {
        if (!isAnimating)
        {
            if (stateVariables.skinTargetEnemy.activeSelf) {
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.RetractVisor);
                isAnimating = true;
            }
            else
            {
                ResetState();
            }
        }
    }

    public override void StateUpdate()
    {
        
    }
    
    public override void StateLateUpdate()
    {
        LookAtEnemy();
    }    
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        stateVariables.clawAnimator.SetBool(Strings.ANIMATIONSTATE, false);
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
        stateVariables.modelHead.transform.LookAt(stateVariables.playerTransform.forward);
        isAnimating = false;
        stateVariables.stateFinished = true;
    }


    private void DoArmExtension(params object[] parameters)
    {        
        stateVariables.clawAnimator.SetBool(Strings.ANIMATIONSTATE, true);
    }

    private void CaptureEnemy(params object[] parameters)
    {
        if (stateVariables.skinTargetEnemy.activeSelf)
        {
            Transform clawTransform = parameters[0] as Transform;
            stateVariables.skinTargetEnemy.transform.SetParent(clawTransform);
            stateVariables.skinTargetEnemy.transform.localPosition = Vector3.zero;
        }
    }

    private void DoSkinning()
    {
        //Check if enemy is still alive
        if (stateVariables.skinTargetEnemy.activeSelf)
        {
            ISkinnable skinnable = stateVariables.skinTargetEnemy.GetComponent<ISkinnable>();
            GameObject skin = skinnable.DeSkin();
            SkinStats skinStats = skin.GetComponent<SkinStats>();
            stateVariables.statsManager.TakeSkin(skinStats.GetSkinHealth());
            Stats stats = GetComponent<Stats>();
            EventSystem.Instance.TriggerEvent(Strings.Events.UPDATE_HEALTH, stats.GetHealthFraction());
            GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXSkinningEffect);
            skinningEffect.transform.position = transform.position;

            GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.VFXHealthGain);
            if (healthJuice)
            {
                healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f);
            }
        }
    }

    private void EndState(params object[] parameters)
    {
        DoSkinning();
        ResetState();
    }
    #endregion

}
