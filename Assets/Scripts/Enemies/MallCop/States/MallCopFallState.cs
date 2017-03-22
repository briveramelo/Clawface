using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopFallState : MallCopState {


    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Stunned);        
        velBody.isKinematic = true;
        controller.GetComponent<CapsuleCollider>().enabled = false;
    }
    public override void Update() {
        
    }
    public override void OnExit() {

    }
    
}
