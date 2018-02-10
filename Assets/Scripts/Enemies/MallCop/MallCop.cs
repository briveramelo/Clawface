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
public class MallCopProperties : AIProperties
{
}

public class MallCop : EnemyBase
{

    #region 1. Serialized Unity Inspector Fields
    [SerializeField] SphereCollider playerDetectorSphereCollider;
    [SerializeField] float closeEnoughToFireDistance;
    [SerializeField] private MallCopProperties properties;
    [SerializeField] private Mod mod;
    [SerializeField] private float maxToleranceTime;
    #endregion

    #region 2. Private fields
    //The AI States of the Mall Cop
    private MallCopChaseState chase;
    private MallCopFireState fire;
    private MallCopStunState stun;
    private MallCopCelebrateState celebrate;
    private float currentToleranceTime;
    private float currentHitReactionLayerWeight;
    private float hitReactionLayerDecrementSpeed = 1.5f;
    private Vector3 rayCastPosition;
    #endregion

    #region 3. Unity Lifecycle

    public override void Awake()
    {
        InitilizeStates();
        controller.Initialize(properties, mod, velBody, animator, myStats, navAgent, navObstacle, aiStates);
        mod.setModSpot(ModSpot.ArmR);
        mod.AttachAffect(ref myStats, velBody);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);

        controller.checksToUpdateState = new List<Func<bool>>() {
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
        base.OnDeath();
        mod.KillCoroutines();
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
        chase = new MallCopChaseState();
        chase.stateName = "chase";
        fire = new MallCopFireState();
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

    void ShowChargeEffect ()
    {
        GameObject vfx = ObjectPool.Instance.GetObject(PoolObjectType.VFXEnemyChargeBlaster);
        Vector3 scaleBackup = vfx.transform.localScale;
        vfx.transform.SetParent (mod.transform);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;
        vfx.transform.localScale = new Vector3 (
            scaleBackup.x / vfx.transform.localScale.x,
            scaleBackup.y / vfx.transform.localScale.y,
            scaleBackup.z / vfx.transform.localScale.z
        );
    }

    #endregion

}

