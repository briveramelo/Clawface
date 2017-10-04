using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BouncerFireState : BouncerState {

    private bool doneFiring = false;
    private float firingWaitTime;

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
        doneFiring = false;
        firingWaitTime = properties.waitShotTime;
        Timing.RunCoroutine(RunStartupTimer(),coroutineName);
    }
    public override void Update()
    {
        controller.transform.LookAt(controller.AttackTarget);
        controller.transform.rotation = Quaternion.Euler(0.0f, controller.transform.rotation.y, controller.transform.rotation.z);
    }
    public override void OnExit()
    {

    }

    IEnumerator<float> RunStartupTimer()
    {
        bulletHellPattern.enabled = true;
        yield return Timing.WaitForSeconds(firingWaitTime);
        bulletHellPattern.enabled = false;
        doneFiring = true;
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

}
