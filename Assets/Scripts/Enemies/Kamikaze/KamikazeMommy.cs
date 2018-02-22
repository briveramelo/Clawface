using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public class KamikazeMommyProperties : AIProperties
{
}

public class KamikazeMommy : EnemyBase
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] private KamikazeMommyProperties properties;
    #endregion

    #region 3. Private fields


    //The AI States of the Kamikaze
    private float closeEnoughToAttackDistance;

    private KamikazeChaseState chase;
    private KamikazeMommyAttackState attack;
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
        controller.Initialize(properties, velBody, animator, myStats, navAgent, navObstacle, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckToAttack,
            CheckToFinishAttack,
            CheckIfStunned
        };
        base.Awake();
    }
    #endregion

    #region 5. Public Methods   

    //State conditions
    bool CheckToAttack()
    {
        if (controller.CurrentState == chase && controller.DistanceFromTarget < closeEnoughToAttackDistance)
        {
            controller.UpdateState(EAIState.Attack);
            return true;
        }
        return false;
    }


    bool CheckToFinishAttack()
    {
        if (controller.CurrentState == attack && attack.DoneAttacking())
        {
            if (controller.DistanceFromTarget < closeEnoughToAttackDistance)
            {
                controller.UpdateState(EAIState.Attack);
            }
            else
            {
                controller.UpdateState(EAIState.Chase);
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

    public override void ResetForRebirth()
    {
        base.ResetForRebirth();
    }

    public override void DoPlayerKilledState(object[] parameters)
    {
        if (myStats.health > myStats.skinnableHealth)
        {
            attack.playerDead = true;
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

    public void GetUpDone()
    {
        getUp.Up();
    }

    #endregion

    #region 6. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new KamikazeChaseState();
        chase.stateName = "chase";
        attack = new KamikazeMommyAttackState();
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
        myStats.health = EnemyStatsManager.Instance.kamikazeMommyStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.kamikazeMommyStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.kamikazeMommyStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.kamikazeMommyStats.speed;
        myStats.attack = EnemyStatsManager.Instance.kamikazeMommyStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.kamikazeMommyStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.kamikazeMommyStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.kamikazeMommyStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.kamikazeMommyStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.kamikazeMommyStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.kamikazeMommyStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.kamikazeMommyStats.stunnedTime;

        closeEnoughToAttackDistance = EnemyStatsManager.Instance.kamikazeMommyStats.closeEnoughToAttackDistance;
        properties.spawnRate = EnemyStatsManager.Instance.kamikazeMommyStats.spawnRate;
        properties.kamikazeProbability = EnemyStatsManager.Instance.kamikazeMommyStats.kamikazeSpawnProbability;
        properties.kamikazePulserProbability = EnemyStatsManager.Instance.kamikazeMommyStats.kamikazePulserSpawnProbability;

        myStats.SetStats();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }


    #endregion



}

