﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackState : IPlayerState
{

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
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
    }
    #endregion

    #region Private Structures
    #endregion

}
