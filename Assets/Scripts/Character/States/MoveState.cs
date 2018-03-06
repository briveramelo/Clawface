using UnityEngine;

public class MoveState : IPlayerState
{
    #region Private Fields
    private float sphereRadius = 0.1f;
    private Vector3 moveDirection;
    private bool canMove;
    private Vector3 lastMoveDirection;
    private float currentSpeed;
    private Vector3 lastLookDirection;
    private bool playerDead;
    #endregion

    #region Unity lifecycle
    private void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, PlayerDead);
    }

    private void OnDisable()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, PlayerDead);
        }
    }
    #endregion

    #region Public Methods    
    public override void Init(ref PlayerStateManager.StateVariables moveStateVariables)
    {
        stateVariables = moveStateVariables;
        canMove = true;
        lastMoveDirection = moveStateVariables.playerTransform.forward;
    }

    public override void StateUpdate()
    {
        if (!playerDead)
        {
            Vector2 controllerMoveDir = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
            Vector2 lookDir = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);
            bool isAnyAxisInput = controllerMoveDir.magnitude > stateVariables.axisThreshold;
            if (!isAnyAxisInput)
            {
                controllerMoveDir = Vector2.zero;
            }
            if (lookDir.magnitude > stateVariables.axisThreshold)
            {
                lastLookDirection = new Vector3(lookDir.x, 0, lookDir.y);
            }
            else
            {
                lastLookDirection = Vector3.zero;
            }
            Vector2 moveModified = new Vector2(controllerMoveDir.x, controllerMoveDir.y);

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
            }
            else if (lastLookDirection != Vector3.zero)
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
            PlayerAnimationStates movementAnimState = PlayerAnimationStates.RunningForward;
            float moveLookDot = Vector3.Dot (moveDirection, stateVariables.velBody.GetForward());
            if (moveLookDot > 0.5f) movementAnimState = PlayerAnimationStates.RunningForward;
            else if (moveLookDot > -0.5f)
            {
                if (Vector3.Cross (moveDirection, stateVariables.velBody.GetForward()).y > 0f)
                    movementAnimState = PlayerAnimationStates.SideStrafeRight;
                else movementAnimState = PlayerAnimationStates.SideStrafeLeft;
            }
            else movementAnimState = PlayerAnimationStates.RunningBackward;

            if (stateVariables.animator.GetInteger(Strings.ANIMATIONSTATE) != (int)movementAnimState)
            {
                stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)movementAnimState);
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
        if (SettingsManager.Instance.SnapLook)
        {
            if (lastLookDirection != Vector3.zero)
            {
                stateVariables.playerTransform.forward = lastLookDirection;
            }
            else if (moveDirection != Vector3.zero)
            {
                stateVariables.playerTransform.forward = moveDirection;
            }
        }
        else
        {
            if (lastLookDirection != Vector3.zero)
            {
                stateVariables.playerTransform.forward = lastLookDirection;
            }
        }
    }

    protected override void ResetState()
    {
    }

    private void PlayerDead(params object[] parameters)
    {
        playerDead = true;
        stateVariables.velBody.velocity = Vector3.zero;
    }
    #endregion

    #region Public Methods

    public PlayerStateManager.StateVariables GetStateVariables()
    {
        return stateVariables;
    }
    #endregion
}