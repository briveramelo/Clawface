//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopChaseState : MallCopState {    

    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);        
    }
    public override void Update() {
        Chase();        
    }
    public override void OnExit() {

    }

    void Chase() {
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, 0, controller.AttackTarget.position.z);
        velBody.transform.LookAt(lookAtPosition);
        velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);
                
        float targetSpeed = myStats.moveSpeed * properties.runMultiplier * Time.fixedDeltaTime;
        velBody.velocity = controller.directionToTarget * targetSpeed;        
    }
}
