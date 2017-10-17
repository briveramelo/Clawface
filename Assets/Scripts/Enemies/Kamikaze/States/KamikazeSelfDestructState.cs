using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class KamikazeSelfDestructState : KamikazeState {

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    private float waitTimeToDestruct;
    private float blastRadius;


    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
        waitTimeToDestruct = properties.selfDestructTime;
        blastRadius = properties.blastRadius;
        Timing.RunCoroutine(RunStartupTimer(),coroutineName);
    }
    public override void Update()
    {
       controller.transform.LookAt(controller.AttackTarget);
       navAgent.velocity = Vector3.zero;
    }
    public override void OnExit()
    {

    }

    IEnumerator<float> RunStartupTimer()
    {
        yield return Timing.WaitForSeconds(waitTimeToDestruct);

        if (Vector3.Distance(controller.transform.position, controller.AttackTarget.transform.position) <= blastRadius)
        {
            //Set Damage to the player
            Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
        }
        //Set Damage to self(Kamikaze)
        DamageSelf(controller.gameObject.GetComponent<IDamageable>());
    }


    public void SetShooterProperties(ShooterProperties shooterProperties)
    {
        this.shooterProperties = shooterProperties;
    }


    private void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(shooterProperties.damage, DamagerType.Kamikaze, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    private void DamageSelf(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(myStats.health, DamagerType.BlasterBullet, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

}
