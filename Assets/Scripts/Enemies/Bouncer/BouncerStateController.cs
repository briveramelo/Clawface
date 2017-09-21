using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class BouncerStateController : BouncerController
{

    [SerializeField] SphereCollider playerDetectorSphereCollider;
    [SerializeField] float closeEnoughToAttackDistance;
    private Collider[] playerColliderList = new Collider[10];
    float maxDistanceBeforeChasing = 2.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerDetectorSphereCollider.transform.position, maxDistanceBeforeChasing);
    }


    void Awake()
    {
        checksToUpdateState = new List<Func<bool>>() {
            CheckToAttack,
            CheckToPatrol,
            CheckToFinishAttacking
        };
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.CompareTag(Strings.Tags.PLAYER) && CurrentState != states.chase && CurrentState != states.fire)
        //{

        //    AttackTarget = other.transform;
        //    UpdateState(EBouncerState.Chase);
        //}
    }



    bool CheckToAttack()
    {
        if (CurrentState == states.chase && distanceFromTarget < closeEnoughToAttackDistance)
        {
            UpdateState(EBouncerState.Fire);
            return true;
        }
        return false;
    }

    bool CheckToPatrol()
    {
        if (AttackTarget == null)
        {

            UpdateState(EBouncerState.Patrol);
            return true;
        }
        return false;
    }

    bool CheckToFinishAttacking()
    {
        if (CurrentState == states.fire)
        {

            bool shouldChase = distanceFromTarget > maxDistanceBeforeChasing;

            if (shouldChase)
            {
                UpdateState(EBouncerState.Chase);
            }
            else
            {
                UpdateState(EBouncerState.Fire);
            }
            return true;
        }
        return false;
    }


}

