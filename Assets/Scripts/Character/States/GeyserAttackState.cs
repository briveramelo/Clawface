using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserAttackState : IPlayerState {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    #endregion

    #region Private Fields
    #endregion

    #region Unity Lifecycle
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
    }
    public override void StateFixedUpdate()
    {
        
    }

    public override void StateUpdate()
    {
        
    }
    #endregion

    #region Public Methods
    public override void Attack()
    {
        //((GeyserMod)stateVariables.currentMod).SetFoot(stateVariables.foot.position);
        stateVariables.currentMod.Activate();
        ResetState();
    }
    public override void SecondaryAttack(bool isHeld, float holdTime)
    {
        //((GeyserMod)stateVariables.currentMod).SetFoot(stateVariables.foot.position);
        //stateVariables.currentMod.AlternateActivate(isHeld, holdTime);
        if (!isHeld)
        {
            ResetState();
        }
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        stateVariables.stateFinished = true;
    }
    #endregion

    #region Private Structures
    #endregion

}
