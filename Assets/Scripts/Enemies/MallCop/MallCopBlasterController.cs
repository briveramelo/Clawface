//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class MallCopBlasterController : MallCopController {

    [SerializeField] SphereCollider playerDetectorSphereCollider;
    [SerializeField] public float fleeForce;
    [SerializeField] float closeEnoughToFireDistance;
    [SerializeField] float fleeRadius;
    private Collider[] playerColliderList = new Collider[10];
    float maxDistanceBeforeChasing { get { return playerDetectorSphereCollider.radius * playerDetectorSphereCollider.transform.localScale.x * transform.localScale.x; } } //assumes parenting scheme...

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToFireDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerDetectorSphereCollider.transform.position, maxDistanceBeforeChasing);
    }


    void Awake() {
        checksToUpdateState = new List<Func<bool>>() {
            CheckToFire,
            CheckToPatrol,
            CheckFinishTwitching,
            CheckToFinishFiring,
<<<<<<< HEAD
            CheckFinishGettingUp
=======
            CheckToFlee,
>>>>>>> refs/remotes/origin/master
        };
    }

    protected override void Update() {
        
        base.Update();
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag(Strings.Tags.PLAYER) && CurrentState != states.chase && CurrentState != states.flee && CurrentState != states.fire) {

            AttackTarget = other.transform;
            UpdateState(EMallCopState.Chase);
        }
    }

        

    bool CheckToFire() {
        if ((CurrentState == states.flee && states.flee.IsFinished()) ||
            (CurrentState == states.chase && distanceFromTarget < closeEnoughToFireDistance)) {                
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

    bool CheckToFlee() {
        playerColliderList = Physics.OverlapSphere(transform.position, fleeRadius, LayerMasker.GetLayerMask(Layers.ModMan));
        foreach (Collider col in playerColliderList) {
            if (col != null && col.CompareTag(Strings.Tags.PLAYER) &&
                CurrentState != states.flee) {

                AttackTarget = col.transform;
                UpdateState(EMallCopState.Flee);
                return true;
            }
        }
        return false;
    }

}