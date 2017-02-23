using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : MonoBehaviour,IPlayerState {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    #endregion

    #region Private Fields
    private PlayerStateManager.StateVariables stateVariables;
    #endregion

    #region Unity Lifecycle
    public void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
    }

    public void StateFixedUpdate()
    {
        if (CanPounce())
        {
            stateVariables.rb.velocity = transform.forward * stateVariables.meleePounceVelocity;
        }
    }

    public void StateUpdate()
    {
        
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    bool CanPounce()
    {
        if (stateVariables.currentEnemy != null)
        {
            float distance = Vector3.Distance(stateVariables.currentEnemy.transform.position, transform.position);
            return distance <= stateVariables.meleePounceMaxDistance && distance >= stateVariables.meleePounceMinDistance;
        }
        return false;
    }
    #endregion

    #region Private Structures
    #endregion

}
