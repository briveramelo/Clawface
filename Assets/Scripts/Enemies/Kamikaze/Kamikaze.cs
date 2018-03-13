using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public class KamikazeProperties : AIProperties
{
}

public class Kamikaze : EnemyBase
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] private KamikazeProperties properties;
    #endregion

    #region 3. Private fields
    private float selfDestructTime;
    private float blastRadius;
    private float closeEnoughToAttackDistance;

    //The AI States of the Kamikaze
    private KamikazeChaseState chase;
    private KamikazeAttackState attack;
    private KamikazeStunState stun;
    private KamikazeCelebrateState celebrate;
    private KamikazeGetUpState getUp;
    #endregion

    #region 4. Unity Lifecycle
    public override void Awake()
    {
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();
        controller.Initialize(properties,velBody, animator, myStats, navAgent, navObstacle, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckPlayerDead,
            CheckToSelfDestruct,
            CheckIfStunned,
            DeleteKamikaze
        };
        base.Awake();
    }



    #endregion

    #region 5. Public Methods   


    //State conditions
    bool CheckPlayerDead()
    {
        if (AIManager.Instance.GetPlayerDead())
        {
            if (myStats.health > myStats.skinnableHealth && !celebrate.isCelebrating())
            {
                controller.CurrentState = celebrate;
                controller.UpdateState(EAIState.Celebrate);
            }
            return true;
        }
        return false;
    }

    bool CheckToSelfDestruct()
    {
        if (controller.CurrentState == chase && controller.DistanceFromTarget < closeEnoughToAttackDistance)
        {
            controller.UpdateState(EAIState.Attack);
            return true;
        }
        return false;
    }
    bool CheckIfStunned()
    {
        if (myStats.health <= myStats.skinnableHealth || alreadyStunned)
        {
            DisableStateResidue();
            controller.CurrentState = stun;
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
            return true;
        }
        return false;
    }
    bool DeleteKamikaze()
    {

        if (controller.CurrentState == attack && attack.DoneAttacking() )
        {
            if (attack.setToSelfDestruct)
            {
                DisableStateResidue();
                OnDeath();
                return true;
            }
        }
        return false;
    }

    public override void ResetForRebirth()
    {
        attack.setToSelfDestruct = false;
        SetScorePoints(EnemyStatsManager.Instance.kamikazeStats.scoreValue);
        base.ResetForRebirth();
    }

    public override void DoPlayerKilledState(object[] parameters)
    {

    }

    public override Vector3 ReCalculateTargetPosition()
    {
        return Vector3.zero;
    }

    public void GetUpDone()
    {
        getUp.Up();
    }

    public void SetScorePoints(int score)
    {
        scoreValue = score;
    }

    public override void DisableStateResidue()
    {
        attack.StopCoroutines();
    }

    #endregion

    #region 6. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new KamikazeChaseState();
        chase.stateName = "chase";
        attack = new KamikazeAttackState();
        attack.stateName = "attack";
        stun = new KamikazeStunState();
        stun.stateName = "stun";
        celebrate = new KamikazeCelebrateState();
        celebrate.stateName = "celebrate";
        getUp = new KamikazeGetUpState();
        getUp.stateName = "getUp";
        aiStates.Add(chase);
        aiStates.Add(attack);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
        aiStates.Add(getUp);
    }

    private void SetAllStats()
    {
        myStats.health = EnemyStatsManager.Instance.kamikazeStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.kamikazeStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.kamikazeStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.kamikazeStats.speed;
        myStats.attack = EnemyStatsManager.Instance.kamikazeStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.kamikazeStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.kamikazeStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.kamikazeStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.kamikazeStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.kamikazeStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.kamikazeStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.kamikazeStats.stunnedTime;

        closeEnoughToAttackDistance = EnemyStatsManager.Instance.kamikazeStats.closeEnoughToAttackDistance;
        properties.selfDestructTime = EnemyStatsManager.Instance.kamikazeStats.selfDestructTime;
        properties.blastRadius = EnemyStatsManager.Instance.kamikazeStats.blastRadius;

        myStats.SetStats();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }


    #endregion



}