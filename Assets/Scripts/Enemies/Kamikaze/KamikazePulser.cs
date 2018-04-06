using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class KamikazePulserProperties : AIProperties
{
}

public class KamikazePulser : EnemyBase
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] private KamikazePulserProperties properties;
    #endregion

    #region 3. Private fields
    private float closeEnoughToAttackDistance;
    private bool isUp;

    //The AI States of the Kamikaze
    private KamikazeChaseState chase;
    private KamikazePulserAttackState attack;
    private KamikazeStunState stun;
    private KamikazeCelebrateState celebrate;
    private KamikazeGetUpState getUp;
    #endregion

    #region 4. Unity Lifecycle
    protected override void Awake()
    {        
	isUp = false;
        myStats = GetComponent<Stats>();
        SetAllStats();
        InitilizeStates();
        attack.waitTimeAfterAttack = EnemyStatsManager.Instance.kamikazePulserStats.waitTimeAfterAttack;
        controller.Initialize(properties, velBody, animator, myStats, navAgent, navObstacle, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckDoneGettingUp,
            CheckPlayerDead,
            CheckToAttack,
            CheckToFinishAttack,
            CheckIfStunned
        };
        base.Awake();
    }
    #endregion

    #region 5. Public Methods   

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
                attack.StopPulse();
                controller.CurrentState = celebrate;
                controller.UpdateState(EAIState.Celebrate);
            }
            return true;
        }
        return false;
    }

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
            attack.StopPulse();
            controller.CurrentState = stun;
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
            return true;
        }
        return false;
    }
    public override void OnDeath()
    {
        isUp = false;
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        SetScorePoints(EnemyStatsManager.Instance.kamikazePulserStats.scoreValue);
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
        isUp = true;
        controller.CurrentState = chase;
        controller.UpdateState(EAIState.Chase);
    }

    public void SetScorePoints(int score)
    {
        scoreValue = score;
    }
    #endregion

    #region 6. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new KamikazeChaseState();
        chase.stateName = "chase";
        attack = new KamikazePulserAttackState();
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
        myStats.health = EnemyStatsManager.Instance.kamikazePulserStats.health;
        myStats.maxHealth = EnemyStatsManager.Instance.kamikazePulserStats.maxHealth;
        myStats.skinnableHealth = EnemyStatsManager.Instance.kamikazePulserStats.skinnableHealth;
        myStats.moveSpeed = EnemyStatsManager.Instance.kamikazePulserStats.speed;
        myStats.attack = EnemyStatsManager.Instance.kamikazePulserStats.attack;

        navAgent.speed = EnemyStatsManager.Instance.kamikazePulserStats.speed;
        navAgent.angularSpeed = EnemyStatsManager.Instance.kamikazePulserStats.angularSpeed;
        navAgent.acceleration = EnemyStatsManager.Instance.kamikazePulserStats.acceleration;
        navAgent.stoppingDistance = EnemyStatsManager.Instance.kamikazePulserStats.stoppingDistance;

        scoreValue = EnemyStatsManager.Instance.kamikazePulserStats.scoreValue;
        eatHealth = EnemyStatsManager.Instance.kamikazePulserStats.eatHealth;
        stunnedTime = EnemyStatsManager.Instance.kamikazePulserStats.stunnedTime;

        closeEnoughToAttackDistance = EnemyStatsManager.Instance.kamikazePulserStats.closeEnoughToAttackDistance;
        properties.maxPulses = EnemyStatsManager.Instance.kamikazePulserStats.maxPulses;
        properties.pulseRate = EnemyStatsManager.Instance.kamikazePulserStats.pulseRate;
        properties.scaleRate = EnemyStatsManager.Instance.kamikazePulserStats.scaleRate;
        properties.maxScale = EnemyStatsManager.Instance.kamikazePulserStats.maxScale;

        myStats.SetStats();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }
    #endregion



}
