//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class MallCopSwingerController : MallCopController {

    public float strikingDistance;

    void Start() {
        checksToUpdateState = new List<Func<bool>>() {
            CheckToSwing,
            CheckToPatrol,
            CheckForFinishedSwing,
            CheckForFinishedTwitch
        };
    }

    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState != states.chase && CurrentState != states.swing) {

            AttackTarget = other.transform;
            UpdateState(EMallCopState.Chase);
        }
    }

    bool CheckToSwing() {
        if (CurrentState == states.chase) {                
            bool inStrikingDistance = distanceFromTarget < strikingDistance;

            if (inStrikingDistance) {
                UpdateState(EMallCopState.Swing);
                return true;
            }
        }
        return false;        
    }

    bool CheckToPatrol() {
        //if (CurrentState == states.chase &&
        //    timeInLastState > properties.maxChaseTime &&
        //    AttackTarget != null) {
        if (CurrentState == states.chase && AttackTarget == null) {
            UpdateState(EMallCopState.Patrol);
            return true;
        }        
        return false;
    }

    bool CheckForFinishedSwing() {
        if (CurrentState == states.swing && states.swing.CanRestart()) {                
            if (distanceFromTarget > strikingDistance) {
                UpdateState(EMallCopState.Chase);
            }
            else {
                UpdateState(EMallCopState.Swing);
            }
            return true;
        }
        return false;
    }

    bool CheckForFinishedTwitch() {        
        if (CurrentState == states.twitch && !states.twitch.IsMidTwitch()) {
            if (stats.health > 0) {
                UpdateState(EMallCopState.Chase);
                return true;
            }
        }
        return false;
    }
}
