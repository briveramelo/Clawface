using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MEC;

public class BouncerChaseState : AIState {

    private Damager damager = new Damager();
    private float jumpTargetDistance = 10f;
    private bool moving = false;
    private float height = 12.0f;
    private float tolerance = 0.05f;
    private int jumpCount = 0;
    private int maxJumpCount;
    private Vector3 finalPosition;
    private Vector3 targetPosition;
    private SpriteRenderer shadowOutline;
    private float originalShadowOutlineScale;
    float jumpPercentage = 0.0f;

    //Smooth Lerping
    float lerpTime = 1.0f;
    float currentLerpTime;


    public Vector3 jumpTarget;
    public bool doneStartingJump;
    public bool doneLandingJump;
    public bool gotStunned;

    public BouncerChaseState (SpriteRenderer shadowOutline)
    {
        this.shadowOutline = shadowOutline;
        originalShadowOutlineScale = shadowOutline.transform.localScale.x;
    }

    public override void OnEnter()
    {
        controller.AttackTarget = controller.FindPlayer();
        jumpCount = 0;

        Vector3 moveDirection = controller.DirectionToTarget;
        jumpTarget = controller.transform.position + (moveDirection.normalized * jumpTargetDistance);
        finalPosition = jumpTarget;
        maxJumpCount = Random.Range(properties.minBounces, properties.maxBounces);
        moving = false;
        doneStartingJump = false;
        doneLandingJump = false;
        gotStunned = false;
    }
    public override void Update()
    {
        Chase();
        
    }

    public override void OnExit()
    {
    }

