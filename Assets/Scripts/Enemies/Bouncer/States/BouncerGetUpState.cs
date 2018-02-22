using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerGetUpState : AIState {

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;

    }
    public override void Update()
    {
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }

    public void Up()
    {
        controller.UpdateState(EAIState.Chase);
    }
}
