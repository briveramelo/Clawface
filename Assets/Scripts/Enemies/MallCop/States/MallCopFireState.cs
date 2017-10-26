//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFireState : AIState {

    public override void OnEnter() {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        Timing.RunCoroutine(RunStartupTimer());        
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
    }
    public override void Update() {
        controller.transform.LookAt(controller.AttackTarget);
        navAgent.velocity = Vector3.zero;
    }
    public override void OnExit() {
        navAgent.enabled = true;
        navObstacle.enabled = false;
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