    private void Chase()
    {        
        if (!moving)
        {
            GetNewChaseTarget();
        }

        else
        {
            controller.transform.eulerAngles = new Vector3(0.0f, controller.transform.eulerAngles.y, controller.transform.eulerAngles.z);
        }

        if (shadowOutline.gameObject.activeInHierarchy)
        {
            Vector3 targetPosition = Vector3.Lerp (shadowOutline.transform.position, finalPosition, 1.0f);
            targetPosition.y = shadowOutline.transform.position.y;
            shadowOutline.transform.position = targetPosition;
            float heightPercentage = Mathf.Sqrt(controller.transform.position.y / height);
            float scale = heightPercentage * originalShadowOutlineScale;
            //shadowOutline.SetAlpha (heightPercentage);
            shadowOutline.SetAlpha (jumpPercentage);
            shadowOutline.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void GetNewChaseTarget()
    {
        Vector3 moveDirection = controller.DirectionToTarget;
        jumpTarget = controller.transform.position + (moveDirection.normalized * jumpTargetDistance);

        Vector3 fwd = controller.DirectionToTarget;
        RaycastHit hit;
        //Do a ray cast to check there is no obstruction
        if (Physics.Raycast(controller.transform.position, fwd, out hit, Mathf.Infinity, LayerMask.GetMask(Strings.Layers.MODMAN, Strings.Layers.OBSTACLE)))
        {            
            if (hit.transform.tag == Strings.Tags.PLAYER)
            {                
                //Special case when a wall is behind the player
                if (Physics.Raycast(controller.AttackTargetPosition, fwd, out hit, 5))
                {                    
                    if (hit.transform.tag == Strings.Tags.WALL) {                        
                        if (Vector3.Distance(controller.transform.position, controller.AttackTargetPosition) < jumpTargetDistance) {                            
                            if (navAgent.SetDestination(controller.AttackTargetPosition + (-controller.DirectionToTarget.normalized) * 1.5f)) {
                                finalPosition = controller.AttackTargetPosition + (-controller.DirectionToTarget.normalized) * 1.5f;
                            }
                        }
                        else {                            
                            if (navAgent.SetDestination(jumpTarget)) {
                                finalPosition = jumpTarget;
                            }
                        }
                    }
                    else {                        
                        if (navAgent.SetDestination(controller.AttackTargetPosition)) {
                            finalPosition = controller.AttackTargetPosition;
                        }
                    }
                }
                else
                {                    
                    if (navAgent.SetDestination(jumpTarget))
                    {
                        finalPosition = jumpTarget;
                    }
                }
                
            }

            //Hit obstacle
            else
            {
                
                if (Vector3.Distance(controller.transform.position, controller.AttackTargetPosition) < jumpTargetDistance)
                {                    
                    if (navAgent.SetDestination(controller.AttackTargetPosition))
                    {
                        finalPosition = controller.AttackTargetPosition;
                    }
                }
                else
                {                    
                    Vector3 bouncerPos = new Vector3(controller.transform.position.x, controller.transform.position.y, controller.transform.position.z);
                    Vector3 closestPoint = hit.collider.ClosestPointOnBounds(bouncerPos);
                    float newDistance = Vector3.Distance(closestPoint, bouncerPos);

                    if (newDistance < jumpTargetDistance)
                    {                        
                        jumpTarget = controller.transform.position + (moveDirection.normalized * (jumpTargetDistance + hit.collider.bounds.extents.magnitude * 1.2f));

                        if (navAgent.SetDestination(jumpTarget))
                        {
                            finalPosition = jumpTarget;
                        }
                    }

                    else
                    {                        
                        if (navAgent.SetDestination(jumpTarget))
                        {
                            finalPosition = jumpTarget;
                        }
                    }
                }
                
            }
        }


        AIEnemyData testData = new AIEnemyData(controller.GetInstanceID(), finalPosition);
        if (AIManager.Instance != null)
        {

            if (AIManager.Instance.AssignPosition(testData))
            {
                //Debug.Log("Normal");
                moving = true;
                Timing.RunCoroutine(Move(), CoroutineName);
            }

            else
            {
                //Debug.Log("Readjusted");
                finalPosition = controller.transform.position + Random.insideUnitSphere * jumpTargetDistance / 2;
                finalPosition.y = controller.transform.position.y;
                moving = true;
                Timing.RunCoroutine(Move(), CoroutineName);
            }
        }

        else
        {
            //Debug.Log("Readjusted");
            finalPosition = controller.transform.position + Random.insideUnitSphere * jumpTargetDistance / 2;
            finalPosition.y = controller.transform.position.y;
            moving = true;
            Timing.RunCoroutine(Move(), CoroutineName);
        }
    }


    IEnumerator<float> Move()
    {
        navAgent.updatePosition = false;

        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Jumping);

        while (!doneStartingJump)
        {
           
            yield return 0.0f;
        }

        Vector3 initialPosition = controller.transform.position;
        targetPosition = new Vector3(finalPosition.x, finalPosition.y + 0.2f, finalPosition.z);

        Vector3 smoothMidpoint1 = initialPosition + ((targetPosition - initialPosition) * 0.2f);
        Vector3 midpoint = initialPosition + ((targetPosition - initialPosition) * 0.5f);
        Vector3 smoothMidpoint2 = initialPosition + ((targetPosition - initialPosition) * 0.8f);

        midpoint.y += height;
        smoothMidpoint1.y += height * 0.7f;
        smoothMidpoint2.y += height * 0.7f;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(initialPosition, smoothMidpoint1, myStats.moveSpeed*2.5f, 0.0f, 0.25f),CoroutineName));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(smoothMidpoint1, midpoint, myStats.moveSpeed * 2.0f, 0.25f, 0.5f) ,CoroutineName));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(midpoint, smoothMidpoint2, myStats.moveSpeed * 2.5f, 0.5f, 0.75f), CoroutineName));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(smoothMidpoint2, targetPosition, myStats.moveSpeed * 3.0f, 0.75f, 1.0f),CoroutineName));

        if (gotStunned)
        {
            controller.UpdateState(EAIState.Stun);
        }
        else
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.EndJump);

        while (!doneLandingJump)
        {
            yield return 0.0f;
        }
        doneStartingJump = false;
        doneLandingJump = false;
        jumpCount++;
        moving = false;

        //Fix to avoid teleportation
        navAgent.Warp(controller.transform.position);

        navAgent.updatePosition = true;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(controller.transform.position, navAgent.nextPosition, myStats.moveSpeed * 1.5f, 0.0f, 0.0f),CoroutineName));
    }


    private IEnumerator<float> LerpToNextPosition(Vector3 initialPosition, Vector3 targetPosition, float lerpSpeed, float startPercentage, float endPercentage)
    {
        float interpolation = 0.0f;
        currentLerpTime = 0.0f;

        while (interpolation< 1.0f)
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

            jumpPercentage = Mathf.Lerp (startPercentage, endPercentage, interpolation);

            yield return 0.0f;
        }
        controller.transform.position = targetPosition;
        interpolation = 1.0f;
    }


    public bool OverMaxJumpCount()
    {
        if (jumpCount >= maxJumpCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(myStats.attack, DamagerType.BlasterBullet, navAgent.transform.forward);
            damageable.TakeDamage(damager);
        }
    }

    public void StopCoroutines()
    {
        Timing.KillCoroutines(CoroutineName);

        Vector3 fwd = -controller.transform.up;
        RaycastHit hit;

        if (Physics.Raycast(controller.transform.position, fwd, out hit, Mathf.Infinity, LayerMask.GetMask(Strings.Layers.GROUND)))
        {
            if (hit.transform.tag == Strings.Tags.FLOOR)
            {
                controller.transform.position = hit.point;
            }
        }
    }
}
