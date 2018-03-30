using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BouncerFireState : AIState {

    public bool doneFiring = false;
    public int shotCount = 0;
    public int maxShots;
    public float animatorSpeed;

    private Vector3 freezePosition;
    private float rotation;
    private float oldAnimatorSpeed;
    private float tolerance = 0.001f;
    private bool lerping = true;

    //Smooth Lerping
    float lerpTime = 1.0f;
    float currentLerpTime;

    public override void OnEnter()
    {
        shotCount = 0;
        navAgent.updatePosition = true;
        lerping = true;
        Timing.RunCoroutine(LerpToNextPosition(controller.transform.position, navAgent.nextPosition, myStats.moveSpeed * 3.5f),coroutineName);
        maxShots = Random.Range(properties.minShots, properties.maxShots);
        rotation = controller.transform.eulerAngles.y;
        oldAnimatorSpeed = animator.speed;
        animator.speed = animatorSpeed;
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Fire1);
        doneFiring = false;
    }
    public override void Update()
    {
        if (properties.rotate)
        {
            DoRotationPattern();
        }
       
        if(!lerping)
            FreezePosition();
    }
    public override void OnExit()
    {
        animator.speed = oldAnimatorSpeed;
        shotCount = 0;
    }

    public bool DoneFiring()
    {
        return doneFiring;
    }

    private void FreezePosition()
    {
        controller.transform.position = freezePosition;
    }

    private void DoRotationPattern()
    {
        rotation += Time.deltaTime * properties.rotationSpeed;
        controller.transform.eulerAngles = new Vector3(0.0f,rotation,0.0f);
    }

    private IEnumerator<float> LerpToNextPosition(Vector3 initialPosition, Vector3 targetPosition, float lerpSpeed)
    {
        float interpolation = 0.0f;
        currentLerpTime = 0.0f;

        while (interpolation < 1.0f)
        {
            currentLerpTime += Time.deltaTime * lerpSpeed;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            interpolation = currentLerpTime / lerpTime;

            controller.transform.position = Vector3.Lerp(initialPosition, targetPosition, interpolation);

            if (Vector3.Distance(controller.transform.position, targetPosition) < tolerance)
            {
                controller.transform.position = targetPosition;
                break;
            }

            yield return 0.0f;
        }
        controller.transform.position = targetPosition;
        interpolation = 1.0f;

        freezePosition = controller.transform.position;
        lerping = false;

    }

    public void FireBullet()
    {
        bulletPatternController.FireBullet();
    }



}
