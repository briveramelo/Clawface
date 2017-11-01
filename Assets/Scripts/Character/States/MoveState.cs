using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IPlayerState
{
    #region Private Fields
    private float sphereRadius = 0.1f;
    private Vector3 moveDirection;
    private bool isSidescrolling;
    private bool canMove;
    private Vector3 lastMoveDirection;
    private float currentSpeed;
    private Vector3 lastLookDirection;
    #endregion

    #region Public Methods    
    public override void Init(ref PlayerStateManager.StateVariables moveStateVariables)
    {
        stateVariables = moveStateVariables;
        canMove = true;
        isSidescrolling = false;
        lastMoveDirection = moveStateVariables.playerTransform.forward;
    }

    public override void StateUpdate()
    {                
        Vector2 controllerMoveDir = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
        Vector2 lookDir = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);
        bool isAnyAxisInput = controllerMoveDir.magnitude > stateVariables.axisThreshold;
        if (!isAnyAxisInput) {
            controllerMoveDir = Vector2.zero;
        }
        if(lookDir.magnitude > stateVariables.axisThreshold)
        {
            lastLookDirection = new Vector3(lookDir.x, 0, lookDir.y);
        }else
        {
            lastLookDirection = Vector3.zero;
        }
        Vector2 moveModified = new Vector2(controllerMoveDir.x, controllerMoveDir.y);

        if (isSidescrolling)
        {
            moveModified.y = 0F;
        }

        moveDirection = new Vector3(moveModified.x, 0.0F, moveModified.y);

        if (!canMove)
        {
            moveDirection = Vector3.zero;
            lastMoveDirection = Vector3.zero;
        }

        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0f;
        moveDirection.Normalize();

        lastLookDirection = Camera.main.transform.TransformDirection(lastLookDirection);
        lastLookDirection.y = 0f;
        lastLookDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        } else if (lastLookDirection != Vector3.zero)
        {
            lastMoveDirection = lastLookDirection;
        }
        stateVariables.velBody.MoveDirection = lastMoveDirection;

        switch (stateVariables.velBody.GetMovementMode())
        {
            case MovementMode.PRECISE:
                MovePrecise();
                break;
            case MovementMode.ICE:
                MoveIce();
                break;
        }
        HandleRotation();
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateLateUpdate()
    {
        
    }
    #endregion

    #region Private Methods
    private void MovePrecise() {
        stateVariables.velBody.velocity = moveDirection * stateVariables.statsManager.GetStat(CharacterStatType.MoveSpeed);
        if (moveDirection.magnitude > stateVariables.axisThreshold)
        {   
            if (stateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Running)
            {
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Running);
            }
        }
        else
        {   
            if (stateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
            {
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
                stateVariables.animator.speed = 1.0f;
            }
        }
    }

    private void MoveIce() {
        if (stateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
        {
            stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
            stateVariables.animator.speed = 1.0f;
        }

        Vector3 flatMovement = new Vector3(stateVariables.velBody.velocity.x, 0f, stateVariables.velBody.velocity.z);
        currentSpeed = flatMovement.magnitude;

        if (currentSpeed < stateVariables.statsManager.GetStat(CharacterStatType.MoveSpeed)){ 
            stateVariables.velBody.AddDecayingForce(moveDirection * stateVariables.acceleration * Time.deltaTime);
        }

        if (currentSpeed > stateVariables.statsManager.GetStat(CharacterStatType.MoveSpeed))
        {
            Vector3 currentFlatVelocity = flatMovement;
            currentFlatVelocity = -currentFlatVelocity;
            currentFlatVelocity.Normalize();
            currentFlatVelocity *= (currentSpeed - stateVariables.statsManager.GetStat(CharacterStatType.MoveSpeed));
            currentFlatVelocity *= stateVariables.manualDrag;
            stateVariables.velBody.AddDecayingForce(currentFlatVelocity);
        }
    }

    private void HandleRotation(){
        if (lastLookDirection != Vector3.zero) {
            stateVariables.playerTransform.forward = lastLookDirection;
        }
    }

    protected override void ResetState()
    {
    }
    #endregion

    #region Public Methods
    public void SetSidescrolling(bool mode)
    {
        isSidescrolling = mode;
    }

    public PlayerStateManager.StateVariables GetStateVariables()
    {
        return stateVariables;
    }
    #endregion
}