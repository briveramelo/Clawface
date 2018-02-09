//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using ModMan;
using MovementEffects;

[System.Serializable]
public class MallCopReanimatorProperties : AIProperties
{
}

public class MallCopReanimator : EnemyBase
{

    #region 1. Serialized Unity Inspector Fields
    [SerializeField] SphereCollider enemyDetectorSphereCollider;
    [SerializeField] float closeEnoughToFireDistance;
    [SerializeField] private MallCopProperties properties;
    [SerializeField] private BulletHellPatternController bulletHellController;
    [SerializeField] private float maxToleranceTime;
    #endregion

    #region 2. Private fields
    //The AI States of the Mall Cop
    private MallCopReanimatorChaseState chase;
    private MallCopReanimatorFireState fire;
    private MallCopStunState stun;
    private MallCopCelebrateState celebrate;
    private float currentToleranceTime;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;
    private Vector3 rayCastPosition;
    private bool foundStunnedEnemy = false;
    #endregion

    #region 3. Unity Lifecycle

    public override void Awake()
    {
        InitilizeStates();
        controller.Initialize(properties, velBody, animator, myStats, navAgent, navObstacle, bulletHellController, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);

        controller.checksToUpdateState = new List<Func<bool>>() {
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
                foundStunnedEnemy = true;
                controller.AttackTarget = other.gameObject.transform;
            }
        }
    }


    #endregion

    #region 4. Public Methods   

    //State conditions
    bool CheckToFire()
    {
        if ((controller.CurrentState == chase && foundStunnedEnemy &&  controller.distanceFromTarget < closeEnoughToFireDistance/3.0f))
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
            foundStunnedEnemy = false;
            controller.AttackTarget = controller.FindPlayer();
            controller.UpdateState(EAIState.Chase);
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
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        base.ResetForRebirth();
    }

    public void Fire()
    {
        bulletHellController.FireBullet();
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
        chase = new MallCopReanimatorChaseState();
        chase.stateName = "chase";
        fire = new MallCopReanimatorFireState();
        fire.stateName = "fire";
        stun = new MallCopStunState();
        stun.stateName = "stun";
        celebrate = new MallCopCelebrateState();
        celebrate.stateName = "celebrate";
        aiStates.Add(chase);
        aiStates.Add(fire);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToFireDistance);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(playerDetectorSphereCollider.transform.position, maxDistanceBeforeChasing);
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


    #endregion

}

