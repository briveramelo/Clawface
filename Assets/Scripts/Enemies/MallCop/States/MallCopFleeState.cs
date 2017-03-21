using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopFleeState : MallCopState {

    public override void OnEnter() {
        JumpAway();
    }
    public override void Update() {
        Vector3 lookAtPosition = new Vector3(controller.attackTarget.transform.position.x, 0, controller.attackTarget.transform.position.z);
        velBody.transform.LookAt(lookAtPosition);
        velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);



    }
    public override void OnExit() {

    }

    private void JumpAway() {
        
    }
}
