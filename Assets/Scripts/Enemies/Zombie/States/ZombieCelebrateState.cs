using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ZombieCelebrateState : AIState {


    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetTrigger("DoVictoryDance");
    }
    public override void Update()
    {
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }
}
