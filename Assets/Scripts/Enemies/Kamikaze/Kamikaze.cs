using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public class KamikazeProperties : AIProperties
{
    [Range(1f, 5f)] public float kamikazeSelfDestructTime;
    [Range(1f, 100f)] public float kamikazeBlastRadius;

    public void InitializeProperties()
    {
        selfDestructTime = kamikazeSelfDestructTime;
        blastRadius = kamikazeBlastRadius;
    }
}

public class Kamikaze : EnemyBase
{

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] float closeEnoughToAttackDistance;
    [SerializeField] private KamikazeProperties properties;
    #endregion

    #region 3. Private fields
    //The AI States of the Kamikaze
    private KamikazeChaseState chase;
    private KamikazeAttackState attack;
    private KamikazeStunState stun;
    #endregion

    #region 4. Unity Lifecycle
    public override void Awake()
    {
        InitilizeStates();
        properties.InitializeProperties();
        controller.Initialize(properties,velBody, animator, myStats, navAgent, navObstacle, aiStates);
        damaged.Set(DamagedType.MallCop, bloodEmissionLocation);
        controller.checksToUpdateState = new List<Func<bool>>() {
            CheckToSelfDestruct,
            CheckIfStunned
        };
        base.Awake();
    }
    #endregion

    #region 5. Public Methods   
    
    //State conditions
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
        chase = new KamikazeChaseState();
        chase.stateName = "chase";
        attack = new KamikazeAttackState();
        attack.stateName = "attack";
        stun = new KamikazeStunState();
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