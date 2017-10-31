using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class ZombieAttackState : AIState
{

    bool isPastStartup;
    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    float timeSinceLastHit;
    float hitRate;


    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        hitRate = properties.hitRate;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Idle);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
    }
    public override void Update()
    {
        velBody.LookAt(controller.AttackTarget);
        navAgent.velocity = Vector3.zero;

        timeSinceLastHit += Time.deltaTime;

        if(timeSinceLastHit > hitRate)
        {
            Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
            timeSinceLastHit = 0.0f;
        }
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }


    public void SetShooterProperties(ShooterProperties shooterProperties)
    {
        this.shooterProperties = shooterProperties;
    }


    private void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(shooterProperties.damage, DamagerType.BlasterBullet, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

}
