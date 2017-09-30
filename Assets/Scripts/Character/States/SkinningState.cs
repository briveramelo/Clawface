using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class SkinningState : IPlayerState
{

    #region Serialized Unity Inspector fields
    [SerializeField] private OnScreenScoreUI healthBar;
    #endregion

    #region Private Fields
    private bool isAnimating;
    #endregion

    #region Unity Lifecycle 
    private void Awake()
    {
        PlayerAnimationEventsListener.FaceOpenEvent.AddListener(DoArmExtension);
        ClawAnimationEventsListener.ClawArmExtendedEvent.AddListener(DoSkinning);
    }

    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
        isAnimating = false;
    }

    public override void StateFixedUpdate()
    {
        if (!isAnimating)
        {
            Debug.Log("Entering Skinning state");
            if (stateVariables.skinTargetEnemy.activeSelf) {
                Vector3 enemyPosition = stateVariables.skinTargetEnemy.transform.position;
                stateVariables.playerTransform.LookAt(new Vector3(enemyPosition.x, 0f, enemyPosition.z));
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.RetractVisor);
            }
            else
            {
                ResetState();
            }
        }
        else
        {
            if(stateVariables.clawAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
            {
                ResetState();
            }
        }
    }

    public override void StateUpdate()
    {
        
    }

    private void OnDestroy()
    {
        PlayerAnimationEventsListener.FaceOpenEvent.RemoveListener(DoArmExtension);
        ClawAnimationEventsListener.ClawArmExtendedEvent.RemoveListener(DoSkinning);
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        Debug.Log("Exiting Skinning state");
        stateVariables.clawAnimator.SetBool(Strings.ANIMATIONSTATE, false);
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
        isAnimating = false;
        stateVariables.stateFinished = true;
    }


    private void DoArmExtension()
    {
        Debug.Log("Extending!");
        stateVariables.clawAnimator.SetBool(Strings.ANIMATIONSTATE, true);
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
            healthBar.SetHealth(stats.GetHealthFraction());
            GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXSkinningEffect);
            skinningEffect.transform.position = transform.position;

            GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.VFXHealthGain);
            if (healthJuice)
            {
                healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f);
            }
        }
    }
    #endregion

}
