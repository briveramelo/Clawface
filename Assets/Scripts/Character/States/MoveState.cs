using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : MonoBehaviour, IPlayerState
{
    #region Private Fields
    PlayerStateManager.StateVariables moveStateVariables;
    private float sphereRadius = 0.1f;
    private Vector3 moveDirection;
    private bool isSidescrolling;
    private bool canMove;
    private Vector3 lastMoveDirection;
    private float currentSpeed;
    #endregion

    #region Public Methods    
    public void Init(ref PlayerStateManager.StateVariables moveStateVariables)
    {
        this.moveStateVariables = moveStateVariables;
        canMove = true;
        isSidescrolling = false;
        lastMoveDirection = transform.forward;
    }

    public void StateUpdate()
    {                
        Vector2 controllerMoveDir = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
        bool isAnyAxisInput = controllerMoveDir.magnitude > moveStateVariables.axisThreshold;
        if (!isAnyAxisInput) {
            controllerMoveDir = Vector2.zero;
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

        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }        
    }

    public void StateFixedUpdate()
    {
        switch (moveStateVariables.velBody.GetMovementMode())
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


    #endregion

    #region Private Methods
    private void MovePrecise() {
        moveStateVariables.velBody.velocity = moveDirection * moveStateVariables.statsManager.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime;
        if (moveDirection.magnitude > moveStateVariables.axisThreshold)
        {
            if (moveStateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Running)
            {
                moveStateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Running);
            }
        }
        else
        {
            if (moveStateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
            {
                moveStateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
            }
        }
    }

    private void MoveIce() {
        if (moveStateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
        {
            moveStateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
        }

        moveStateVariables.velBody.AddDecayingForce(moveDirection * moveStateVariables.acceleration * Time.fixedDeltaTime);

        Vector3 flatMovement = new Vector3(moveStateVariables.velBody.velocity.x, 0f, moveStateVariables.velBody.velocity.z);
        currentSpeed = flatMovement.magnitude;

        if (currentSpeed > moveStateVariables.statsManager.GetStat(StatType.MoveSpeed))
        {
            Vector3 currentFlatVelocity = flatMovement;
            currentFlatVelocity = -currentFlatVelocity;
            currentFlatVelocity.Normalize();
            currentFlatVelocity *= (currentSpeed - moveStateVariables.statsManager.GetStat(StatType.MoveSpeed));
            currentFlatVelocity *= moveStateVariables.manualDrag;
            moveStateVariables.velBody.AddDecayingForce(currentFlatVelocity);
        }
    }

    private void HandleRotation(){
        if (moveStateVariables.currentEnemy != null){
            moveStateVariables.velBody.LookAt(moveStateVariables.currentEnemy.transform);
        }
        else{
            transform.forward = lastMoveDirection;
        }
    }
    #endregion

    #region Public Methods
    public void SetSidescrolling(bool mode)
    {
        isSidescrolling = mode;
    }

    #endregion
}