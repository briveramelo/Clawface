using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : MonoBehaviour, IPlayerState
{
    #region Private Fields
    PlayerStateManager.StateVariables moveStateVariables;
    Vector3 currentEnemyVector;
    private bool isGrounded;
    private bool isFalling = false;
    private float sphereRadius = 0.1f;
    private Vector3 movement;
    private List<Vector3> externalForces;
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
        externalForces = new List<Vector3>();
        externalForcesToAdd = new List<Vector3>();
        for (int i = 0; i < 100; i++)
        {
            externalForces.Add(Vector3.zero);
        }
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

        velocity = moveStateVariables.rb.velocity;

        movement = Camera.main.transform.TransformDirection(movement);

        movement.y = 0f;

        movement = movement.normalized;

        if (movement != Vector3.zero)
        {
            lastMovement = movement;
        }

        isGrounded = IsGrounded();
        maxSpeed = moveStateVariables.statsManager.GetStat(StatType.MoveSpeed);
    }

    public void StateFixedUpdate()
    {
        switch (moveStateVariables.movementMode)
        {
            case MovementMode.PRECISE:
                moveStateVariables.rb.velocity = movement * moveStateVariables.statsManager.GetStat(StatType.MoveSpeed) + GetExternalForceSum();
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

                moveStateVariables.rb.AddForce(movement * moveStateVariables.acceleration * Time.fixedDeltaTime);

                foreach (Vector3 vector in externalForcesToAdd)
                {
                    moveStateVariables.rb.AddForce(vector * Time.fixedDeltaTime);
                }


                Vector3 flatMovement = new Vector3(moveStateVariables.rb.velocity.x, 0f, moveStateVariables.rb.velocity.z);
                currentSpeed = flatMovement.magnitude;

                if (currentSpeed > moveStateVariables.statsManager.GetStat(StatType.MoveSpeed))
                {
                    Vector3 currentFlatVelocity = flatMovement;
                    currentFlatVelocity = -currentFlatVelocity;
                    currentFlatVelocity = currentFlatVelocity.normalized;
                    currentFlatVelocity *= (currentSpeed - moveStateVariables.statsManager.GetStat(StatType.MoveSpeed));
                    currentFlatVelocity *= moveStateVariables.manualDrag;
                    moveStateVariables.rb.AddForce(currentFlatVelocity);
                }
                externalForcesToAdd.Clear();

                break;
            default:
                moveStateVariables.rb.velocity = movement * moveStateVariables.statsManager.GetStat(StatType.MoveSpeed) + GetExternalForceSum();
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
        velocity = moveStateVariables.rb.velocity;
        currentSpeed = moveStateVariables.rb.velocity.magnitude;
    }
    #endregion

    #region Private Methods
    private bool IsGrounded()
    {

        Collider[] cols = Physics.OverlapSphere(moveStateVariables.foot.transform.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer == (int)Layers.Ground)
            {
                return true;
            }
        }
        if (!isFalling)
        {
            StartCoroutine(ApplyGravity());
        }
        return false;
    }

    private IEnumerator ApplyGravity()
    {
        isFalling = true;
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);
        float timeElapsed = 0f;
        float gravity = 9.81f;
        while (!isGrounded && isFalling)
        {
            externalForces[currentIndex] = Vector3.down * (gravity * timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isFalling = false;
        externalForces[currentIndex] = Vector3.zero;
    }

    private Vector3 GetExternalForceSum()
    {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }
    private IEnumerator AddPsuedoForce(Vector3 forceVector, float decay)
    {
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);

        externalForces[currentIndex] = forceVector;
        while (externalForces[currentIndex].magnitude > .2f)
        {
            externalForces[currentIndex] = Vector3.Lerp(externalForces[currentIndex], Vector3.zero, decay);
            yield return null;
        }
        externalForces[currentIndex] = Vector3.zero;
    }
    #endregion

    #region Public Methods
    public void AddExternalForce(Vector3 forceVector, float decay = 0.1f)
    {
        switch (moveStateVariables.movementMode)
        {
            case MovementMode.PRECISE:
            default:

                if (canMove)
                {
                    StartCoroutine(AddPsuedoForce(forceVector, decay));
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

    public void SetMovementMode(MovementMode mode)
    {
        moveStateVariables.movementMode = mode;
        moveStateVariables.rb.useGravity = mode == MovementMode.ICE ? true : false;
    }
    
    #endregion
}