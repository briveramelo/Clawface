using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class ZombieStateController : ZombieController {

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
            CheckToFinishAttacking
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
            UpdateState(EZombieState.Chase);
        }
    }



    bool CheckToAttack()
    {
        if(CurrentState == states.chase && distanceFromTarget < closeEnoughToAttackDistance)
        {
            UpdateState(EZombieState.Attack);
            return true;
        }
        return false;
    }

    bool CheckToFinishAttacking()
    {
        if (CurrentState == states.attack)
        {

            bool shouldChase = distanceFromTarget > maxDistanceBeforeChasing;
                UpdateState(EZombieState.Chase);
            return true;
        }
        return false;
    }


}
