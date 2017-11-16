using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

[System.Serializable]
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
        attacks.Add((int)AnimationStates.Attack3);
        attacks.Add((int)AnimationStates.Attack4);
        attacks.Add((int)AnimationStates.Attack5);
        attacks.Add((int)AnimationStates.Attack6);
        int animationAttackValue = attacks[Random.Range(0, attacks.Count)];
        animator.SetInteger(Strings.ANIMATIONSTATE, animationAttackValue);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
    }
    public override void Update()
    {
        controller.transform.LookAt(controller.AttackTarget);
        controller.transform.RotateAround(controller.transform.position, controller.transform.up,-25.0f);
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
