using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeCelebrateState : AIState {

    private bool alreadyCelebrating = false;

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        alreadyCelebrating = true;
        animator.SetInteger("AnimationState", -1);
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
