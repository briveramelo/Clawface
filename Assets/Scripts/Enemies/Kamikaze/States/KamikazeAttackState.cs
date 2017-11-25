using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class KamikazeAttackState : AIState {

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    private float waitTimeToDestruct;
    private float blastRadius;
    public bool setToSelfDestruct = false;
    private bool attackDone = false;

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;

        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Idle);
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
        navObstacle.enabled = false;
        navAgent.enabled = true;
        setToSelfDestruct = true;
    }

    IEnumerator<float> RunStartupTimer()
    {
        yield return Timing.WaitForSeconds(waitTimeToDestruct);

        if (Vector3.Distance(controller.transform.position, controller.AttackTarget.transform.position) <= blastRadius)
        {
            //Set Damage to the player
            Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
            GameObject effect = ObjectPool.Instance.GetObject (PoolObjectType.VFXKamikazeExplosion);
            effect.transform.position = controller.transform.position;
            attackDone = true;
        }
        else
        {
            attackDone = true;
        }
    }


    public void SetShooterProperties(ShooterProperties shooterProperties)
    {
        this.shooterProperties = shooterProperties;
    }


    private void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(myStats.attack, DamagerType.Kamikaze, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    public bool DoneAttacking()
    {
        return attackDone;
    }


}
