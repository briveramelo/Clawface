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
    [SerializeField]
    private float wallPushBack;
    #endregion

    #region Private Fields
    private bool isAttackRequested;
    private int frameCount;
    private int currentAttackPose;
    private int leftHandOffset;
    private int highlightPoseIndex;
    private bool weHaveHitHighlightPose;
    private bool isHittingAWall;
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
        isHittingAWall = false;
    }

    public override void StateFixedUpdate()
    { 
    }

    public override void StateUpdate()
    {
        DisableEnemyCollision();

        if (!stateVariables.stateFinished)
        {
            if (stateVariables.currentMod.getModSpot() == ModSpot.Legs)
            {
                ActivateLegs();
            }
            else
            {
                if (frameCount == 0)
                {
                    //Not reached highlight pose
                    ChangePose();
                }
                if (weHaveHitHighlightPose)
                {
                    //Start state cooldown counter
                    frameCount++;
                }
                if (frameCount > coolDownFrameCount)
                {
                    //Counter is past cooldown
                    if (frameCount < coolDownFrameCount + inputCheckFrameCount)
                    {
                        //Check if player has requested for input
                        if (isAttackRequested)
                        {
                            //Reset cooldown counter
                            frameCount = 0;
                            //Fire the mod
                            weHaveHitHighlightPose = false;
                            stateVariables.currentMod.modEnergySettings.isActive = false;
                            stateVariables.currentMod.Activate();
                            stateVariables.currentMod.modEnergySettings.isActive = true;
                        }
                    }
                    else
                    {
                        //No input requested
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == Strings.Tags.WALL)
        {
            isHittingAWall = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == Strings.Tags.WALL)
        {
            isHittingAWall = false;
        }
    }
    #endregion

    #region Public Methods
    public override void Attack()
    {
        isAttackRequested = true;
        if (!stateVariables.currentMod.modEnergySettings.isActive)
        {
            stateVariables.currentMod.Activate();
            if (stateVariables.currentMod.getModSpot() != ModSpot.Legs) {
                stateVariables.currentMod.modEnergySettings.isActive = true;
            }
        }
    }

    public override void SecondaryAttack(bool isHeld, float holdTime)
    {

    }
    #endregion

    #region Private Methods
    private void ActivateLegs()
    {
        stateVariables.currentMod.Activate();
        stateVariables.stateFinished = true;
    }

    private void DisableEnemyCollision()
    {
        if (isStarting)
        {
            isStarting = false;
            Physics.IgnoreLayerCollision((int)Layers.ModMan, (int)Layers.Enemy, true);
        }
    }

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
            if (!isHittingAWall)
            {
                stateVariables.playerTransform.position += stateVariables.playerTransform.forward * attackForwadDisplacement;
            }else
            {
                stateVariables.playerTransform.position -= stateVariables.playerTransform.forward * wallPushBack;
            }
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
        stateVariables.currentMod.modEnergySettings.isActive = false;
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
