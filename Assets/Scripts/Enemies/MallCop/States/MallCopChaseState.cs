//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class MallCopChaseState : AIState {
    public override void OnEnter() {
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

        //Orient cop towards player
        if (Vector3.Distance(controller.AttackTarget.position, controller.transform.position) > 100.0f)
        {
            Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
            controller.transform.LookAt(lookAtPosition);
            //controller.transform.rotation = Quaternion.Euler(0f, controller.transform.rotation.eulerAngles.y, 0f);
        }
        navAgent.SetDestination(controller.AttackTarget.position);
    }
}
