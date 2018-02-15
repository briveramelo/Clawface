//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class MallCopReanimatorChaseState : AIState {
    private float awayDistance = 20.0f;

    public override void OnEnter() {

        if(controller.AttackTarget.gameObject == null)
        controller.AttackTarget = controller.FindPlayer();

        navAgent.speed = myStats.moveSpeed;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Walk);
    }
    public override void Update() {
        Chase();        
    }
    public override void OnExit() {

    }

    void Chase() {
        if (controller.AttackTarget.gameObject.tag == Strings.Tags.PLAYER)
            navAgent.SetDestination(controller.transform.position + (controller.DirectionToTarget * -1.0f * awayDistance));
        else if (controller.AttackTarget.gameObject.tag == Strings.Layers.ENEMY)
        {
            if (controller.AttackTarget.GetComponent<AIController>().IsStunned())
            //Move towards stunned enemy
            {
                navAgent.SetDestination(controller.AttackTarget.position);
            }

            if (!controller.AttackTarget.gameObject.activeSelf)
            {
                controller.AttackTarget = controller.FindPlayer();
                navAgent.SetDestination(controller.transform.position + (controller.DirectionToTarget * -1.0f * awayDistance));
            }
        }
        
    }

}
