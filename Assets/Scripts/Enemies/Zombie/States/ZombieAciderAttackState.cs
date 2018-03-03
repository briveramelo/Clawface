using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

[System.Serializable]
public class ZombieAciderAttackState : AIState
{
    public bool moveTowardsPlayer;
    public bool doneAttacking;
    public float animatorSpeed;

    public ColliderGenerator colliderGenerator;

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    List<int> attacks;
    private Vector3 initialPosition;
    private float oldAnimatorSpeed;

    public override void OnEnter()
    {
        initialPosition = controller.transform.position;
        doneAttacking = false;
        oldAnimatorSpeed = animator.speed;
        animator.speed = animatorSpeed;
        colliderGenerator.enabled = false;
        attacks = new List<int>();
        //attacks.Add((int)AnimationStates.Attack1);
        //attacks.Add((int)AnimationStates.Attack2);
        //attacks.Add((int)AnimationStates.Attack3);
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
        FreezePosition();
        Vector3 lookPos = new Vector3(controller.AttackTarget.transform.position.x, controller.transform.position.y, controller.AttackTarget.transform.position.z);
        controller.transform.LookAt(lookPos);
    }
    public override void OnExit()
    {
        navAgent.speed = myStats.moveSpeed;
        navObstacle.enabled = false;
        navAgent.enabled = true;
        animator.speed = oldAnimatorSpeed;
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
            damager.Set(myStats.attack, DamagerType.BlasterBullet, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    public bool CanRestart()
    {
        return doneAttacking;
    }

    private void FreezePosition()
    {
        controller.transform.position = initialPosition;
    }




}
