using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingBotExploderController : GrapplingBotController {

    void Start() {
        checksToUpdateState = new List<System.Func<bool>>() {
            CheckToEndApproach,
            CheckToEndGrapple,
        };
        CurrentState = states.patrol;
    }

    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState == states.patrol) {

            attackTarget = other.transform;
            UpdateState(EGrapplingBotState.Approach);
        }
    }

    bool CheckToEndApproach() {
        if (CurrentState == states.approach && states.approach.IsDone()) {            
            if (distanceFromTarget <= properties.grappleDistance) {
                UpdateState(EGrapplingBotState.Grapple);
            }
            else {
                UpdateState(EGrapplingBotState.Approach);
            }
            return true;
        }
        return false;
    }

    bool CheckToEndGrapple() {
        if (CurrentState == states.grapple && states.grapple.IsDone()) {
            if (states.grapple.HitTarget()) {
                UpdateState(EGrapplingBotState.Explode);
            }
            else {
                UpdateState(EGrapplingBotState.Approach);
            }
            return true;
        }
        return false;
    }
}
