//Uses 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class MallCopReanimatorFireState : AIState {

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
        animator.speed = animatorSpeed;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.ReadyFire);
        doneFiring = false;
        firstDetection = false;
        animator.SetLayerWeight(1, 0.0f);

    }
    public override void Update() {
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
        controller.transform.LookAt(lookAtPosition);
        navAgent.velocity = Vector3.zero;
        FreezePosition();
    }

    public override void OnExit() {
        navObstacle.enabled = false;
        navAgent.enabled = true;
        doneFiring = false;
        animator.speed = oldAnimatorSpeed;
        animator.SetLayerWeight(1, 0.0f);
    }

    public void HealWounded()
    {
        //Make sure to heal an enemy
        if (controller.AttackTarget != null && controller.AttackTarget.gameObject.tag == Strings.Layers.ENEMY && controller.AttackTarget.gameObject.GetInstanceID() != this.controller.gameObject.GetInstanceID())
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
        controller.AttackTarget = controller.FindPlayer();
        controller.UpdateState(EAIState.Chase);
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

    public void StopCoroutines()
    {
        Kill();
    }

}
