using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopBlasterController : MallCopController {

    public override State CurrentState {
        set {
            if (stats.health > 0 || value == states.fall) {
                if ((currentState == states.twitch && !states.twitch.IsMidTwitch()) || currentState != states.twitch) {
                    if (currentState != null) {
                        currentState.OnExit();
                    }
                    currentState = value;
                    mystate = currentState.ToString();
                    currentState.OnEnter();
                    StartCoroutine(IERestartStateTimer());
                }
            }
        }
    }


    protected override void Update() {

        if (CurrentState == states.chase &&
            timeInLastState > properties.maxChaseTime &&
            attackTarget != null) {

            CurrentState = states.patrol;
        }
        base.Update();
    }


    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState != states.flee) {

            attackTarget = other.gameObject;
            CurrentState = states.flee;
        }
    }
  
}