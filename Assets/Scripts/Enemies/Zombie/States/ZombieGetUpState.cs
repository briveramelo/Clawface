using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ZombieGetUpState : AIState {

    public TrailRenderer trailRenderer;
    public bool needToClearTrail;

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.GetUp);
    }

    public override void Update()
    {
    }
    public override void OnExit()
    {
    }

    public void Up()
    {
         if (needToClearTrail)
        {
            trailRenderer.Clear();
            needToClearTrail = false;
        }
        if (trailRenderer) trailRenderer.enabled = true;

        controller.UpdateState(EAIState.Chase);
    }
}
