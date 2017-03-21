using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModMan;

public class MallCopPatrolState : MallCopState {

    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Walk);
    }
    public override void Update() {
        Patrol();
    }
    public override void OnExit() {

    }

    private void Patrol() {        
        if (controller.timeInLastState > properties.walkTime) {
            GetNewPatrolTarget();
        }

        velBody.transform.LookAt(walkTarget);
        velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);
        Vector3 movementDirection = (walkTarget - velBody.transform.position).normalized;
        velBody.velocity = movementDirection * myStats.moveSpeed * Time.deltaTime;

        if (IsHittingWall(movementDirection, 2f)) {
            GetNewPatrolTarget();
        }
    }

    private bool IsHittingWall(Vector3 movementDirection, float checkDistance) {
        Ray raycast = new Ray(velBody.foot.position, movementDirection);
        List<RaycastHit> rayCastHits = new List<RaycastHit>(Physics.RaycastAll(raycast, checkDistance));
        if (rayCastHits.Any(hit => (
            hit.collider.tag == Strings.Tags.UNTAGGED ||
            hit.collider.tag == Strings.Tags.ENEMY ||
            hit.transform.gameObject.layer == (int)Layers.Ground))) {

            return true;
        }
        return false;
    }
    Vector3 walkTarget;

    private void GetNewPatrolTarget() {

        int numRayChecks = 20;
        float randStart = Random.Range(0, 360f);
        int clockwise = Random.value > 0.5f ? 1 : -1;
        for (int i = 0; i < numRayChecks; i++) {
            float angle = randStart + clockwise * i * (360f / numRayChecks);
            Vector3 moveDirection = angle.ToVector3();
            if (!IsHittingWall(moveDirection, 6f)) {

                moveDirection = moveDirection.normalized * 50f;
                moveDirection.y = .1f;
                walkTarget = velBody.foot.position + moveDirection;
                controller.RestartStateTimer();
                break;
            }
        }

    }
}
