using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAciderChaseState : AIState {

    public TrailRenderer trailRenderer;
    public ColliderGenerator colliderGenerator;
    public bool needToClearTrail;

    public override void OnEnter()
    {
        if (needToClearTrail)
        {
            trailRenderer.Clear();
            needToClearTrail = false;
        }
        trailRenderer.enabled = true;
        colliderGenerator.enabled = true;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Walk);
        controller.AttackTarget = controller.FindPlayer();
        navAgent.speed = myStats.moveSpeed;
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
        if (Vector3.Distance(controller.transform.position, controller.AttackTarget.transform.position) < 10.0f)
        {
        Vector3 lookAtTarget = new Vector3(controller.AttackTarget.transform.position.x, controller.transform.position.y, controller.AttackTarget.transform.position.z);
        controller.transform.LookAt(lookAtTarget);
        }

        navAgent.SetDestination(controller.AttackTarget.position);
    }


}
