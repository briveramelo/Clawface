using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopSwingState : MallCopState {

    float animationTime;

    public override void OnEnter() {        
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Swing);
        velBody.velocity = Vector3.zero;        
    }
    public override void Update() {
        velBody.velocity = Vector3.zero;
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.99f) {
            controller.UpdateState(EMallCopState.Chase);
        }
    }
    public override void OnExit() {
        
    }
    
}
