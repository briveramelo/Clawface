using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : MonoBehaviour, IPlayerState
{
    #region Private Fields
    PlayerStateManager.StateVariables moveStateVariables;
    Vector3 currentEnemyVector;
    private float sphereRadius = 0.1f;
    private Vector3 movement;
    private List<Vector3> externalForcesToAdd;
    private bool isAnyAxisInput;
    private bool isSidescrolling;
    private bool canMove;
    private Vector3 velocity;
    private Vector3 lastMovement;
    private float maxSpeed;
    private float currentSpeed;
    #endregion

    #region Public Methods    
    public void Init(ref PlayerStateManager.StateVariables moveStateVariables)
    {
        this.moveStateVariables = moveStateVariables;
        canMove = true;
        isSidescrolling = false;
        externalForcesToAdd = new List<Vector3>();
    }

    public void StateUpdate()
    {        
        if (moveStateVariables.currentEnemy != null)
        {
            currentEnemyVector = moveStateVariables.currentEnemy.transform.position;
        }
        Vector2 move = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
        isAnyAxisInput = move.magnitude > moveStateVariables.axisThreshold;
        if (!isAnyAxisInput) {
            move = Vector2.zero;
        }

        Vector2 moveModified = new Vector2(move.x, move.y);

        if (isSidescrolling)
        {
            moveModified.y = 0F;
        }

        movement = new Vector3(moveModified.x, 0.0F, moveModified.y);

        if (!canMove)
        {
            movement = Vector3.zero;
        }

        velocity = moveStateVariables.velBody.velocity;

        movement = Camera.main.transform.TransformDirection(movement);

        movement.y = 0f;

        movement = movement.normalized;

        if (movement != Vector3.zero)
        {
            lastMovement = movement;
        }

        maxSpeed = moveStateVariables.statsManager.GetStat(StatType.MoveSpeed);
    }

    public void StateFixedUpdate()
    {
        switch (moveStateVariables.velBody.GetMovementMode())
        {
            case MovementMode.PRECISE:
                moveStateVariables.velBody.velocity = movement * moveStateVariables.statsManager.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime;
                if (movement.magnitude > moveStateVariables.axisThreshold)
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
                break;
            case MovementMode.ICE:

                if (moveStateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)PlayerAnimationStates.Idle)
                {
                    moveStateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.Idle);
                }

                moveStateVariables.velBody.rigbod.AddForce(movement * moveStateVariables.acceleration * Time.fixedDeltaTime);

                foreach (Vector3 vector in externalForcesToAdd)
                {
                    moveStateVariables.velBody.rigbod.AddForce(vector * Time.fixedDeltaTime);
                }


                Vector3 flatMovement = new Vector3(moveStateVariables.velBody.velocity.x, 0f, moveStateVariables.velBody.velocity.z);
                currentSpeed = flatMovement.magnitude;

                if (currentSpeed > moveStateVariables.statsManager.GetStat(StatType.MoveSpeed))
                {
                    Vector3 currentFlatVelocity = flatMovement;
                    currentFlatVelocity = -currentFlatVelocity;
                    currentFlatVelocity = currentFlatVelocity.normalized;
                    currentFlatVelocity *= (currentSpeed - moveStateVariables.statsManager.GetStat(StatType.MoveSpeed));
                    currentFlatVelocity *= moveStateVariables.manualDrag;
                    moveStateVariables.velBody.rigbod.AddForce(currentFlatVelocity);
                }
                externalForcesToAdd.Clear();

                break;
            default:
                moveStateVariables.velBody.velocity = movement * moveStateVariables.statsManager.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime;
                break;
        }

        if (!isAnyAxisInput)
        {
            if (moveStateVariables.currentEnemy != null)
            {
                transform.LookAt(currentEnemyVector, Vector3.up);
                Quaternion rotation = transform.rotation;
                rotation.x = 0;
                rotation.z = 0;
                transform.rotation = rotation;
            }
            else if (lastMovement != Vector3.zero)
            {
                if (lastMovement != Vector3.zero)
                {
                    transform.forward = lastMovement;
                }
            }
        }
        else if (movement != Vector3.zero)
        {
            if (moveStateVariables.currentEnemy != null)
            {
                transform.LookAt(currentEnemyVector, Vector3.up);
                Quaternion rotation = transform.rotation;
                rotation.x = 0;
                rotation.z = 0;
                transform.rotation = rotation;
            }
            else
            {
                transform.forward = movement;
            }
        }
        velocity = moveStateVariables.velBody.velocity;
        currentSpeed = moveStateVariables.velBody.velocity.magnitude;
    }


    #endregion

    #region Private Methods
    
    #endregion

    #region Public Methods
    public void AddExternalForce(Vector3 forceVector, float decay = 0.1f)
    {
        switch (moveStateVariables.velBody.GetMovementMode())
        {
            case MovementMode.PRECISE:
            default:

                if (canMove)
                {
                    moveStateVariables.velBody.AddDecayingForce(forceVector, decay);
                }
                break;
            case MovementMode.ICE:
                if (canMove)
                {
                    externalForcesToAdd.Add(forceVector * moveStateVariables.iceForceMultiplier);
                }
                break;
        }
    }
    public void SetSidescrolling(bool mode)
    {
        isSidescrolling = mode;
    }

    #endregion
}