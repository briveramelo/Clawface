﻿//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopSwingState : MallCopState {

    public override void OnEnter() {
        controller.StartCoroutine(RunStartupTimer());
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Swing);
    }
    public override void Update() {
        velBody.velocity = Vector3.zero;
        velBody.LookAt(controller.attackTarget);        
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
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f && isPastStartup;
    }

}
