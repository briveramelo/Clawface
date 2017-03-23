//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopBlasterController : MallCopController {

    [SerializeField] public float fleeForce;
    [SerializeField] float maxDistanceBeforeChasing;
    [SerializeField] float distanceToFire;

    protected override void Update() {

        bool justSwitchedState = false;
        CheckToPatrol(ref justSwitchedState);
        CheckToFire(ref justSwitchedState);
        CheckFinishTwitching(ref justSwitchedState);
        CheckToFinishFiring(ref justSwitchedState);

        base.Update();
    }   

    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState != states.flee) {

            attackTarget = other.transform;
            UpdateState(EMallCopState.Flee);
        }
    }

    void CheckToFire(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if ((CurrentState == states.flee && states.flee.IsFinished()) ||
                (CurrentState == states.chase && distanceFromTarget < distanceToFire)) {                
                UpdateState(EMallCopState.Fire);
                justSwitchedState = true;
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


    void CheckFinishTwitching(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.twitch && !states.twitch.IsMidTwitch()) {
                if (stats.health > 0) {
                    UpdateState(EMallCopState.Chase);
                    justSwitchedState = true;
                }
            }
        }
    }

    void CheckToFinishFiring(ref bool justSwitchedState) {
        if (!justSwitchedState) {
            if (CurrentState == states.fire && states.fire.CanRestart()) {

                bool shouldChase = distanceFromTarget > maxDistanceBeforeChasing;

                if (shouldChase) {
                    UpdateState(EMallCopState.Chase);
                }
                else {
                    UpdateState(EMallCopState.Fire);
                }                
                justSwitchedState = true;
            }
        }
    }
    
}