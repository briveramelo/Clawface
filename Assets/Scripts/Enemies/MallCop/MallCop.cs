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

        mod.damage = myStats.attack;

        base.Awake();
    }

    #endregion

    #region 4. Public Methods   

    //State conditions
    bool CheckToFire()
    {
        Vector3 fwd = controller.DirectionToTarget;
        RaycastHit hit;

        if ((controller.CurrentState== chase && controller.DistanceFromTarget <= closeEnoughToFireDistance))
        {
            if (Physics.Raycast(controller.transform.position, fwd, out hit, 50, LayerMask.GetMask(Strings.Layers.MODMAN, Strings.Layers.OBSTACLE)))
            {
                if(hit.transform.tag == Strings.Tags.PLAYER)
                controller.UpdateState(EAIState.Fire);
            }
            return true;
        }
        return false;
    }
    bool CheckToFinishFiring()
    {
        if (controller.CurrentState == fire)
        {

            if (controller.DistanceFromTarget > closeEnoughToFireDistance)
            {
                fire.StartEndFire();
            }
            else
            {
                Vector3 fwd = controller.DirectionToTarget;
                RaycastHit hit;

                if (Physics.Raycast(controller.transform.position, fwd, out hit, 50, LayerMask.GetMask(Strings.Layers.MODMAN,Strings.Layers.OBSTACLE)))
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
        base.OnDeath();
        mod.KillCoroutines();
    }

    public override void ResetForRebirth()
    {
        mod.DeactivateModCanvas();
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

    public void StartAiming ()
    {
        animator.SetLayerWeight (2, 1.0f);
    }

    public void StopAiming ()
    {
        animator.SetLayerWeight (2, 0.0f);
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
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(playerDetectorSphereCollider.transform.position, maxDistanceBeforeChasing);
    }

    #endregion

}

