using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;
using ModMan;

public class DashState : IPlayerState {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private int totalDashFrames;
    [SerializeField]
    private int iFrameStart;
    [SerializeField]
    private int iFrameEnd;
    [SerializeField]
    private float dashVelocity;
    [SerializeField]
    private VFXOneOff dashPuff;
    [SerializeField]
    private GameObject dashTrail;
    [SerializeField] private Collider playerCollider;
    [SerializeField]
    protected int[] highlightPoses;
    [SerializeField]
    protected int totalAttackPoses;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float skinRadius;
    #endregion

    #region Private Fields
    private int currentFrame;
    private int currentPose;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
        currentFrame = 0;
        currentPose = 0;        
    }

    public override void StateFixedUpdate()
    {
        if (currentFrame == 0)
        {
            SFXManager.Instance.Play(SFXType.Dash, transform.position);
            dashPuff.Play();
            dashTrail.GetComponent<TrailRenderer>().enabled = true;
        }
        currentFrame++;
        PlayAnimation();
        CheckForIFrames();
        MovePlayer();
        SkinEnemies();
        if (currentFrame == totalDashFrames)
        {
            ResetState();
        }
    }

    public override void StateUpdate()
    {
        
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    protected override void ResetState()
    {        
        currentFrame = 0;
        currentPose = 0;
        stateVariables.statsManager.damageModifier = 1.0f;
        playerCollider.enabled = true;
        if (stateVariables.velBody.GetMovementMode()==MovementMode.ICE) {
            stateVariables.velBody.velocity = stateVariables.velBody.GetForward() * dashVelocity/10f;
        }
        dashTrail.GetComponent<TrailRenderer>().enabled = false;
        stateVariables.stateFinished = true;
    }

    private void PlayAnimation()
    {
        if (currentPose != highlightPoses[0] || totalDashFrames - currentFrame <= totalAttackPoses / 2)
        {
            
            if (currentPose <= totalAttackPoses)
            {
                currentPose++;
            }            
        }
        stateVariables.animator.Play(PlayerAnimationStates.Dash.ToString(), -1, currentPose / (float)totalAttackPoses);
    }

    private void CheckForIFrames()
    {
        if(currentFrame == iFrameStart)
        {
            stateVariables.statsManager.damageModifier = 0.0f;
            playerCollider.enabled=false;
        }else if (currentFrame == iFrameEnd)
        {
            stateVariables.statsManager.damageModifier = 1.0f;
            playerCollider.enabled=true;
        }
    }

    private void MovePlayer()
    {
        Vector3 direction = stateVariables.velBody.MoveDirection;
        direction.y = 0f;
        stateVariables.velBody.velocity = direction * dashVelocity;
    }
    
    private void SkinEnemies()
    {
        ISkinnable skinnable = GetClosestEnemy();
        if (skinnable != null && skinnable.IsSkinnable())
        {
            GameObject skin = skinnable.DeSkin();
            SkinStats skinStats = skin.GetComponent<SkinStats>();
            stateVariables.statsManager.TakeSkin(skinStats.GetSkinHealth());
            Stats stats = GetComponent<Stats>();
            healthBar.SetHealth(stats.GetHealthFraction());
            GameObject skinningEffect = ObjectPool.Instance.GetObject(PoolObjectType.SkinningEffect);
            skinningEffect.transform.position = transform.position;

            GameObject healthJuice = ObjectPool.Instance.GetObject(PoolObjectType.HealthGain);
            if (healthJuice)
            {
                healthJuice.FollowAndDeActivate(3f, transform, Vector3.up * 3.2f);
            }
        }
    }

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
                if (closestDistance > distance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            if (closestEnemy != null)
            {
                return closestEnemy.gameObject.GetComponent<ISkinnable>();
            }
        }
        return null;
    }
    #endregion

    #region Private Structures
    #endregion

}
