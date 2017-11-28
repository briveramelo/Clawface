//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopFireState : AIState {

    private float currentAngleToTarget;
    private float lastAngleToTarget;
    private float currentWeight;
    private Vector3 initialPosition;

    public bool endFireDone;

    public override void OnEnter() {
        initialPosition = controller.transform.position;
        navAgent.enabled = false;
        navObstacle.enabled = true;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.ReadyFire);
        animator.SetInteger(Strings.FEETSTATE, (int)AnimationStates.TurnLeft);
        endFireDone = false;
        animator.SetLayerWeight(1, 0.0f);

    }
    public override void Update() {
        currentWeight = animator.GetLayerWeight(1);
        currentAngleToTarget = CheckAngle();
        controller.transform.LookAt(controller.AttackTarget);
        navAgent.velocity = Vector3.zero;
        lastAngleToTarget = CheckAngle();
        CheckRotationDifference();
        FreezePosition();
    }

    public override void OnExit() {
        navObstacle.enabled = false;
        navAgent.enabled = true;
        endFireDone = false;
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
    }

    public void ReadyToFireDone()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
    }

    public void StartEndFire()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.EndFire);
    }

    public void EndFireDone()
    {
        controller.UpdateState(EAIState.Chase);
    }

}
