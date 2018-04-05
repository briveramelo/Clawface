//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class MallCopFireState : AIState {

    public float fireRange;
    public bool firstDetection;
    public float animatorSpeed;

    private float currentAngleToTarget;
    private float lastAngleToTarget;
    private float currentWeight;
    private Vector3 initialPosition;
    private bool doneFiring;
    private float oldAnimatorSpeed;

    public override void OnEnter() {
        initialPosition = controller.transform.position;
        navAgent.enabled = false;
        navObstacle.enabled = true;
        oldAnimatorSpeed = animator.speed;
        if (controller.DistanceFromTarget <= fireRange && !firstDetection)
        {
            animator.speed = animatorSpeed;
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
        }
        else
        {
            animator.speed = oldAnimatorSpeed;
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.ReadyFire);
        }
        doneFiring = false;
        firstDetection = false;
        animator.SetLayerWeight(1, 0.0f);

    }
    public override void Update() {
        currentWeight = animator.GetLayerWeight(1);
        currentAngleToTarget = CheckAngle();
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
        controller.transform.LookAt(lookAtPosition);
        navAgent.velocity = Vector3.zero;
        controller.transform.eulerAngles = new Vector3(0.0f,controller.transform.eulerAngles.y,0.0f);
        lastAngleToTarget = CheckAngle();
        CheckRotationDifference();
        FreezePosition();   
    }

    public override void OnExit() {
        navObstacle.enabled = false;
        navAgent.enabled = true;
        doneFiring = false;
        animator.speed = oldAnimatorSpeed;
        animator.SetLayerWeight(1, 0.0f);
    }

    private void CheckRotationDifference()
    {
        float difference = Mathf.Abs(currentAngleToTarget - lastAngleToTarget);

        if (difference >= 0.01f)
        {
            currentWeight = Mathf.Lerp(currentWeight, 1.0f, Time.deltaTime);
            animator.SetLayerWeight(1, currentWeight);
        }
        else if (difference < 0.01f)
        {
            currentWeight = Mathf.Lerp(currentWeight, 0.0f, Time.deltaTime * 0.5f);
            animator.SetLayerWeight(1, currentWeight);
        }
    }

    private float CheckAngle()
    {
        float angleToTarget = Vector3.Angle(controller.transform.forward, controller.DirectionToTarget);
        return angleToTarget;
    }

    private void FreezePosition()
    {        
        controller.transform.position = initialPosition;
        navAgent.nextPosition = controller.transform.position;
    }

    public void ReadyToFireDone()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
    }

    public void StopAiming()
    {
        doneFiring = true;
    }


    public void StartEndFire()
    {
        animator.speed = oldAnimatorSpeed;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.EndFire);
    }

    public void EndFireDone()
    {
        controller.UpdateState(EAIState.Chase);
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

    public void StopCoroutines()
    {
        Timing.KillCoroutines(coroutineName);
    }

}
