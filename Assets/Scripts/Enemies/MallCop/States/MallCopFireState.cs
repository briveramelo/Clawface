//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFireState : AIState {

    private float oldRotationY;
    private float newRotationY;


    public override void OnEnter() {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        Timing.RunCoroutine(RunStartupTimer());
        oldRotationY = controller.transform.rotation.y;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
    }
    public override void Update() {
        controller.transform.LookAt(controller.AttackTarget);
        newRotationY = controller.transform.rotation.y;
        CheckRotationDifference();
        oldRotationY = controller.transform.rotation.y;
        navAgent.velocity = Vector3.zero;
    }

    

    public override void OnExit() {
        navObstacle.enabled = false;
        navAgent.enabled = true;
    }

    IEnumerator<float> RunStartupTimer() {
        isPastStartup = false;
        yield return Timing.WaitForSeconds(.2f);
        isPastStartup = true;
    }
    bool isPastStartup;

    public bool CanRestart() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >0.99f && isPastStartup;
    }

    private void CheckRotationDifference()
    {
        if (newRotationY > oldRotationY)
        {
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.TurnRight);
        }

        if (newRotationY < oldRotationY)
        {
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.TurnLeft);
        }

        if (newRotationY == oldRotationY)
        {
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
        }
    }


}
