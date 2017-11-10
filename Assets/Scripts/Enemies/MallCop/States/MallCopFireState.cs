//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFireState : AIState {
    
    private float currentAngleToTarget;
    private float lastAngleToTarget;
    MallCopBlasterController blasterController;

    int counter;
    bool doneFiring;


    public override void OnEnter() {
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
        animator.SetInteger(Strings.FEETSTATE, (int)AnimationStates.TurnLeft);
        Timing.RunCoroutine(RunStartupTimer(), coroutineName);
        doneFiring = false;
        blasterController = (MallCopBlasterController)controller;
       
    }
    public override void Update() {

        currentAngleToTarget = CheckAngle();
        controller.transform.LookAt(controller.AttackTarget);
        //Adjust the gun nozzle to the player
        //controller.transform.RotateAround(controller.transform.position, controller.transform.up, -3.0f);

        CheckRotationDifference();
        navAgent.velocity = Vector3.zero;

        lastAngleToTarget = CheckAngle();
    }

    public override void OnExit() {
        navObstacle.enabled = false;
        navAgent.enabled = true;

        //blasterController.feetLayerAnimator.Stop(coroutineName);
        //blasterController.feetLayerAnimatorReverse.Stop(coroutineName);
        animator.SetLayerWeight(1, 0.0f);
        
    }


    IEnumerator<float> RunStartupTimer() {
        isPastStartup = false;
        yield return Timing.WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isPastStartup = true;
    }
    bool isPastStartup;

    public bool CanRestart() {

        if (isPastStartup)
        {
            doneFiring = true;
        }
        else
        {
            doneFiring = false;
        }

        return doneFiring;
    }

    private void CheckRotationDifference()
    {
        float difference = Mathf.Abs(currentAngleToTarget - lastAngleToTarget);

        if (difference >= 0.01f)// && !blasterController.feetLayerAnimator.IsAnimating)
        {
            //blasterController.feetLayerAnimatorReverse.Stop(coroutineName);
            //blasterController.feetLayerAnimator.startValue = animator.GetLayerWeight(1);
            //blasterController.feetLayerAnimator.diff = 1f-animator.GetLayerWeight(1);
            //blasterController.feetLayerAnimator.OnUpdate = (val) => {
                animator.SetLayerWeight(1, 1.0f);
            //};
            //blasterController.feetLayerAnimator.Animate(coroutineName);
        }
        else if (difference < 0.01f )//&& !blasterController.feetLayerAnimatorReverse.IsAnimating)
        {
            //blasterController.feetLayerAnimator.Stop(coroutineName);

            //blasterController.feetLayerAnimatorReverse.startValue = animator.GetLayerWeight(1);
            //blasterController.feetLayerAnimatorReverse.diff = 0f-animator.GetLayerWeight(1);
            //blasterController.feetLayerAnimatorReverse.OnUpdate = (val) =>
            //{
                animator.SetLayerWeight(1, 0.0f);
            //};
            //blasterController.feetLayerAnimatorReverse.Animate(coroutineName);
        }

       //Debug.Log(animator.GetLayerWeight(1));


    }

    private float CheckAngle()
    {        
        float angleToTarget = Vector3.Angle(controller.transform.forward, controller.DirectionToTarget);
        return angleToTarget;
    }



}
