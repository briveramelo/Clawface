using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StunBatonAttackState : IPlayerState {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields

    [SerializeField]
    private int attackForwadDisplacement;
    [SerializeField]
    private VFXMeleeSwing[] vfxMeleeSwing;
    private bool isStarting=true;
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
        /*if (CanPounce())
        {
            stateVariables.velBody.velocity = stateVariables.playerTransform.forward * stateVariables.meleePounceVelocity * Time.fixedDeltaTime;
        }*/
    }

    public override void StateUpdate()
    {
        if (isStarting) {
            isStarting = false;
            Physics.IgnoreLayerCollision((int)Layers.ModMan, (int)Layers.Enemy, true);
        }

        if (!stateVariables.stateFinished)
        {
            if (stateVariables.currentMod.getModSpot() == ModSpot.Legs)
            {
                stateVariables.currentMod.Activate();
                stateVariables.stateFinished = true;
            }
            else
            {
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
                }
                else
                {
                    isAttackRequested = false;
                }
            }
        }        
    }
    #endregion

    #region Public Methods
    public override void Attack()
    {
        isAttackRequested = true;
        stateVariables.currentMod.Activate();
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
        if (currentAttackPose == highlightPoses[highlightPoseIndex])
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
            }
            else
            {
                vfxMeleeSwing[0].PlayAnimation();
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

        Physics.IgnoreLayerCollision((int)Layers.ModMan, (int)Layers.Enemy, false);
        isStarting = true;      
    }
    #endregion

    #region Private Structures    
    #endregion

}
