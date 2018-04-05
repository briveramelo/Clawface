using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAciderChaseState : AIState {

    public float trailRendererTime;
    public float trailRendererStartWidth;

    public float colliderGenerationTime;
    public float acidTriggerLife;

    public ColliderGenerator colliderGenerator;
    public TrailRenderer trailRenderer;
    public GameObject acidObject;

    public override void OnEnter()
    {
        if (acidObject == null)
        {
            acidObject = ObjectPool.Instance.GetObject(PoolObjectType.AcidTrail);
            colliderGenerator = acidObject.GetComponent<ColliderGenerator>();
            colliderGenerator.SetZombieAciderParent(controller.gameObject.GetComponent<ZombieAcider>());
            trailRenderer = acidObject.GetComponent<TrailRenderer>();
            colliderGenerator.SetTrailRenderer(trailRenderer);
            colliderGenerator.SetStats(colliderGenerationTime, acidTriggerLife);
            trailRenderer.time = trailRendererTime;
            trailRenderer.startWidth = trailRendererStartWidth;
        }

        colliderGenerator.enabled = true;
        trailRenderer.enabled = true;
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

    public void ResetAcidObject()
    {
        acidObject = null;
    }
}
