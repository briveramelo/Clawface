using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class KamikazeAttackState : AIState {

    private ShooterProperties shooterProperties = new ShooterProperties();
    private Damager damager = new Damager();
    private float waitTimeToDestruct;
    private float blastRadius;
    private float scaleRate;
    public bool setToSelfDestruct = false;
    private bool attackDone = false;

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        attackDone = false;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Attack1);
        shooterProperties.Initialize(2, 5, 6, 0);
        SetShooterProperties(shooterProperties);
        waitTimeToDestruct = properties.selfDestructTime;
        blastRadius = properties.blastRadius;
        scaleRate = properties.selfDestructTime;
        Timing.RunCoroutine(RunStartupTimer(),coroutineName);
    }
    public override void Update()
    {
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
        controller.transform.LookAt(lookAtPosition);
        navAgent.velocity = Vector3.zero;
    }
    public override void OnExit()
    {
        attackDone = false;
        navObstacle.enabled = false;
        navAgent.enabled = true;
        setToSelfDestruct = true;
    }

    IEnumerator<float> RunStartupTimer()
    {
        //GameObject explosionSphere = ObjectPool.Instance.GetObject(PoolObjectType.KamikazeExplosionSphere);
        GameObject explosionWarning = ObjectPool.Instance.GetObject(PoolObjectType.VFXKamikazeExplosionWarning);
        if (explosionWarning) {
            explosionWarning.transform.position = controller.transform.position;
            //explosionSphere.GetComponent<ExplosionTrigger>().Initialize(blastRadius,scaleRate);
        }

        yield return Timing.WaitForSeconds(waitTimeToDestruct);

        //Make sure the kamikaze is not stunned
        if (myStats.health <= myStats.skinnableHealth)
        {
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
        }
        else if (Vector3.Distance(controller.transform.position, controller.AttackTarget.transform.position) < blastRadius * 0.5f)
        {
            //Set Damage to the player
            Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
            GameObject effect = ObjectPool.Instance.GetObject(PoolObjectType.VFXKamikazeExplosion);
            if (effect) {
                effect.transform.position = controller.transform.position;
            }
            attackDone = true;
            setToSelfDestruct = true;
        }
        else
        {
            attackDone = true;
            setToSelfDestruct = true;
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
