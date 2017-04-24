//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class MallCopBlasterController : MallCopController {

    [SerializeField] public float fleeForce;
    [SerializeField] float maxDistanceBeforeChasing;
    [SerializeField] float distanceToFire;

    void Awake() {
        checksToUpdateState = new List<Func<bool>>() {
            CheckToFire,
            CheckToPatrol,
            CheckFinishTwitching,
            CheckToFinishFiring,
<<<<<<< HEAD
<<<<<<< HEAD
            CheckFinishGettingUp
=======
            CheckToFlee,
>>>>>>> refs/remotes/origin/master
=======
            CheckFinishGettingUp,
            CheckToFlee,
>>>>>>> origin/Art
        };
    } 

    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState != states.flee) {

            AttackTarget = other.transform;
            UpdateState(EMallCopState.Flee);
        }
    }

    bool CheckToFire() {
        if ((CurrentState == states.flee && states.flee.IsFinished()) ||
            (CurrentState == states.chase && distanceFromTarget < distanceToFire)) {                
            UpdateState(EMallCopState.Fire);
            return true;
        }
        return false;
    }

    bool CheckToPatrol() {
        if (CurrentState == states.chase &&
            timeInLastState > properties.maxChaseTime &&
            AttackTarget != null) {

            UpdateState(EMallCopState.Patrol);
            return true;
        }        
        return false;
    }


    bool CheckFinishTwitching() {
        if (CurrentState == states.twitch && !states.twitch.IsMidTwitch()) {
            if (stats.health > 0) {
                UpdateState(EMallCopState.Chase);
                return true;
            }
        }
        return false;        
    }

    bool CheckFinishGettingUp () {
        if (CurrentState == states.gettingUp && states.gettingUp.IsDoneGettingUp) {
            if (stats.health > 0) {
                UpdateState(EMallCopState.Chase);
                return true;
            }
        }
        return false;
    }

    bool CheckToFinishFiring() {
        if (CurrentState == states.fire && states.fire.CanRestart()) {

            bool shouldChase = distanceFromTarget > maxDistanceBeforeChasing;

            if (shouldChase) {
                UpdateState(EMallCopState.Chase);
            }
            else {
                UpdateState(EMallCopState.Fire);
            }
            return true;
        }        
        return false;
    }

}