using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BouncerFireState : AIState {

    public bool doneFiring = false;
    public int shotCount = 0;
    public int maxShots;
    public float animatorSpeed;

    private Vector3 initialPosition;
    private float rotation;
    private float oldAnimatorSpeed;

    public override void OnEnter()
    {
        shotCount = 0;
        maxShots = Random.Range(properties.minShots, properties.maxShots);
        initialPosition = controller.transform.position;
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
        controller.transform.position = initialPosition;
    }

    void DoRotationPattern()
    {
        rotation += Time.deltaTime * properties.rotationSpeed;
        controller.transform.eulerAngles = new Vector3(0.0f,rotation,0.0f);
       
    }

    public void FireBullet()
    {
        bulletPatternController.FireBullet();
    }

}
