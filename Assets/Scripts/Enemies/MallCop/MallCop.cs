//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    #endregion

    #region 2. Private fields
    //The AI States of the Mall Cop
    private MallCopChaseState chase;
    private MallCopFireState fire;
    private MallCopStunState stun;
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

        base.Awake();
    }

    #endregion

    #region 4. Public Methods   

    //State conditions
    bool CheckToFire()
    {
        if ((controller.CurrentState == chase && controller.distanceFromTarget < closeEnoughToFireDistance))
        {
            controller.UpdateState(EAIState.Fire);
            return true;
        }
        return false;
    }
    bool CheckToFinishFiring()
    {
        if (controller.CurrentState == fire && fire.CanRestart())
        {

            bool shouldChase = controller.distanceFromTarget > maxDistanceBeforeChasing;

            if (shouldChase)
            {
                controller.UpdateState(EAIState.Chase);
            }
            else
            {
                controller.UpdateState(EAIState.Fire);
            }
            return true;
        }
        return false;
    }
    bool CheckIfStunned()
    {
        if (myStats.health <= myStats.skinnableHealth)
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
        if (!will.isDead)
        {
            GameObject mallCopParts = ObjectPool.Instance.GetObject(PoolObjectType.VFXMallCopExplosion);
            if (mallCopParts)
            {
                SFXManager.Instance.Play(SFXType.BloodExplosion, transform.position);
                mallCopParts.transform.position = transform.position + Vector3.up * 3f;
                mallCopParts.transform.rotation = transform.rotation;
                mallCopParts.DeActivate(5f);
            }
            mod.KillCoroutines();
        }
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        copUICanvas.gameObject.SetActive(false);
        mod.DeactivateModCanvas();
        mod.setModSpot(ModSpot.ArmR);
        base.ResetForRebirth();
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
        aiStates.Add(chase);
        aiStates.Add(fire);
        aiStates.Add(stun);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToFireDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerDetectorSphereCollider.transform.position, maxDistanceBeforeChasing);
    }

    #endregion

}

