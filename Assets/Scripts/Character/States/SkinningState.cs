using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class SkinningState : IPlayerState
{
    
    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Transform skinSlot;
    [SerializeField] private List<CapsuleCollider> clothColliders;
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
            GameObject skin = skinnable.DeSkin();
            skin.transform.SetParent(skinSlot);
            skin.transform.ResetFull();            
            skin.GetComponent<Cloth>().capsuleColliders = clothColliders.ToArray();

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
