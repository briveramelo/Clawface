using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class GrapplingBotExploderController : GrapplingBotController {

    private Quaternion rotatorStartRotation;
    private Vector3 startPosition;
    private Vector3 startScale;
    private Quaternion startRotation;

    void Start() {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        startRotation = transform.rotation;
        rotatorStartRotation = transform.parent.rotation;

        Timing.RunCoroutine(Begin());
    }

    IEnumerator<float> Begin() {
        yield return Timing.WaitForOneFrame;
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

    public override void ResetForRebirth()
    {
        transform.localPosition = startPosition;
        transform.localScale = startScale;
        transform.localRotation = startRotation;
        transform.parent.rotation = rotatorStartRotation;

        base.ResetForRebirth();
    }

    public override void OnDeath() {
        bot.OnDeath();        
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
