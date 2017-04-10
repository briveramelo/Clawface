using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using ModMan;

public class GrapplingBotExploderController : GrapplingBotController {

    private Quaternion rotatorStartRotation;
    private TransformMemento tMemento = new TransformMemento();

    void Start() {
        tMemento.Initialize(transform);
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

            AttackTarget = other.transform;
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

    public override void ResetForRebirth()
    {
        transform.Reset(tMemento);
        transform.parent.rotation = rotatorStartRotation;

        base.ResetForRebirth();
    }

    public override void OnDeath()
    {
        bot.OnDeath();
        //Timing.KillCoroutines(GetInstanceID().ToString());        
    }
}
