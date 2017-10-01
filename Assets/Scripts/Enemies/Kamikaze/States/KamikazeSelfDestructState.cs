using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class KamikazeSelfDestructState : KamikazeState {

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();


    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
    }
    public override void Update()
    {
       controller.transform.LookAt(controller.AttackTarget);
       navAgent.velocity = Vector3.zero;

       //Set Damage to self(Kamikaze)
       DamageSelf(controller.gameObject.GetComponent<IDamageable>());

       //Set Damage to the player
       Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());


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
