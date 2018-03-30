﻿//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using ModMan;
using MEC;

[System.Serializable]
public class MallCopProperties : AIProperties
{
}

public class MallCop : EnemyBase
{

    #region 1. Serialized Unity Inspector Fields
    [SerializeField] SphereCollider playerDetectorSphereCollider;
    [SerializeField] private MallCopProperties properties;
    [SerializeField] private Mod mod;

    #endregion

    #region 2. Private fields
    //The AI States of the Mall Cop
    private MallCopChaseState chase;
    private MallCopFireState fire;
    private MallCopStunState stun;
    private MallCopCelebrateState celebrate;
    private MallCopGetUpState getUp;
    private float currentToleranceTime;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;
    private float closeEnoughToFireDistance;
    private float maxToleranceTime;
    private Vector3 rayCastPosition;
    private bool isUp;
    #endregion

    #region 3. Unity Lifecycle

    public override void Awake()
    {
        isUp = false;
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();
        fire.animatorSpeed = EnemyStatsManager.Instance.blasterStats.animationShootSpeed;
        controller.Initialize(properties, mod, velBody, animator, myStats, navAgent, navObstacle, aiStates);
        mod.setModSpot(ModSpot.ArmR);
        mod.AttachAffect(ref myStats, velBody);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);

        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckDoneGettingUp,
            CheckPlayerDead,
            CheckToFire,
            CheckToFinishFiring,
            CheckIfStunned
        };

        mod.damage = myStats.attack;
        currentToleranceTime = 0.0f;
        base.Awake();
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
        Vector3 fwd = controller.DirectionToTarget;
        rayCastPosition = new Vector3(controller.transform.position.x, controller.transform.position.y + 1f, controller.transform.position.z);
        RaycastHit hit;

        if ((controller.CurrentState == chase && controller.DistanceFromTarget <= closeEnoughToFireDistance))
        {
            if (Physics.Raycast(rayCastPosition, fwd, out hit, 50, LayerMask.GetMask(Strings.Layers.MODMAN, Strings.Layers.OBSTACLE)))
            {
                if (hit.transform.tag == Strings.Tags.PLAYER)
                    controller.UpdateState(EAIState.Fire);
            }
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
        }

        if (controller.CurrentState == fire && fire.DoneFiring())
            {
                if (controller.DistanceFromTarget > closeEnoughToFireDistance)
                {
                    ToleranceTimeToExit();
                }
                else if (controller.DistanceFromTarget < closeEnoughToFireDistance)
                {
                    ToleranceTimeToExit();
                    currentToleranceTime = 0.0f;
                }
            else
            {
                Vector3 fwd = controller.DirectionToTarget;
                rayCastPosition = new Vector3(controller.transform.position.x, controller.transform.position.y + 1f, controller.transform.position.z);
                RaycastHit hit;

                if (Physics.Raycast(rayCastPosition, fwd, out hit, 50, LayerMask.GetMask(Strings.Layers.MODMAN, Strings.Layers.OBSTACLE)))
                {
                    if (hit.transform.tag != Strings.Tags.PLAYER)
                    {
                        fire.StartEndFire();
                    }
                }

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

    float maxDistanceBeforeChasing { get { return playerDetectorSphereCollider.radius * playerDetectorSphereCollider.transform.localScale.x * transform.localScale.x; } } //assumes parenting scheme...


    public override void OnDeath()
    {
        isUp = false;
        mod.KillCoroutines();
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        mod.setModSpot(ModSpot.ArmR);
        base.ResetForRebirth();
    }

    public void ReadyToFire()
    {
        fire.ReadyToFireDone();
    }

    public void EndFireDone()
    {
        fire.EndFireDone();
    }

    public void StartAiming()
    {
        animator.SetLayerWeight(2, 1.0f);
    }

    public void StopAiming()
    {
        fire.StopAiming();
        animator.SetLayerWeight(2, 0.0f);
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
            Timing.RunCoroutine(HitReactionLerp(), coroutineName);
        }
        base.DoHitReaction(damager);
    }
    #endregion

    #region 5. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new MallCopChaseState();
        chase.stateName = "chase";
        fire = new MallCopFireState();
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
        myStats.health = EnemyStatsManager.Instance.blasterStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.blasterStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.blasterStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.blasterStats.speed;
        myStats.attack = EnemyStatsManager.Instance.blasterStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.blasterStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.blasterStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.blasterStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.blasterStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.blasterStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.blasterStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.blasterStats.stunnedTime;

        closeEnoughToFireDistance = EnemyStatsManager.Instance.blasterStats.closeEnoughToFireDistance;
        maxToleranceTime = EnemyStatsManager.Instance.blasterStats.maxToleranceTime;

        if (mod.GetComponent<BlasterMod>())
        {
            mod.GetComponent<BlasterMod>().SetBulletStats(EnemyStatsManager.Instance.blasterStats.bulletLiveTime, EnemyStatsManager.Instance.blasterStats.bulletSpeed, EnemyStatsManager.Instance.blasterStats.attack);
        }
        myStats.SetStats();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToFireDistance);
    }

    private void ToleranceTimeToExit()
    {
        currentToleranceTime += Time.deltaTime;

        if (currentToleranceTime >= maxToleranceTime)
        {
            if (controller.DistanceFromTarget < closeEnoughToFireDistance)
            {
                currentToleranceTime = 0.0f;
            }
            else
            {
                fire.StartEndFire();
            }
        }

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

    void ShowChargeEffect ()
    {
        GameObject vfx = ObjectPool.Instance.GetObject(PoolObjectType.VFXEnemyChargeBlaster);
        Vector3 scaleBackup = vfx.transform.localScale;
        vfx.transform.SetParent (mod.transform);
        //For offsetting the particle
        vfx.transform.localPosition = new Vector3(0.0f,0.2f,1.0f);
        vfx.transform.localRotation = Quaternion.identity;
        vfx.transform.localScale = new Vector3 (
            scaleBackup.x / vfx.transform.localScale.x,
            scaleBackup.y / vfx.transform.localScale.y,
            scaleBackup.z / vfx.transform.localScale.z
        );
    }

    #endregion

}

