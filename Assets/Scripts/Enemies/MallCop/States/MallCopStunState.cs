﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopStunState : AIState {

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Stunned);
    }
    public override void Update()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Stunned);
    }
    public override void OnExit()
    {
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }

}
