using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class GrapplingBotApproachState : GrapplingBotState {

    private bool isDone;

    public override void OnEnter() {
        Approach();
    }
    public override void Update() {
        velBody.LookAt(controller.attackTarget);
        velBody.velocity = Vector3.zero;
    }
    public override void OnExit() {

    }

    void Approach() {
        float crossMultipler = (Random.value > 0.5f? 1 :-1)*3f;
        Vector3 crossDirection = Vector3.Cross(controller.directionToTarget, Vector3.up);
        Vector3 targetLocation = controller.attackTarget.position + crossMultipler * crossDirection;
        Vector3 approachDirection = (targetLocation - velBody.transform.position);
        approachDirection.y = 0;
        approachDirection.Normalize();
        velBody.AddDecayingForce(approachDirection * properties.approachForce);
    }

    public bool IsDone() {
        return controller.timeInLastState > 1f;
    }
}
