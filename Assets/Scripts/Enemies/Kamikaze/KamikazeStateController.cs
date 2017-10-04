using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class KamikazeStateController : KamikazeController
{

    [SerializeField] float closeEnoughToAttackDistance;
    private Collider[] playerColliderList = new Collider[10];
    float maxDistanceBeforeChasing = 2.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToAttackDistance);
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
