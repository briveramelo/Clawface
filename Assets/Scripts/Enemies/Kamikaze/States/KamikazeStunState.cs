using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeStunState : AIState {

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Stunned);
    }
    public override void Update()
    {
    }
    public override void OnExit()
    {
        navAgent.enabled = true;
        navObstacle.enabled = false;
    }
}
