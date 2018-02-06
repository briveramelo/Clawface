using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class KamikazeMommyAttackState : AIState
{
    private bool attackDone = false;
    private float kamikazeSpawnProbablity;
    private float kamikazePulserSpawnProbability;

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        attackDone = false;
        kamikazeSpawnProbablity = properties.kamikazeProbability;
        kamikazePulserSpawnProbability = properties.kamikazePulserProbability;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Attack1);
        Attack();
    }
    public override void Update()
    {
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
        controller.transform.LookAt(lookAtPosition);
        navAgent.velocity = Vector3.zero;
    }
    public override void OnExit()
    {
        attackDone = false;
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }

    private void Attack()
    {
        //Make sure the kamikaze is not stunned
        if (myStats.health <= myStats.skinnableHealth)
        {
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
        }

        float probabilitySum = kamikazeSpawnProbablity + kamikazePulserSpawnProbability;
        float randomRoll = Random.Range(0.0f,probabilitySum);

        //Check random roll case

        if (randomRoll <= kamikazeSpawnProbablity)
        {
            GameObject kamikaze = ObjectPool.Instance.GetObject(PoolObjectType.Kamikaze);
            if (kamikaze)
            {
                kamikaze.transform.position = controller.transform.position;
            }
        }
        else if (randomRoll > kamikazeSpawnProbablity && randomRoll <= probabilitySum)
        {
            GameObject kamikaze = ObjectPool.Instance.GetObject(PoolObjectType.KamikazePulser);
            if (kamikaze)
            {
                kamikaze.transform.position = controller.transform.position;
            }

        }

        

        attackDone = true;
    }

    public bool DoneAttacking()
    {
        return attackDone;
    }


}
