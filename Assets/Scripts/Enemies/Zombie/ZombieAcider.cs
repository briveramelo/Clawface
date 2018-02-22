using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using MovementEffects;

[System.Serializable]
public class ZombieAciderProperties : AIProperties
{
}

public class ZombieAcider : EnemyBase
{
    #region 1. Serialized Unity Inspector Fields
    [SerializeField] private ZombieAciderProperties properties;
    [SerializeField] private TentacleTrigger tentacle;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private ColliderGenerator colliderGenerator;
    #endregion

    #region 2. Private fields

    //The AI States of the Zombie
    private ZombieAciderChaseState chase;
    private ZombieAttackState attack;
    private ZombieStunState stun;
    private ZombieCelebrateState celebrate;
    private ZombieGetUpState getUp;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;
    private float closeEnoughToAttackDistance;

    #endregion

    #region 3. Unity Lifecycle


    public override void Awake()
    {
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();
        trailRenderer.Clear();
        trailRenderer.enabled = false;
        colliderGenerator.enabled = false;
        chase.trailRenderer = trailRenderer;
        chase.colliderGenerator = colliderGenerator;
        chase.needToClearTrail = true;
        attack.animatorSpeed = EnemyStatsManager.Instance.zombieAciderStats.animationAttackSpeed;
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

    public void GetUpDone()
    {
        getUp.Up();
    }

    public void DamageAttackTarget()
    {
        attack.Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
    }

    public override void OnDeath()
    {
        trailRenderer.Clear();
        trailRenderer.enabled = false;
        colliderGenerator.enabled = false;
        chase.needToClearTrail = true;
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        trailRenderer.Clear();
        chase.needToClearTrail = true;
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
        chase = new ZombieAciderChaseState();
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
        myStats.health = EnemyStatsManager.Instance.zombieAciderStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.zombieAciderStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.zombieAciderStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.zombieAciderStats.speed;
        myStats.attack = EnemyStatsManager.Instance.zombieAciderStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.zombieAciderStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.zombieAciderStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.zombieAciderStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.zombieAciderStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.zombieAciderStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.zombieAciderStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.zombieAciderStats.stunnedTime;

        closeEnoughToAttackDistance = EnemyStatsManager.Instance.zombieAciderStats.stunnedTime;
        

        trailRenderer.time = EnemyStatsManager.Instance.zombieAciderStats.trailRendererTime;
        trailRenderer.startWidth = EnemyStatsManager.Instance.zombieAciderStats.trailRendererWidth;

        colliderGenerator.SetStats(EnemyStatsManager.Instance.zombieAciderStats.colliderGenerationTime, EnemyStatsManager.Instance.zombieAciderStats.acidTriggerLife);

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
