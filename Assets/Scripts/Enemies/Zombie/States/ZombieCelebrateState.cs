using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ZombieCelebrateState : AIState {

    private bool alreadyCelebrating = false;

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        alreadyCelebrating = true;
        animator.SetInteger("AnimationState", -1);
        animator.SetInteger("VictoryDanceIndex", Random.Range(0, 5));
        animator.SetTrigger("DoVictoryDance");
    }
    public override void Update()
    {
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
        alreadyCelebrating = false;
    }

    public bool isCelebrating()
    {
        return alreadyCelebrating;
    }
}
