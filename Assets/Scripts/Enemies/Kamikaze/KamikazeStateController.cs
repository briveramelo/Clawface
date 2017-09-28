using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class KamikazeStateController : KamikazeController
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
            CheckToSelfDestruct
        };
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.Tags.PLAYER))
        {

            AttackTarget = other.transform;
            UpdateState(EKamikazeState.Chase);
        }
    }

    bool CheckToSelfDestruct()
    {
        if (CurrentState == states.chase && distanceFromTarget < closeEnoughToAttackDistance)
        {
            UpdateState(EKamikazeState.SelfDestruct);
            return true;
        }
        return false;
    }



}
