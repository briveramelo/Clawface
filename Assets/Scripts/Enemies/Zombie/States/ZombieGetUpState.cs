using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ZombieGetUpState : AIState {

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetTrigger("DoGetUp");
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
