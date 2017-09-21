using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModMan;
using MovementEffects;

public class BouncerPatrolState : BouncerState {

    private Vector3 walkTarget;
    private float walkTargetDistance = 20f;
    private bool moving = false;
    private float firingAngle;
    private float gravity = 9.8f;

    public override void OnEnter()
    {
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

        if (IsHittingWall(movementDirection, 2f))
        {
            GetNewPatrolTarget();
        }

        Timing.RunCoroutine(Move());
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

                moveDirection = moveDirection.normalized;
                moveDirection.y = .1f;
                walkTarget = velBody.foot.position + moveDirection * walkTargetDistance;
                controller.RestartStateTimer();
                break;
            }
        }

    }

    IEnumerator<float> Move()
    {
        yield return Timing.WaitForSeconds(0.0f);

        Vector3 initialPosition = velBody.transform.position;
        Vector3 targetPosition = new Vector3 (walkTarget.x, 0.0f,walkTarget.z);

        float target_Distance = Vector3.Distance(velBody.transform.position, targetPosition);

        float movementVelocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad));

        float Vx = Mathf.Sqrt(movementVelocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(movementVelocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        float flightTime = target_Distance / Vx;

        float elapsedTime = 0;

        while (elapsedTime < flightTime)
        {

            velBody.transform.Translate(0, (Vy - (gravity * elapsedTime)) * Time.deltaTime, Vx * Time.deltaTime);
            elapsedTime += Time.deltaTime;

            yield return Timing.WaitForSeconds(0.0f);
        }

    }
}
