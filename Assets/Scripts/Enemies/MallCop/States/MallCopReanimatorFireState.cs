
//Uses 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopReanimatorFireState : AIState {

    private float currentAngleToTarget;
    private float lastAngleToTarget;
    private float currentWeight;
    private Vector3 initialPosition;

    private bool doneFiring;
    public float fireRange;
    public bool firstDetection;


    public override void OnEnter() {
        initialPosition = controller.transform.position;
        navAgent.enabled = false;
        navObstacle.enabled = true;
        doneFiring = false;
        firstDetection = false;
        animator.SetLayerWeight(1, 0.0f);

    }
    public override void Update() {
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
        controller.transform.LookAt(lookAtPosition);
        navAgent.velocity = Vector3.zero;
        HealWounded();
        FreezePosition();
    }

    public override void OnExit() {
        navObstacle.enabled = false;
        navAgent.enabled = true;
        doneFiring = false;
        animator.SetLayerWeight(1, 0.0f);
    }

    private void HealWounded()
    {
        if (controller.AttackTarget != null)
        {
            controller.AttackTarget.GetComponent<EnemyBase>().ResetHealth();
        }

        doneFiring = true;
    }

    private void FreezePosition()
    {
        controller.transform.position = initialPosition;
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


}
