using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseState : AIState {

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Walk);
        controller.AttackTarget = controller.FindPlayer();
        navAgent.speed = myStats.moveSpeed;
    }
    public override void Update()
    {
        Chase();
    }
    public override void OnExit()
    {

    }

    private void Chase()
    {
        navAgent.SetDestination(controller.AttackTarget.position);
    }


}
