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
        
        Vector3 movementDirection = controller.AttackTarget.position - velBody.transform.position;
        Vector3 movementDirectionXZ = new Vector3(movementDirection.x, 0, movementDirection.z);
        float targetSpeed = Mathf.Clamp(myStats.moveSpeed * properties.runMultiplier * Time.deltaTime, 0f, 7f);
        velBody.velocity = movementDirectionXZ.normalized * targetSpeed;        
    }
}
