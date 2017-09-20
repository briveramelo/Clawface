using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModMan;

public class ZombiePatrolState : ZombieState {

    private Vector3 walkTarget;
    private float walkTargetDistance = 50f;

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Walk);
        navAgent.speed = myStats.moveSpeed;
    }
    public override void Update()
    {
        Patrol();
    }
    public override void OnExit()
    {

    }

    private void Patrol()
    {

        if (controller.timeInLastState > properties.walkTime)
        {
            GetNewPatrolTarget();
        }

        Vector3 movementDirection = (walkTarget - velBody.transform.position).normalized;
        navAgent.SetDestination(movementDirection* walkTargetDistance);

        if (IsHittingWall(movementDirection, 2f))
        {
            GetNewPatrolTarget();
        }
    }

    private bool IsHittingWall(Vector3 movementDirection, float checkDistance)
    {

        Ray raycast = new Ray(velBody.foot.position, movementDirection);
        List<RaycastHit> rayCastHits = new List<RaycastHit>(Physics.RaycastAll(raycast, checkDistance));
        if (rayCastHits.Any(hit => (
            hit.collider.tag == Strings.Tags.UNTAGGED ||
            hit.collider.tag == Strings.Tags.ENEMY ||
            hit.transform.gameObject.layer == (int)Layers.Ground)))
        {
            return true;
        }
        return false;
    }


    private void GetNewPatrolTarget()
    {

        int numRayChecks = 20;
        float randStart = Random.Range(0, 360f);
        int clockwise = Random.value > 0.5f ? 1 : -1;
        for (int i = 0; i < numRayChecks; i++)
        {
            float angle = randStart + clockwise * i * (360f / numRayChecks);
            Vector3 moveDirection = angle.ToVector3();
            if (!IsHittingWall(moveDirection, 6f))
            {

                moveDirection = moveDirection.normalized * 50f;
                moveDirection.y = .1f;
                walkTarget = velBody.foot.position + moveDirection;
                controller.RestartStateTimer();
                break;
            }
        }

    }


}
