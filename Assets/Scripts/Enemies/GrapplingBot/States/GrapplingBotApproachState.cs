using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class GrapplingBotApproachState : GrapplingBotState {

    private bool isDone;

    public override void OnEnter() {
        Timing.RunCoroutine(Timer());
        Approach();
    }
    public override void Update() {
        velBody.LookAt(controller.attackTarget);
    }
    public override void OnExit() {

    }
    IEnumerator<float> Timer() {
        isDone = false;
        yield return Timing.WaitForSeconds(1f);
        isDone = true;
    }

    void Approach() {
        Vector3 approachDirection = controller.directionToTarget;
        velBody.AddDecayingForce(approachDirection * properties.approachForce);
    }

    public bool IsDone() {
        return isDone;
    }
}
