using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class ZombieAttackState : AIState
{
    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    float timeSinceLastHit = 0.0f;
    float hitRate;
    List<int> attacks;

    public override void OnEnter()
    {
        attacks = new List<int>();
        navAgent.enabled = false;
        navObstacle.enabled = true;
        hitRate = properties.hitRate;
        attacks.Add((int)AnimationStates.Attack1);
        attacks.Add((int)AnimationStates.Attack2);
        int animationAttackValue = attacks[Random.Range(0, attacks.Count)];
        animator.SetInteger(Strings.ANIMATIONSTATE, animationAttackValue);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
    }
    public override void Update()
    {
      
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
        attacks.Clear();
    }


    public void SetShooterProperties(ShooterProperties shooterProperties)
    {
        this.shooterProperties = shooterProperties;
    }

    public void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(shooterProperties.damage, DamagerType.BlasterBullet, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    public bool CanRestart()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >0.99f;
    }

}
