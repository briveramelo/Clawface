﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;
using ModMan;

[RequireComponent(typeof(EatingState))]
public class DashState : IPlayerState {

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
    [SerializeField]
    protected int[] highlightPoses;
    [SerializeField]
    protected int totalAttackPoses;
    [SerializeField]
    private ClawArmController clawArmController;
    #endregion

    #region Private Fields
    private int currentFrame;
    private int currentPose;
    private bool isClawOut;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        isClawOut = false;
    }

    // Use this for initialization
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;        
        currentFrame = 0;
        currentPose = 0;
    }

    public override void StateFixedUpdate()
    {
        if (currentFrame == 0) {
            SFXManager.Instance.Play(SFXType.Dash, transform.position);
            dashPuff.Play();
            dashTrail.GetComponent<TrailRenderer>().enabled = true;
            if (!isClawOut)
            {
                isClawOut = true;
                clawArmController.ExtendClawToDistance(stateVariables.clawPunchDistance);
            }
        }
        currentFrame++;
        PlayAnimation();
        CheckForIFrames();
        MovePlayer();
        BlastEm();
        if (currentFrame >= totalDashFrames) {
            ResetState();
        }
    }

    public override void StateUpdate()
    {
        
    }

    public override void StateLateUpdate()
    {
        stateVariables.modelHead.transform.forward = stateVariables.velBody.MoveDirection;
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY), LayerMask.NameToLayer(Strings.Layers.MODMAN), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY_BODY), LayerMask.NameToLayer(Strings.Layers.MODMAN), false);
        currentFrame = 0;
        currentPose = 0;
        stateVariables.statsManager.damageModifier = 1.0f;
        if (stateVariables.velBody.GetMovementMode()==MovementMode.ICE) {
            stateVariables.velBody.velocity = stateVariables.velBody.GetForward() * dashVelocity/10f;
        }
        dashTrail.GetComponent<TrailRenderer>().enabled = false;
        clawArmController.ResetClawArm();
        isClawOut = false;
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
        }else if (currentFrame == iFrameEnd)
        {
            stateVariables.statsManager.damageModifier = 1.0f;
        }
    }

    private void MovePlayer()
    {
        Vector3 direction = stateVariables.velBody.MoveDirection;
        direction.y = 0f;
        stateVariables.velBody.velocity = direction * dashVelocity;
    }

    private void BlastEm()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, stateVariables.dashBlastRadius, LayerMask.GetMask(Strings.Layers.ENEMY));
        foreach(Collider enemy in enemies)
        {
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase)
            {
                enemyBase.Push(stateVariables.dashBlastForce, transform.position, stateVariables.dashBlastRadius);
            }
        }
    }
    #endregion

}
