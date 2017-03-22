using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopFleeState : MallCopState {

    public override void OnEnter() {
        JumpAway();
    }
    public override void Update() {
        velBody.LookAt(controller.attackTarget.transform);
    }
    public override void OnExit() {

    }

    private void JumpAway() {
        Vector3 jumpDir = (controller.attackTarget.transform.position - velBody.transform.position).normalized;
        jumpDir.y = 0;
        jumpDir.Normalize();
        velBody.AddDecayingForce(jumpDir * 100f);
    }

}
