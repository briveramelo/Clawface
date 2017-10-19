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
    //[SerializeField] private Collider playerCollider;
    [SerializeField]
    protected int[] highlightPoses;
    [SerializeField]
    protected int totalAttackPoses;
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

    public override void StateLateUpdate()
    {
        
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
    #endregion

}
