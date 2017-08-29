using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerAttackState : IPlayerState
{

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    #endregion

    #region Private Fields
    private int count;
    #endregion

    #region Unity Lifecycle
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
        count = 0;
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateUpdate()
    {
        if (!stateVariables.stateFinished)
        {
            count++;
            if (stateVariables.currentMod.getModSpot() == ModSpot.ArmL || stateVariables.currentMod.getModSpot() == ModSpot.ArmR)
            {
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
                stateVariables.currentMod.Activate();
            }
            if (count > coolDownFrameCount)
            {
                ResetState();
            }
        }
    }
    #endregion

    #region Public Methods
    public override void Attack()
    {
        
    }

    public override void SecondaryAttack(bool isHeld, float holdTime)
    {
        
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        count = 0;
        stateVariables.stateFinished = true;
    }
    #endregion

    #region Private Structures
    #endregion
}
