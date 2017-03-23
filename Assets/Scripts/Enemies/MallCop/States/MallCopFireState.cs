//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopFireState : MallCopState {

    public override void OnEnter() {
        controller.StartCoroutine(RunStartupTimer());        
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
    }
    public override void Update() {
        velBody.LookAt(controller.attackTarget);
        velBody.velocity = Vector3.zero;              
    }
    public override void OnExit() {
        
    }

    IEnumerator RunStartupTimer() {
        isPastStartup = false;
        yield return new WaitForSeconds(.2f);
        isPastStartup = true;
    }
    bool isPastStartup;

    public bool CanRestart() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >0.99f && isPastStartup;
    }

}
