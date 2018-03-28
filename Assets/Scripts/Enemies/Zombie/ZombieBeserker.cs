using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using MEC;

[System.Serializable]
public class ZombieBeserkerProperties : AIProperties
{
}

public class ZombieBeserker : EnemyBase
{
    #region 1. Serialized Unity Inspector Fields
    [SerializeField] private ZombieProperties properties;
    [SerializeField] private TentacleTrigger tentacle;
    #endregion

    #region 2. Private fields

    //The AI States of the Zombie
    private ZombieBeserkerChaseState chase;
    private ZombieAttackState attack;
    private ZombieStunState stun;
    private ZombieCelebrateState celebrate;
    private ZombieGetUpState getUp;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;
    private float closeEnoughToAttackDistance;

    #endregion

    #region 3. Unity Lifecycle

    protected override void Awake()
    {
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();
        attack.animatorSpeed = EnemyStatsManager.Instance.zombieBeserkerStats.animationAttackSpeed;
        controller.Initialize(properties,velBody, animator, myStats, navAgent, navObstacle,aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckPlayerDead,
            CheckIfStunned,
            CheckToAttack,
            CheckToFinishAttacking

        };
        base.Awake();
    }

    #endregion

    #region 4. Public Methods   

    //State conditions
    bool CheckPlayerDead()
    {
        if (AIManager.Instance.GetPlayerDead())
        {
            if (myStats.health > myStats.skinnableHealth && !celebrate.isCelebrating())
            {
                animator.SetLayerWeight(2, 0.0f);
                controller.CurrentState = celebrate;
                controller.UpdateState(EAIState.Celebrate);
            }
            return true;
        }
        return false;
    }

    bool CheckToAttack()
    {
        if (controller.CurrentState == chase && controller.DistanceFromTarget < closeEnoughToAttackDistance)
        {
            if (chase.DoneAttacking())
            {
                animator.SetLayerWeight(2, 0.0f);
                animator.SetTrigger("Attack");
                controller.UpdateState(EAIState.Attack);
            }
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
                animator.SetLayerWeight(2, 1.0f);
                controller.UpdateState(EAIState.Chase);
            }
            else
            {
                animator.SetLayerWeight(2, 0.0f);
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
            animator.SetLayerWeight(2, 0.0f);
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

    public void GetUpDone()
    {
        getUp.Up();
    }

    public void FinishedAttack()
    {
        chase.RandomAttack();
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
    }

    public override Vector3 ReCalculateTargetPosition()
    {
        return Vector3.zero;
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
        chase = new ZombieBeserkerChaseState();
        chase.stateName = "chase";
        attack = new ZombieAttackState();
        attack.stateName = "attack";
        stun = new ZombieStunState();
        stun.stateName = "stun";
        celebrate = new ZombieCelebrateState();
        celebrate.stateName = "celebrate";
        getUp = new ZombieGetUpState();
        getUp.stateName = "getUp";
        aiStates.Add(chase);
        aiStates.Add(attack);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
        aiStates.Add(getUp);
    }

    private void SetAllStats()
    {
        myStats.health = EnemyStatsManager.Instance.zombieBeserkerStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.zombieBeserkerStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.zombieBeserkerStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.zombieBeserkerStats.speed;
        myStats.attack = EnemyStatsManager.Instance.zombieBeserkerStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.zombieBeserkerStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.zombieBeserkerStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.zombieBeserkerStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.zombieBeserkerStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.zombieBeserkerStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.zombieBeserkerStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.zombieBeserkerStats.stunnedTime;

        closeEnoughToAttackDistance = EnemyStatsManager.Instance.zombieBeserkerStats.stunnedTime;
        
        myStats.SetStats();
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
