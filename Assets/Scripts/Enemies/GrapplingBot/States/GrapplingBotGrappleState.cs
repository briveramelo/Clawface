using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingBotGrappleState : GrapplingBotState {

    private bool isDone;
    private GrapplerMod mod;

    public void AddedInitialize(GrapplerMod mod) {
        this.mod = mod;
    }

    public override void OnEnter() {
        controller.ActivateMod();
    }

    public override void Update() {
        velBody.LookAt(controller.AttackTarget);
        velBody.velocity = Vector3.zero;
    }
    public override void OnExit() {
        
    }

    public bool IsDone() {
        return controller.timeInLastState > 1f || HitTarget();
    }

    public bool HitTarget() {
        return mod.HitTargetThisShot();
    }
}
