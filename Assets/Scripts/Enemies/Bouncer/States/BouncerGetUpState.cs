using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerGetUpState : AIState {

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
}
