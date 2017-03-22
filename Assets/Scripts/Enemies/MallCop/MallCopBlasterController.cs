using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopBlasterController : MallCopController {

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