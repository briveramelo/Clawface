using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BouncerFireState : AIState {

    private bool doneFiring = false;
    private float firingWaitTime;

    public override void OnEnter()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Idle);
        doneFiring = false;
        firingWaitTime = properties.waitShotTime;
        Timing.RunCoroutine(RunStartupTimer(), coroutineName);
    }
    public override void Update()
    {
        controller.transform.LookAt(controller.AttackTarget);
        controller.transform.rotation = Quaternion.Euler(0.0f, controller.transform.rotation.y, controller.transform.rotation.z);
    }
    public override void OnExit()
    { 
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }


    IEnumerator<float> RunStartupTimer()
    {
        bulletPatternController.enabled = true;
        yield return Timing.WaitForSeconds(firingWaitTime);
        bulletPatternController.enabled = false;
        doneFiring = true;
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

}
