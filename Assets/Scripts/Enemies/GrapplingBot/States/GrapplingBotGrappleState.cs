using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingBotGrappleState : GrapplingBotState {

    private bool isDone;
    private bool hitTarget;

    public override void OnEnter() {
        controller.ActivateMod();
    }
    public override void Update() {
        velBody.LookAt(controller.attackTarget);
        if (true) {
            hitTarget = true;
        }
    }
    public override void OnExit() {

    }

    public bool IsDone() {
        return isDone;
    }

    public bool HitTarget() {
        return hitTarget;
    }
}
