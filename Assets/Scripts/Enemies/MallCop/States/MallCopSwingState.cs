using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopSwingState : MallCopState {

    float animationTime;

    public override void OnEnter() {        
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Swing);
    }
    public override void Update() {
        velBody.velocity = Vector3.zero;        
    }
    public override void OnExit() {
        
    }
    
}
