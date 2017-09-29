//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MallCopChaseState : MallCopState {    

    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);
        controller.AttackTarget = controller.FindPlayer();
        Debug.Log("made it to chase begin");
    }
    public override void Update() {
        Chase();        
    }
    public override void OnExit() {

    }

    void Chase() {

        //Orient cop towards player
        if (Vector3.Distance(controller.AttackTarget.position, velBody.transform.position) > 100.0f)
        {
            Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, 0, controller.AttackTarget.position.z);
            velBody.transform.LookAt(lookAtPosition);
            velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);
        }
        velBody.velocity = Vector3.zero;    

        if(navAgent.enabled && navAgent.isOnNavMesh)
        navAgent.SetDestination(controller.AttackTarget.position);
    }
}
