using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseState : ZombieState {

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);
        navAgent.speed = myStats.moveSpeed * properties.runMultiplier;
    }
    public override void Update()
    {
        Chase();
    }
    public override void OnExit()
    {

    }

    private void Chase()
    {
        //Orient zombie towards player
        if (Vector3.Distance(controller.AttackTarget.position, velBody.transform.position) > 100.0f)
        {
            Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, 0, controller.AttackTarget.position.z);
            velBody.transform.LookAt(lookAtPosition);
            velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);
        }
        navAgent.SetDestination(controller.AttackTarget.position);
    }


}
