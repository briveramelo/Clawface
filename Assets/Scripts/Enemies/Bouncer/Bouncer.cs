using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public class BouncerProperties : AIProperties
{
    [Range(1, 10)] public int bouncerBounces;
    [Range(1f, 100f)] public float bouncerWaitShotTime;

    public void InitializeProperties()
    {
        bounces = bouncerBounces;
        waitShotTime = bouncerWaitShotTime;
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

            if (fire.DoneFiring())
            {
                controller.UpdateState(EAIState.Chase);
                return true;
            }
            else
            {
                return false;
            }
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

    public override void ResetForRebirth()
    {
        copUICanvas.gameObject.SetActive(false);
        base.ResetForRebirth();
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

   
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }
}