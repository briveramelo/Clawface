using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangAttackState : IPlayerState {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private VFXMeleeSwing vfx;
    #endregion

    #region Private Fields
    private int currentAttackPose;
    private int frameCount;    
    #endregion

    #region Unity Lifecycle
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
        currentAttackPose = 1;
        frameCount = 0;
    }

    public override void StateFixedUpdate()
    {
    }

    public override void StateUpdate()
    {
        if (!stateVariables.stateFinished)
        {
            if (frameCount == 0)
            {
                stateVariables.modAnimationManager.PlayModAnimation(stateVariables.currentMod, (float)currentAttackPose / (float)totalAttackPoses);
                currentAttackPose++;
                if (currentAttackPose > highlightPoses[0])
                {
                    vfx.PlayAnimation();
                    frameCount++;
                }
            }else if(frameCount < coolDownFrameCount)
            {
                frameCount++;
            }else
            {
                ResetState();
            }
        }
    }
    #endregion

    #region Public Methods
    public override void Attack()
    {
        stateVariables.currentMod.Activate();
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        stateVariables.stateFinished = true;
        frameCount = 0;
        currentAttackPose = 1;
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
    }
    #endregion

    #region Private Structures
    #endregion

}
