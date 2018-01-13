﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using MovementEffects;

[System.Serializable]
public class ZombieProperties : AIProperties
{
    [HideInInspector] public float zombieHitRate;

    public void InitializeProperties()
    {
        hitRate = zombieHitRate;
    }
}

public class Zombie : EnemyBase
{
    #region 1. Serialized Unity Inspector Fields

    [SerializeField] float closeEnoughToAttackDistance;
    [SerializeField] float maxDistanceBeforeChasing = 2.0f;
    [SerializeField] private ZombieProperties properties;
    [SerializeField] private TentacleTrigger tentacle;
    #endregion

    #region 2. Private fields

    //The AI States of the Zombie
    private ZombieChaseState chase;
    private ZombieAttackState attack;
    private ZombieStunState stun;
    private ZombieCelebrateState celebrate;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;

    #endregion

    #region 3. Unity Lifecycle

    public override void Awake()
    {
        InitilizeStates();
        properties.InitializeProperties();
        controller.Initialize(properties,velBody, animator, myStats, navAgent, navObstacle,aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckToAttack,
            CheckToFinishAttacking,
            CheckIfStunned
        };
        base.Awake();
    }

    #endregion

    #region 4. Public Methods   

    //State conditions
    bool CheckToAttack()
    {
        if (controller.CurrentState == chase && controller.DistanceFromTarget < closeEnoughToAttackDistance)
        {
            animator.SetTrigger("Attack");
            controller.UpdateState(EAIState.Attack);
            return true;
        }
        return false;
    }
    bool CheckToFinishAttacking()
    {
        if (controller.CurrentState == attack && attack.CanRestart())
        {
            bool shouldChase = controller.DistanceFromTarget > closeEnoughToAttackDistance;

            if (shouldChase)
            {
                controller.UpdateState(EAIState.Chase);
            }
            else
            {
                animator.SetTrigger("Attack");
                controller.UpdateState(EAIState.Attack);
            }
            return true;
        }
        return false;
    }
    bool CheckIfStunned()
    {
        if (myStats.health <= myStats.skinnableHealth || alreadyStunned)
        {
            controller.CurrentState = stun;
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
            
            return true;
        }
        return false;
    }

    public void ActivateTentacleTrigger()
    {
        tentacle.ActivateTriggerDamage();
    }

    public void DeactivateTentacleTrigger()
    {
        tentacle.DeactivateTriggerDamage();
    }

    public void MoveTowardsPlayerInAttack()
    {
        attack.moveTowardsPlayer = true;
    }

    public void FinishedAttack()
    {
        attack.doneAttacking = true;
    }


    public void DamageAttackTarget()
    {
        attack.Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        base.ResetForRebirth();
    }

    public override void DoPlayerKilledState(object[] parameters)
    {
        if (myStats.health > myStats.skinnableHealth)
        {
            animator.SetTrigger("DoVictoryDance");
            controller.CurrentState = celebrate;
            controller.UpdateState(EAIState.Celebrate);
            animator.SetInteger("AnimationState", -1);
        }
    }

    public override void DoHitReaction(Damager damager)
    {
        if (myStats.health > myStats.skinnableHealth)
        {
            float hitAngle = Vector3.Angle(controller.transform.forward, damager.impactDirection);

            if (hitAngle < 45.0f || hitAngle > 315.0f)
            {
                animator.SetTrigger("HitFront");
            }
            else if (hitAngle > 45.0f && hitAngle < 135.0f)
            {
                animator.SetTrigger("HitRight");
            }
            else if (hitAngle > 225.0f && hitAngle < 315.0f)
            {
                animator.SetTrigger("HitLeft");
            }
            currentHitReactionLayerWeight = 1.0f;
            animator.SetLayerWeight(1, currentHitReactionLayerWeight);
            Timing.RunCoroutine(HitReactionLerp(), coroutineName);
        }
        base.DoHitReaction(damager);
    }

    #endregion

    #region 5. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new ZombieChaseState();
        chase.stateName = "chase";
        attack = new ZombieAttackState();
        attack.stateName = "attack";
        stun = new ZombieStunState();
        stun.stateName = "stun";
        celebrate = new ZombieCelebrateState();
        celebrate.stateName = "celebrate";
        aiStates.Add(chase);
        aiStates.Add(attack);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }

    private IEnumerator<float> HitReactionLerp()
    {
        while (currentHitReactionLayerWeight > 0.0f)
        {
            currentHitReactionLayerWeight -= Time.deltaTime * hitReactionLayerDecrementSpeed;

            animator.SetLayerWeight(1, currentHitReactionLayerWeight);
            yield return 0.0f;
        }
        currentHitReactionLayerWeight = 0.0f;
        animator.SetLayerWeight(1, currentHitReactionLayerWeight);
    }


    #endregion


}
