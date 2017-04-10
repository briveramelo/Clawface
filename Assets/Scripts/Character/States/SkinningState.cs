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
        ISkinnable skinnable =stateVariables.currentEnemy.GetComponent<ISkinnable>();
        if (skinnable!=null){ 
            //TODO apply skin texture to player model
            GameObject skin = skinnable.DeSkin();
            SkinStats skinStats = skin.GetComponent<SkinStats>();
            Stats stats = GetComponent<Stats>();
            stats.Modify(StatType.Health, skinStats.GetSkinHealth());
            HealthBar.Instance.SetHealth(stats.GetHealthFraction());
        }
        stateVariables.stateFinished = true;
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
    }
    #endregion

    #region Private Structures
    #endregion

}
