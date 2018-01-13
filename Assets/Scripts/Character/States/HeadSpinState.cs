using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSpinState : IPlayerState
{
    #region Private variables
    private bool isClawExtended;
    private float localTime;
    private float currentRotation;
    #endregion

    #region Serialized fields
    [SerializeField]
    private ClawArmController clawArmController;
    #endregion

    #region Unity lifecycle
    private void Start()
    {
        isClawExtended = false;
        localTime = 0.0f;
        currentRotation = 0.0f;
    }
    #endregion

    #region Public method
    public override void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateLateUpdate()
    {
        if (!stateVariables.stateFinished)
        {
            currentRotation += Time.deltaTime * stateVariables.headSpinSpeed;         
            stateVariables.modelHead.transform.rotation = Quaternion.AngleAxis(currentRotation, Vector3.up);
        }
    }

    public override void StateUpdate()
    {
        if (!isClawExtended)
        {
            stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.OpenFace);
            clawArmController.ExtendClawToRadius(stateVariables.headSpinClawRadius);
            isClawExtended = true;
        }
        ClearProjectiles();
        localTime += Time.deltaTime;
        if (localTime > stateVariables.headSpinDuration)
        {
            ResetState();
        }
    }
    #endregion

    #region Private Methods
    protected override void ResetState()
    {
        stateVariables.animator.SetInteger(Strings.ANIMATIONSTATE, (int)PlayerAnimationStates.CloseFace);
        clawArmController.ResetClawArm();
        isClawExtended = false;
        localTime = 0.0f;
        currentRotation = 0.0f;
        stateVariables.stateFinished = true;
    }

    private void ClearProjectiles()
    {

        Collider[] colliders = Physics.OverlapSphere(stateVariables.playerTransform.position, stateVariables.headSpinClawRadius, LayerMask.GetMask(Strings.Layers.ENEMY_PROJECTILE));
        foreach(Collider collider in colliders)
        {
            BlasterBullet bullet = collider.GetComponent<BlasterBullet>();
            if (bullet)
            {
                bullet.DestroyBullet();
            }
        }
    }
    #endregion
}
