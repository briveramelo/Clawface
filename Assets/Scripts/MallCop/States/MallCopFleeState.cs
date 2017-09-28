//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFleeState : MallCopState {

    private bool isFinishedFleeing;

    public override void OnEnter() {
        JumpAway();
        Timing.RunCoroutine(RunFleeTimer());
    }
    public override void Update() {
        velBody.LookAt(controller.AttackTarget);
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

    IEnumerator<float> RunFleeTimer() {
        isFinishedFleeing = false;
        yield return Timing.WaitForSeconds(.5f);
        isFinishedFleeing = true;
    }

}
