using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

[System.Serializable]
public class ZombieAttackState : AIState
{
    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    List<int> attacks;
    public bool moveTowardsPlayer;
    public bool doneAttacking;

    public override void OnEnter()
    {
        doneAttacking = false;
        attacks = new List<int>();
        navAgent.enabled = false;
        navObstacle.enabled = true;
        attacks.Add((int)AnimationStates.Attack1);
        attacks.Add((int)AnimationStates.Attack2);
        attacks.Add((int)AnimationStates.Attack3);
        attacks.Add((int)AnimationStates.Attack4);
        attacks.Add((int)AnimationStates.Attack5);
        attacks.Add((int)AnimationStates.Attack6);
        int animationAttackValue = attacks[Random.Range(0, attacks.Count)];
        moveTowardsPlayer = false;
        animator.SetInteger(Strings.ANIMATIONSTATE, animationAttackValue);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
    }
    public override void Update()
    {
        WaitToMove();
        if(navAgent.isActiveAndEnabled)
        navAgent.SetDestination(controller.AttackTarget.position);

        Vector3 lookPos = new Vector3(controller.AttackTarget.transform.position.x,0.0f, controller.AttackTarget.transform.position.z);
        controller.transform.LookAt(lookPos);
        //controller.transform.RotateAround(controller.transform.position, controller.transform.up,-25.0f);
    }
    public override void OnExit()
    {
        navAgent.speed = myStats.moveSpeed;
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
        return doneAttacking;
    }

    private bool WaitToMove()
    {
        if (moveTowardsPlayer)
        {
            navObstacle.enabled = false;
            navAgent.enabled = true;
            navAgent.speed = navAgent.speed * 1.25f;
            return true;
        }
        return false;
    }


}
