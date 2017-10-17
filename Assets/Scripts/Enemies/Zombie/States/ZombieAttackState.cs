﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class ZombieAttackState : ZombieState
{

    bool isPastStartup;
    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    float timeSinceLastHit;
    float hitRate;


    public override void OnEnter()
    {
        hitRate = properties.hitRate;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
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