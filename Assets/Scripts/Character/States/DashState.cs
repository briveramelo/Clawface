using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

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
    public override void Attack()
    {
        throw new NotImplementedException();
    }
    public override void SecondaryAttack(bool isHeld, float holdTime)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        currentFrame = 0;
        currentPose = 0;
        stateVariables.statsManager.damageModifier = 1.0f;
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
                        
        }
        if (currentFrame == iFrameEnd)
        {
            stateVariables.statsManager.damageModifier = 1.0f;
            playerCollider.enabled=true;
        }
    }

    private void MovePlayer()
    {
        Vector3 forward = stateVariables.velBody.GetForward();
        forward.y = 0f;
        stateVariables.velBody.velocity = forward * dashVelocity;
    }
    #endregion

    #region Private Structures
    #endregion

}
