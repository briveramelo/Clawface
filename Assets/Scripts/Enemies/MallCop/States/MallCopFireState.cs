//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFireState : MallCopState {

    public override void OnEnter() {
        Timing.RunCoroutine(RunStartupTimer());        
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
    }
    public override void Update() {
        controller.transform.LookAt(controller.AttackTarget);
        navAgent.velocity = Vector3.zero;              
    }
    public override void OnExit() {
        
    }

    IEnumerator<float> RunStartupTimer() {
        isPastStartup = false;
        yield return Timing.WaitForSeconds(.2f);
        isPastStartup = true;
    }
    bool isPastStartup;

    public bool CanRestart() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >0.99f && isPastStartup;
    }

}
