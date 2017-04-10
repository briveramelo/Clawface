using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBatonAttackState : IPlayerState {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields

    [SerializeField]
    private int attackForwadDisplacement;
    [SerializeField]
    private VFXMeleeSwing[] vfxMeleeSwing;
    [SerializeField]
    private float lookSensitivity;
    #endregion

    #region Private Fields
    private bool isAttackRequested;
    private int frameCount;
    private int currentAttackPose;
    private int leftHandOffset;
    private int highlightPoseIndex;
    private bool weHaveHitHighlightPose;
    #endregion

    #region Unity Lifecycle
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        isAttackRequested = false;
        this.stateVariables = stateVariables;
        frameCount = 0;
        currentAttackPose = 1;
        weHaveHitHighlightPose = false;
        highlightPoseIndex = 0;
    }

    public override void StateFixedUpdate()
    {
        CheckForRotationInput();
        if (CanPounce())
        {
            stateVariables.velBody.velocity = stateVariables.playerTransform.forward * stateVariables.meleePounceVelocity * Time.fixedDeltaTime;
        }
    }

    public override void StateUpdate()
    {
        if (!stateVariables.stateFinished)
        {
            stateVariables.currentMod.Activate();
            if (frameCount == 0)
            {
                ChangePose();
            }
            if (weHaveHitHighlightPose)
            {
                frameCount++;
            }
            if (frameCount > coolDownFrameCount)
            {
                if (frameCount < coolDownFrameCount + inputCheckFrameCount)
                {
                    if (isAttackRequested)
                    {
                        frameCount = 0;
                        weHaveHitHighlightPose = false;
                    }
                }
                else
                {
                    ResetState();
                }
            }else
            {
                isAttackRequested = false;
            }
        }
    }
    #endregion

    #region Public Methods
    public override void Attack()
    {
        isAttackRequested = true;
    }

    public override void SecondaryAttack(bool isHeld, float holdTime)
    {

    }
    #endregion

    #region Private Methods
    private bool CanPounce()
    {
        if (stateVariables.currentEnemy != null)
        {
            float distance = Vector3.Distance(stateVariables.currentEnemy.transform.position, stateVariables.playerTransform.position);
            return distance <= stateVariables.meleePounceMaxDistance && distance >= stateVariables.meleePounceMinDistance;
        }
        return false;
    }

    private void ChangePose()
    {
        stateVariables.modAnimationManager.PlayModAnimation(stateVariables.currentMod, currentAttackPose, totalAttackPoses);        
        currentAttackPose++;
        if(currentAttackPose == highlightPoses[highlightPoseIndex])
        {
            stateVariables.playerTransform.position += stateVariables.playerTransform.forward * attackForwadDisplacement;
            highlightPoseIndex++;
            if(highlightPoseIndex == highlightPoses.Length)
            {
                highlightPoseIndex = 0;
            }
            weHaveHitHighlightPose = true;            
            if (currentAttackPose % 2 != 0)
            {
                vfxMeleeSwing[1].PlayAnimation();
                AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing2);
            }
            else
            {
                vfxMeleeSwing[0].PlayAnimation();
                AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing1);
            }
        }
        if (currentAttackPose > totalAttackPoses)
        {
            currentAttackPose = 1;
        }
    }

    protected override void ResetState()
    {
        stateVariables.stateFinished = true;
        frameCount = 0;
        isAttackRequested = false;
        currentAttackPose = 1;
        highlightPoseIndex = 0;
        weHaveHitHighlightPose = false;
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
    }

    private void CheckForRotationInput()
    {
        Vector2 controllerMoveDir = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT) * lookSensitivity;
        bool isAnyAxisInput = controllerMoveDir.magnitude > stateVariables.axisThreshold;
        if (!isAnyAxisInput)
        {
            controllerMoveDir = Vector2.zero;
        }
        if (controllerMoveDir != Vector2.zero)
        {
            stateVariables.playerTransform.forward = new Vector3(controllerMoveDir.x, 0.0f, controllerMoveDir.y);
        }
    }
    #endregion

    #region Private Structures    
    #endregion

}
