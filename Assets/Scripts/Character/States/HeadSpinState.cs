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
        ClearProjectilesAndDamageEnemies();
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
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY), LayerMask.NameToLayer(Strings.Layers.MODMAN), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Strings.Layers.ENEMY_BODY), LayerMask.NameToLayer(Strings.Layers.MODMAN), false);
        stateVariables.stateFinished = true;
    }

    private void ClearProjectilesAndDamageEnemies()
    {
        string[] layers = { Strings.Layers.ENEMY_PROJECTILE, Strings.Layers.ENEMY };
        Collider[] colliders = Physics.OverlapSphere(stateVariables.playerTransform.position, stateVariables.headSpinClawRadius, LayerMask.GetMask(layers));
        foreach(Collider collider in colliders)
        {
            BlasterBullet bullet = collider.GetComponent<BlasterBullet>();
            if (bullet)
            {
                bullet.DestroyBullet();
            }
            else
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Damager damager = new Damager();
                    damager.damage = stateVariables.headSpinDamage;
                    damageable.TakeDamage(damager);
                }
            }
        }
    }
    #endregion
}
