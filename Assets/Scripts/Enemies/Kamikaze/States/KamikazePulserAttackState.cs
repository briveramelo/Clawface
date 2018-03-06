using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class KamikazePulserAttackState : AIState {

    private PulseGenerator currentPulseGenerator;
    private bool attackDone = false;

    public override void OnEnter()
    {
        currentPulseGenerator = null;
        navAgent.enabled = false;
        navObstacle.enabled = true;
        attackDone = false;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Attack1);
        Attack();
    }
    public override void Update()
    {
        Vector3 lookAtPosition = new Vector3(controller.AttackTarget.position.x, controller.transform.position.y, controller.AttackTarget.position.z);
        controller.transform.LookAt(lookAtPosition);
        navAgent.velocity = Vector3.zero;
        CheckPulseDone();
    }
    public override void OnExit()
    {
        attackDone = false;
        navObstacle.enabled = false;
        navAgent.enabled = true;
        currentPulseGenerator = null;
    }

    private void Attack()
    {
        //Make sure the kamikaze is not stunned
        if (myStats.health <= myStats.skinnableHealth)
        {
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
        }

        GameObject pulseGenerator = ObjectPool.Instance.GetObject(PoolObjectType.KamikazePulseGenerator);
        if (pulseGenerator) {
            pulseGenerator.transform.position = controller.transform.position;
            currentPulseGenerator = pulseGenerator.GetComponent<PulseGenerator>();
            currentPulseGenerator.SetPulseGeneratorStats(properties.maxPulses,properties.pulseRate,properties.scaleRate,properties.maxScale,myStats.attack);
        }

        SFXManager.Instance.Play(SFXType.KamikazePulse, velBody.transform.position);
    }

    private void CheckPulseDone()
    {
        //Make sure the kamikaze is not stunned
        if (myStats.health <= myStats.skinnableHealth)
        {
            controller.UpdateState(EAIState.Stun);
            controller.DeActivateAI();
        }

        if (currentPulseGenerator.gameObject != null && currentPulseGenerator.DonePulsing())
        {
            currentPulseGenerator.gameObject.SetActive(false);
            attackDone = true;
        }

        
    }

    public bool DoneAttacking()
    {
        return attackDone;
    }


}
