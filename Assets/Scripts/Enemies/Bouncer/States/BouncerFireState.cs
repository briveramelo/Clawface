using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BouncerFireState : BouncerState {

    private bool doneFiring = false;


    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Fire);
        doneFiring = false;
        Timing.RunCoroutine(RunStartupTimer());
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
        yield return Timing.WaitForSeconds(2.0f);
        bulletHellPattern.enabled = false;
        doneFiring = true;
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

}
