//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MallCopChaseState : MallCopState {    

    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);        
    }
    public override void Update() {
        Chase();        
    }
    public override void OnExit() {
        
    }

    private void Chase() {

        //Orient cop towards player 
        if(navAgent.enabled && navAgent.isOnNavMesh)
        navAgent.SetDestination(controller.AttackTarget.position);
    }
}
