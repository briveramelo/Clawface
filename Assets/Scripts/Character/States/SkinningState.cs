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
    [SerializeField] private GameObject skinObject;
    [SerializeField] private PlayerStatsManager playerStatsManager;
    [SerializeField] private OnScreenScoreUI healthBar;
    [SerializeField] private float skinRadius;
    #endregion

    #region Private Fields
    #endregion

    #region Unity Lifecycle

    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
    }

    public void RemoveSkin() {
        skinObject.SetActive(false);
    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        ISkinnable skinnable = GetClosestEnemy();        
        if (skinnable!=null && skinnable.IsSkinnable())
        {
            GameObject skin = skinnable.DeSkin();
            skinObject.SetActive(true);
            SkinStats skinStats = skin.GetComponent<SkinStats>();
            playerStatsManager.TakeSkin(skinStats.GetSkinHealth());
            Stats stats = GetComponent<Stats>();
            healthBar.SetHealth(stats.GetHealthFraction());
            GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXSkinningEffect);
            skinningEffect.transform.position = transform.position;

            GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.VFXHealthGain);
            if (healthJuice) {                                
                healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f);
            }
        }        
        stateVariables.stateFinished = true;
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
    }
    
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private ISkinnable GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, skinRadius, LayerMask.GetMask(Strings.Tags.ENEMY));        
        if (enemies != null)
        {
            Collider closestEnemy = null;
            float closestDistance = Mathf.Infinity;
            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(enemy.ClosestPoint(transform.position), transform.position);
                if(closestDistance > distance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            if(closestEnemy != null)
            {
                return closestEnemy.gameObject.GetComponent<ISkinnable>();
            }
        }
        return null;
    }

    protected override void ResetState()
    {
    }
    #endregion

    #region Private Structures
    #endregion

}
