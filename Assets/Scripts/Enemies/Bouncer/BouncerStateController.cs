using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModMan;

public class BouncerStateController : BouncerController
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
            CheckToAttack,
            CheckToFinishAttacking
        };
    }

    protected override void Update()
    {
        base.Update();
    }

    bool CheckToAttack()
    {
        if (CurrentState == states.chase && states.chase.OverMaxJumpCount())
        {
            UpdateState(EBouncerState.Fire);
            return true;
        }
        return false;
    }

    bool CheckToFinishAttacking()
    {
        if (CurrentState == states.fire)
        {

            if (states.fire.DoneFiring())
            {
                UpdateState(EBouncerState.Chase);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }


}

