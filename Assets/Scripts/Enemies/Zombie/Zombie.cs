using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


[System.Serializable]
public class ZombieProperties : AIProperties
{
    [HideInInspector] public float zombieHitRate;

    public void InitializeProperties()
    {
        hitRate = zombieHitRate;
    }
}

public class Zombie : EnemyBase
{
    #region 1. Serialized Unity Inspector Fields

    [SerializeField] float closeEnoughToAttackDistance;
    [SerializeField] float maxDistanceBeforeChasing = 2.0f;
    [SerializeField] private ZombieProperties properties;
    [SerializeField] private TentacleTrigger tentacle;
    #endregion

    #region 2. Private fields

    //The AI States of the Zombie
    private ZombieChaseState chase;
    private ZombieAttackState attack;
    private ZombieStunState stun;

    #endregion


    #region 3. Unity Lifecycle

    public override void Awake()
    {
        InitilizeStates();
        properties.InitializeProperties();
        controller.Initialize(properties,velBody, animator, myStats, navAgent, navObstacle,aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckToAttack,
            CheckToFinishAttacking,
            CheckIfStunned
        };
        base.Awake();
    }

    #endregion

    #region 4. Public Methods   

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
    bool CheckToFinishAttacking()
    {
        if (controller.CurrentState == attack && attack.CanRestart())
        {
            bool shouldChase = controller.DistanceFromTarget > maxDistanceBeforeChasing;

            if (shouldChase)
            {
                controller.UpdateState(EAIState.Chase);
            }
            else
            {
                controller.UpdateState(EAIState.Attack);
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

    public void ActivateTentacleTrigger()
    {
        tentacle.ActivateTriggerDamage();
    }

    public void DeactivateTentacleTrigger()
    {
        tentacle.DeactivateTriggerDamage();
    }

    public void DamageAttackTarget()
    {
        attack.Damage(controller.AttackTarget.gameObject.GetComponent<IDamageable>());
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void ResetForRebirth()
    {
        copUICanvas.gameObject.SetActive(false);
        base.ResetForRebirth();
    }

    #endregion

    #region 5. Private Methods    

    private void InitilizeStates()
    {
        aiStates = new List<AIState>();
        chase = new ZombieChaseState();
        chase.stateName = "chase";
        attack = new ZombieAttackState();
        attack.stateName = "attack";
        stun = new ZombieStunState();
        stun.stateName = "stun";
        aiStates.Add(chase);
        aiStates.Add(attack);
        aiStates.Add(stun);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
    }

    #endregion


}
