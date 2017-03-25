using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopSwingerController : MallCopController {

    public float distanceToPlayer;

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

            attackTarget = other.gameObject;
            UpdateState(EMallCopState.Chase);
        }
    }

    void CheckToSwing(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.chase) {
                distanceToPlayer = Vector3.Distance(transform.position, attackTarget.transform.position);
                bool inStrikingDistance = distanceToPlayer < properties.strikingDistance;

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
            if (CurrentState == states.twitch) {
                if (stats.health > 0) {
                    UpdateState(EMallCopState.Chase);
                    justSwitchedState = true;
                }
            }
        }
    }
}
