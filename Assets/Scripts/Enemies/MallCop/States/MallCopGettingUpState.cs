using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopGettingUpState : MallCopState {

    bool isGettingUp = false;

    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.GettingUp);
        Timing.RunCoroutine (RunStartupTimer());
    }

    public override void OnExit() {}

    public override void Update() {
        velBody.velocity = Vector3.zero;
    }

    public bool IsDoneGettingUp {
        get {
            return CanRestart();
        }
    }

    IEnumerator<float> RunStartupTimer() {
        isPastStartup = false;
        yield return Timing.WaitForSeconds(.2f);
        isPastStartup = true;
    }
    bool isPastStartup;

    public bool CanRestart() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f && isPastStartup;
    }
}
