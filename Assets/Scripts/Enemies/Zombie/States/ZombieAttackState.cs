using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class ZombieAttackState : ZombieState
{

    bool isPastStartup;
    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    float timeSinceLastHit;
    float hitRate = 0.25f;


    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
        Timing.RunCoroutine(RunStartupTimer());
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

    IEnumerator<float> RunStartupTimer()
    {
        isPastStartup = false;
        yield return Timing.WaitForSeconds(0.5f);
        isPastStartup = true;
    }

    public bool CanRestart()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f && isPastStartup;
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
