using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BouncerFireState : AIState {

    private bool doneFiring = false;
    private float firingWaitTime;
    private Vector3 initialPosition;

    private float rotation;
    private float rotationSpeed = 50.0f;


    public override void OnEnter()
    {
        initialPosition = controller.transform.position;
        rotation = controller.transform.eulerAngles.y;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Idle);
        doneFiring = false;
        firingWaitTime = properties.waitShotTime;
        Timing.RunCoroutine(RunStartupTimer(), coroutineName);
    }
    public override void Update()
    {
        if (properties.rotate)
        {
            DoRotationPattern();
        }

        FreezePosition();



    }
    public override void OnExit()
    { 
    }


    IEnumerator<float> RunStartupTimer()
    {
        bulletPatternController.enabled = true;
        yield return Timing.WaitForSeconds(firingWaitTime);
        bulletPatternController.enabled = false;
        doneFiring = true;
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

    private void FreezePosition()
    {
        controller.transform.position = initialPosition;
    }

    void DoRotationPattern()
    {
        rotation += Time.deltaTime * rotationSpeed;

        controller.transform.eulerAngles = new Vector3(0.0f,rotation,0.0f);
       
    }

}
