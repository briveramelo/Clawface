using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class ZombieBeserkerChaseState : AIState {

    private List<int> attacks;
    private bool doneAttacking = false;
    private float waitTimeToEnterAttack = 2.0f;

    public override void OnEnter()
    {
        doneAttacking = false;
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
        doneAttacking = false;
        Timing.KillCoroutines(coroutineName);
    }

    public void RandomAttack()
    {
        attacks = new List<int>();
        attacks.Add((int)AnimationStates.Attack5);
        attacks.Add((int)AnimationStates.Attack6);
        int animationAttackValue = attacks[Random.Range(0, attacks.Count)];
        animator.SetInteger("AttackValue", animationAttackValue);
        Timing.RunCoroutine(SetDoneAttacking(), coroutineName);
        
    }

    public bool DoneAttacking()
    {
        return doneAttacking;
    }

    private IEnumerator<float> SetDoneAttacking()
    {
        yield return Timing.WaitForSeconds(waitTimeToEnterAttack);
        doneAttacking = true;
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


}
