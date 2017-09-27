using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class SkinningState : IPlayerState
{

    #region Serialized Unity Inspector fields
    #endregion

    #region Private Fields
    private bool isAnimating;
    #endregion

    #region Unity Lifecycle 

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
            stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.RetractVisor);
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

    public void TriggerArmExtension()
    {
        Debug.Log("Extending!");
        stateVariables.clawAnimator.SetBool(Strings.ANIMATIONSTATE, true);
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
    #endregion

}
