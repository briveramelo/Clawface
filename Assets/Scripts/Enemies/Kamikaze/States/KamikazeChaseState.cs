using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeChaseState : AIState {

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
        if (navAgent.enabled && navAgent.isOnNavMesh)
            navAgent.SetDestination(controller.AttackTarget.position);
    }
}
