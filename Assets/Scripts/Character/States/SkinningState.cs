using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinningState : IPlayerState
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
        stateVariables.currentEnemy.GetComponent<ISkinnable>().DeSkin();
        stateVariables.stateFinished = true;
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
