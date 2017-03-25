using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : MonoBehaviour,IPlayerState {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private int coolDownFrameCount;
    [SerializeField]
    private int inputCheckFrameCount;
    [SerializeField]
    private int totalAttackPoses;
    [SerializeField]
    private int attackForwadDisplacement;
    [SerializeField]
    VFXMeleeSwing[] vfxMeleeSwing;
    #endregion

    #region Private Fields
    [HideInInspector]
    private PlayerStateManager.StateVariables stateVariables;
    private bool isAttackRequested;
    private int frameCount;
    private int currentAttackPose;
    #endregion

    #region Unity Lifecycle
    public void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        isAttackRequested = false;
        this.stateVariables = stateVariables;
        frameCount = 0;
        currentAttackPose = 1;
    }

    public void StateFixedUpdate()
    {
        if (CanPounce())
        {
            stateVariables.velBody.velocity = stateVariables.playerTransform.forward * stateVariables.meleePounceVelocity * Time.fixedDeltaTime;
        }
    }

    public void StateUpdate()
    {
        if (!stateVariables.stateFinished)
        {
            stateVariables.currentMod.Activate();
            if (frameCount == 0)
            {
                ChangePose();
            }
            frameCount++;
            if (frameCount > coolDownFrameCount)
            {
                if (frameCount < coolDownFrameCount + inputCheckFrameCount)
                {
                    if (isAttackRequested)
                    {
                        frameCount = 0;
                    }
                }
                else
                {
                    ResetVariables();
                }
            }else
            {
                isAttackRequested = false;
            }
        }
    }
    #endregion

    #region Public Methods
    public void Attack()
    {
        isAttackRequested = true;
    }
    #endregion

    #region Private Methods
    bool CanPounce()
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
        print("Current attack pose " + currentAttackPose);
        stateVariables.playerTransform.position += stateVariables.playerTransform.forward * attackForwadDisplacement;
        stateVariables.modAnimationManager.PlayModAnimation(stateVariables.currentMod, (float)currentAttackPose / (float)totalAttackPoses);
        vfxMeleeSwing[currentAttackPose - 1].PlayAnimation();
        currentAttackPose++;
        if (currentAttackPose > totalAttackPoses)
        {
            currentAttackPose = 1;
        }
    }

    private void ResetVariables()
    {
        stateVariables.stateFinished = true;
        frameCount = 0;
        isAttackRequested = false;
        currentAttackPose = 1;
    }
    #endregion

    #region Private Structures
    #endregion

}
