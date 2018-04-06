//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using ModMan;
using MEC;

[System.Serializable]
public class MallCopReanimatorProperties : AIProperties
{
}

public class MallCopReanimator : EnemyBase
{

    #region 1. Serialized Unity Inspector Fields
    [SerializeField] SphereCollider enemyDetectorSphereCollider;
    [SerializeField] private MallCopProperties properties;
    [SerializeField] private Transform healParticleTransform;
    #endregion

    #region 2. Private fields
    //The AI States of the Mall Cop
    private MallCopReanimatorChaseState chase;
    private MallCopReanimatorFireState fire;
    private MallCopStunState stun;
    private MallCopCelebrateState celebrate;
    private MallCopGetUpState getUp;
    private float currentToleranceTime;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;
    private Vector3 rayCastPosition;
    private bool foundStunnedEnemy = false;
    private float closeEnoughToReanimateDistance;
    private bool isUp;
    #endregion

    #region 3. Unity Lifecycle

    protected override void Awake()
    {
        isUp = false;
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();
        fire.animatorSpeed = EnemyStatsManager.Instance.blasterReanimatorStats.animationShootSpeed;
        controller.Initialize(properties, velBody, animator, myStats, navAgent, navObstacle, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);

        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckDoneGettingUp,
            CheckPlayerDead,
            CheckToFire,
            CheckToFinishFiring,
            CheckIfStunned
        };
        currentToleranceTime = 0.0f;
        base.Awake();
    }

    private void OnTriggerStay(Collider other)
    {
        //Found an enemy
        if (other.gameObject.tag == Strings.Layers.ENEMY && other.GetInstanceID() != this.gameObject.GetInstanceID())
        {
            if (other.gameObject.GetComponent<AIController>().IsStunned() && !foundStunnedEnemy)
            {
                controller.AttackTarget = other.gameObject.transform;
                foundStunnedEnemy = true;
            }
        }
    }


    #endregion

    #region 4. Public Methods   

    //State conditions
    bool CheckDoneGettingUp()
    {
        if (!isUp)
        {
            return true;
        }
        return false;
    }


    bool CheckPlayerDead()
    {
        if (AIManager.Instance.GetPlayerDead())
        {
            if (myStats.health > myStats.skinnableHealth && !celebrate.isCelebrating())
            {
                fire.StopCoroutines();
                controller.CurrentState = celebrate;
                controller.UpdateState(EAIState.Celebrate);
            }
            return true;
        }
        return false;
    }

    bool CheckToFire()
    {
        if ((controller.CurrentState == chase && foundStunnedEnemy &&  controller.DistanceFromTarget <closeEnoughToReanimateDistance))
        {
            controller.UpdateState(EAIState.Fire);
            return true;
        }
        return false;
    }
    bool CheckToFinishFiring()
    {

        if (myStats.health <= myStats.skinnableHealth || alreadyStunned)
        {
            controller.CurrentState = stun;
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
            return true;
        }

        if (controller.CurrentState == fire && fire.DoneFiring())
        {
            
            fire.StartEndFire();
            
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

    float maxDistanceBeforeChasing { get { return enemyDetectorSphereCollider.radius * enemyDetectorSphereCollider.transform.localScale.x * transform.localScale.x; } } //assumes parenting scheme...


    public override void OnDeath()
    {
        isUp = false;
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        base.ResetForRebirth();
    }

    public void Heal()
    {
        fire.HealWounded();
    }

    public void ReadyToFire()
    {
        fire.ReadyToFireDone();
    }

    public void EndFireDone()
    {
        foundStunnedEnemy = false;
        fire.EndFireDone();
    }

    public void GetUpDone()
    {
        isUp = true;
        controller.CurrentState = chase;
        controller.UpdateState(EAIState.Chase);
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
            else if(hitAngle > 45.0f && hitAngle < 135.0f)
            {
                animator.SetTrigger("HitRight");
            }
            else if (hitAngle > 225.0f && hitAngle < 315.0f)
            {
                animator.SetTrigger("HitLeft");
            }

            currentHitReactionLayerWeight = 1.0f;
            animator.SetLayerWeight(3, currentHitReactionLayerWeight);
            Timing.RunCoroutine(HitReactionLerp(), CoroutineName);
        }
        base.DoHitReaction(damager);
    }
    #endregion

    #region 5. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new MallCopReanimatorChaseState();
        chase.stateName = "chase";
        fire = new MallCopReanimatorFireState();
        fire.stateName = "fire";
        stun = new MallCopStunState();
        stun.stateName = "stun";
        celebrate = new MallCopCelebrateState();
        celebrate.stateName = "celebrate";
        getUp = new MallCopGetUpState();
        getUp.stateName = "getUp";
        aiStates.Add(chase);
        aiStates.Add(fire);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
        aiStates.Add(getUp);
    }

    private void SetAllStats()
    {
        myStats.health = EnemyStatsManager.Instance.blasterReanimatorStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.blasterReanimatorStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.blasterReanimatorStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.blasterReanimatorStats.speed;
        myStats.attack = EnemyStatsManager.Instance.blasterReanimatorStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.blasterReanimatorStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.blasterReanimatorStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.blasterReanimatorStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.blasterReanimatorStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.blasterReanimatorStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.blasterReanimatorStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.blasterReanimatorStats.stunnedTime;

        closeEnoughToReanimateDistance = EnemyStatsManager.Instance.blasterReanimatorStats.closeEnoughToReanimateDistance;

        myStats.SetStats();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToReanimateDistance);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(playerDetectorSphereCollider.transform.position, maxDistanceBeforeChasing);
    }


    private IEnumerator<float> HitReactionLerp()
    {
        while (currentHitReactionLayerWeight > 0.0f)
        {
            currentHitReactionLayerWeight -= Time.deltaTime * hitReactionLayerDecrementSpeed;

            animator.SetLayerWeight(3, currentHitReactionLayerWeight);
            yield return 0.0f;
        }
        currentHitReactionLayerWeight = 0.0f;
        animator.SetLayerWeight(3, currentHitReactionLayerWeight);
    }

    void ShowChargeEffect()
    {
        GameObject vfx = ObjectPool.Instance.GetObject(PoolObjectType.VFXEnemyChargeBlaster);
        if (vfx) {
            Vector3 scaleBackup = vfx.transform.localScale;
            vfx.transform.SetParent(healParticleTransform);
            //For offsetting the particle
            vfx.transform.localPosition = Vector3.zero;
            vfx.transform.localRotation = Quaternion.identity;
            vfx.transform.localScale = new Vector3(
                scaleBackup.x / vfx.transform.localScale.x,
                scaleBackup.y / vfx.transform.localScale.y,
                scaleBackup.z / vfx.transform.localScale.z
            );
        }
    }
    #endregion

}

