using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseState : ZombieState {

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);
        controller.AttackTarget = controller.FindPlayer();
        navAgent.speed = myStats.moveSpeed * properties.runMultiplier;
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
