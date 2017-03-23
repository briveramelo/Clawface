//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopSwingerController : MallCopController {

    public float strikingDistance;

    protected override void Update() {

        bool justSwitchedState = false;
        CheckToPatrol(ref justSwitchedState);
        CheckForFinishedSwing(ref justSwitchedState);
        CheckToSwing(ref justSwitchedState);
        CheckForFinishedTwitch(ref justSwitchedState);

        base.Update();
    }

    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState != states.chase && CurrentState != states.swing) {

            attackTarget = other.transform;
            UpdateState(EMallCopState.Chase);
        }
    }

    void CheckToSwing(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.chase) {                
                bool inStrikingDistance = distanceFromTarget < strikingDistance;

                if (inStrikingDistance) {
                    UpdateState(EMallCopState.Swing);
                    justSwitchedState = true;
                }
            }
        }
    }

    void CheckToPatrol(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.chase &&
                timeInLastState > properties.maxChaseTime &&
                attackTarget != null) {

                UpdateState(EMallCopState.Patrol);
                justSwitchedState = true;
            }
        }
    }

    void CheckForFinishedSwing(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.swing) {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f) {
                    UpdateState(EMallCopState.Chase);
                    justSwitchedState = true;
                }
            }
        }
    }

    void CheckForFinishedTwitch(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.twitch && !states.twitch.IsMidTwitch()) {
                if (stats.health > 0) {
                    UpdateState(EMallCopState.Chase);
                    justSwitchedState = true;
                }
            }
        }
    }
}
