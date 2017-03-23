//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopFleeState : MallCopState {

    private bool isFinishedFleeing;

    public override void OnEnter() {
        JumpAway();
        controller.StartCoroutine(RunFleeTimer());
    }
    public override void Update() {
        velBody.LookAt(controller.attackTarget);
        velBody.velocity = Vector3.zero;
    }
    public override void OnExit() {

    }

    public bool IsFinished() {
        return isFinishedFleeing;
    }

    private void JumpAway() {
        Vector3 jumpDir = -controller.directionToTarget;
        velBody.AddDecayingForce(jumpDir * ((MallCopBlasterController)controller).fleeForce);
    }

    IEnumerator RunFleeTimer() {
        isFinishedFleeing = false;
        yield return new WaitForSeconds(.5f);
        isFinishedFleeing = true;
    }

}
