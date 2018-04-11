using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;
using ModMan;

[RequireComponent(typeof(EatingState))]
public class DashState : IPlayerState {

    #region Serialized Unity Inspector fields
    [SerializeField]
    private HitFlasher hitFlasher;
    [SerializeField] private float dashFlashIntensity;
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
    [SerializeField]
    protected int[] highlightPoses;
    [SerializeField]
    protected int totalAttackPoses;

    [SerializeField]
    private float totalDashTime;

    [SerializeField]
    private float dashITimeStart;

    [SerializeField]
    private float dashITimeEnd;

    [SerializeField]
    private float dashComboTime;

    #endregion

    #region Private Fields
    private int currentFrame;
    private int currentPose;
    private bool isClawOut;
    private float currentRotation;

    [SerializeField]
    private float dashTimer;

    [SerializeField]
    private float dashComboTimer;
    #endregion

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();
        isClawOut = false;
        currentRotation = 0.0f;
    }

    // Use this for initialization
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;        
        currentFrame = 0;
        currentPose = 0;
        dashTimer = 0f;
        dashComboTimer = 0f;
    }

    public void Update()
    {
        dashComboTimer += Time.deltaTime;
    }


    public override void StateFixedUpdate()
    {
        
    }

    public void StartDash()
    {
        SFXManager.Instance.Play(SFXType.Dash, transform.position);
        hitFlasher.FixColor(dashFlashIntensity, totalDashTime);
        dashTimer = 0f;
        dashPuff.Play();
        dashTrail.GetComponent<TrailRenderer>().enabled = true;
    }

    public override void StateUpdate()
    {
        dashTimer += Time.deltaTime;
        
        PlayAnimation();
        MovePlayer();

        if (dashTimer >= totalDashTime) ResetState();
    }

    public override void StateLateUpdate()
    {
        
    }

    public override void ResetState()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY), LayerMask.NameToLayer(Strings.Layers.MODMAN), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY_BODY), LayerMask.NameToLayer(Strings.Layers.MODMAN), false);
        currentFrame = 0;
        currentPose = 0;
        dashTimer = 0f;
        stateVariables.statsManager.damageModifier = 1.0f;
        if (stateVariables.velBody.GetMovementMode() == MovementMode.ICE)
        {
            stateVariables.velBody.velocity = stateVariables.velBody.GetForward() * dashVelocity / 10f;
        }
        dashTrail.GetComponent<TrailRenderer>().Clear();
        dashTrail.GetComponent<TrailRenderer>().enabled = false;
        isClawOut = false;
        currentRotation = 0.0f;
        stateVariables.stateFinished = true;
    }
    #endregion

    #region Private Methods
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

    public bool CheckForIFrames()
    {
        if (dashTimer >= dashITimeStart && dashTimer < dashITimeEnd) return true;

        return false;
    }

    public bool CheckIfDashGivesCombo()
    {
        if (dashComboTimer >= dashComboTime) return true;
        return false;
    }

    public void ResetDashComboTimer()
    {
        dashComboTimer = 0f;
    }

    private void MovePlayer()
    {
        Vector3 direction = stateVariables.velBody.MoveDirection;
        direction.y = 0f;
        stateVariables.velBody.velocity = direction * dashVelocity;
    }    

    private void PushEnemies()
    {
        string[] layers = { Strings.Layers.ENEMY };
        Collider[] colliders = Physics.OverlapSphere(transform.position, stateVariables.dashEnemyCheckRadius, LayerMask.GetMask(layers));
        foreach (Collider collider in colliders)
        {            
            EnemyBase enemyBase = collider.GetComponent<EnemyBase>();
            if (enemyBase)
            {
                Vector3 direction = transform.forward;
                direction.y = 0f;
                enemyBase.Push(stateVariables.dashEnemyPushForce);
            }
        }
    }
    #endregion

}
