//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFleeState : MallCopState {

    private bool isFinishedFleeing;
    private float moveAwayDistance = 5.0f;
    private Vector3 moveAwayDirection;
    private Vector3 newDestination;

    public override void OnEnter() {
        velBody.velocity = Vector3.zero;
        RaycastBackCheck();     
    }
    public override void Update() {

        
    }
    public override void OnExit() {
    }

    public bool IsFinished() {
        return isFinishedFleeing;
    }

    private void MoveAway() {
        navAgent.SetDestination(newDestination);
    }

    IEnumerator<float> RunFleeTimer() {
        isFinishedFleeing = false;
        yield return Timing.WaitForSeconds(1.0f);
        //navAgent.SetDestination(velBody.transform.position);
        isFinishedFleeing = true;
    }

    private void RaycastBackCheck()
    {
        moveAwayDirection = (velBody.transform.position - controller.AttackTargetPosition);
        moveAwayDirection = Vector3.Reflect(moveAwayDirection, velBody.transform.up);
        moveAwayDirection = new Vector3(moveAwayDirection.x, 0.0f,moveAwayDirection.z);
        moveAwayDirection.Normalize();
        newDestination = navAgent.transform.position + (moveAwayDirection * moveAwayDistance);

        if (Physics.Raycast(navAgent.transform.position, newDestination, 10))
        {
            return;
        }
        else
        {
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);
            Timing.RunCoroutine(RunFleeTimer());
            MoveAway();
        }
            
    }



}
