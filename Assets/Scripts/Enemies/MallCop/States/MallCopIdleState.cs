using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopIdleState : MallCopState {

    public override void OnEnter() {}

    public override void OnExit() {}

    public override void Update() {
        velBody.velocity = Vector3.zero;
    }
}
