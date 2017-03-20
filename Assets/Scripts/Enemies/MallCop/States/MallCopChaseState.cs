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
        Vector3 lookAtPosition = new Vector3(controller.attackTarget.transform.position.x, 0, controller.attackTarget.transform.position.z);
        velBody.transform.LookAt(lookAtPosition);
        velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);
        float distanceToPlayer = Vector3.Distance(velBody.transform.position, controller.attackTarget.transform.position);
        bool inStrikingDistance = distanceToPlayer < properties.strikingDistance;

        if (inStrikingDistance) {
            controller.UpdateState(EMallCopState.Swing);
        }
        else {            
            Vector3 movementDirection = controller.attackTarget.transform.position - velBody.transform.position;
            Vector3 movementDirectionXZ = new Vector3(movementDirection.x, 0, movementDirection.z);
            float targetSpeed = myStats.moveSpeed * properties.runMultiplier * Time.deltaTime;
            velBody.velocity = movementDirectionXZ.normalized * targetSpeed;
        }
    }
}
