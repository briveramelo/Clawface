using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    //The AI States of the Kamikaze
    private KamikazeChaseState chase;
    private KamikazePulserAttackState attack;
    private KamikazeStunState stun;
    private KamikazeCelebrateState celebrate;
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
        aiStates.Add(chase);
        aiStates.Add(attack);
        aiStates.Add(stun);
        aiStates.Add(celebrate);
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
