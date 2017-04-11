//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModMan;

public class MallCopPatrolToPointsState : MallCopState
{

    [SerializeField] PatrolPoints patrolPoints;

    public override void OnEnter()
    {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Walk);
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
        velBody.transform.LookAt(patrolPoints.currentTarget);
        velBody.transform.rotation = Quaternion.Euler(0f, velBody.transform.rotation.eulerAngles.y, 0f);
        Vector3 movementDirection = (patrolPoints.currentTarget.position - velBody.transform.position).normalized;
        velBody.velocity = movementDirection * myStats.moveSpeed * Time.deltaTime;

        if (Vector3.Distance(controller.transform.position, patrolPoints.currentTarget.position) < 0.1f)
        {
            patrolPoints.UpdateTarget();
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
   
}
