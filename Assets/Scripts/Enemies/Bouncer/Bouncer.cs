using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public class BouncerProperties : AIProperties
{
    [Range(1, 10)] public int maxBouncerBounces;
    [Range(1, 10)] public int minBouncerBounces;
    [Range(1, 10)] public int maxBouncerShots;
    [Range(1, 10)] public int minBouncerShots;
    public bool bouncerRotate;

    public void InitializeProperties()
    {
        maxBounces = maxBouncerBounces;
        minBounces = minBouncerBounces;
        maxShots = maxBouncerShots;
        minShots = minBouncerShots;
        rotate = bouncerRotate;
    }

}

public class Bouncer : EnemyBase
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] float closeEnoughToAttackDistance;
    [SerializeField] private BouncerProperties properties;
    [SerializeField] private BulletHellPatternController bulletPatternController;
    #endregion

    #region 3. Private fields

    //The AI States of the Zombie
    private BouncerChaseState chase;
    private BouncerFireState fire;
    private BouncerStunState stun;


    #endregion

    #region 4. Unity Lifecycle

    public override void Awake()
    {
        InitilizeStates();
        properties.InitializeProperties();
        controller.Initialize(properties, velBody, animator, myStats, navAgent, navObstacle, bulletPatternController, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);

        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckToAttack,
            CheckToFinishAttacking,
            CheckIfStunned

        };

        base.Awake();
    }

    #endregion

    #region 5. Public Methods   

    bool CheckToAttack()
    {
        if (controller.CurrentState == chase && chase.OverMaxJumpCount())
        {
            controller.UpdateState(EAIState.Fire);
            return true;
        }
        return false;
    }
    bool CheckToFinishAttacking()
    {
        if (controller.CurrentState == fire)
        {
            if (myStats.health <= myStats.skinnableHealth)
            {
                controller.CurrentState = stun;
                controller.UpdateState(EAIState.Stun);
                controller.DeActivateAI();
            }

            if (fire.DoneFiring())
            {
                controller.UpdateState(EAIState.Chase);
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

    public void DoneJumpStart()
    {
        chase.doneStartingJump = true;
    }

    public void DoneJumpLanding()
    {
        chase.doneLandingJump = true;
    }


    public override void ResetForRebirth()
    {
        base.ResetForRebirth();
    }

    public void FireBullet()
    {
        if (fire.shotCount >= fire.maxShots)
        {
            fire.doneFiring = true;
        }
        else
        {
            fire.FireBullet();
            fire.shotCount++;
        }
    }


    #endregion

    #region 6. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new BouncerChaseState();
        chase.stateName = "chase";
        fire = new BouncerFireState();
        fire.stateName = "fire";
        stun = new BouncerStunState();
        stun.stateName = "stun";
        aiStates.Add(chase);
        aiStates.Add(fire);
        aiStates.Add(stun);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }

    #endregion
}