using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ZombieGetUpState : AIState {


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
        controller.UpdateState(EAIState.Chase);
    }
}
