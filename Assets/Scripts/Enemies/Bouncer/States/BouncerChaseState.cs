using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MovementEffects;

public class BouncerChaseState : AIState {

    private Damager damager = new Damager();
    private float jumpTargetDistance = 10f;
    private bool moving = false;
    private float height = 12.0f;
    private float tolerance = 0.1f;
    private int jumpCount = 0;
    private int maxJumpCount;
    private Vector3 finalPosition;

    //Smooth Lerping
    float lerpTime = 1.0f;
    float currentLerpTime;


    public Vector3 jumpTarget;
    public bool doneStartingJump;
    public bool doneLandingJump;
    public bool gotStunned;

    public override void OnEnter()
    {
        controller.AttackTarget = controller.FindPlayer();
        jumpCount = 0;
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
    }

    private void GetNewChaseTarget()
    {


        Vector3 moveDirection = controller.DirectionToTarget;
        jumpTarget = controller.transform.position + (moveDirection.normalized * jumpTargetDistance);

        Vector3 fwd = controller.DirectionToTarget;
        RaycastHit hit;
        //finalPosition = controller.AttackTargetPosition;
        //Do a ray cast to check there is no obstruction
        if (Physics.Raycast(controller.transform.position, fwd, out hit, Mathf.Infinity, LayerMask.GetMask(Strings.Layers.MODMAN, Strings.Layers.OBSTACLE)))
        {            
            if (hit.transform.tag == Strings.Tags.PLAYER)
            {                
                //Special case when a wall is behind the player
                if (Physics.Raycast(controller.AttackTargetPosition, fwd, out hit, 5, LayerMask.GetMask(Strings.Layers.GROUND)))
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
                    Vector3 bouncerPos = new Vector3(controller.transform.position.x, 0.0f, controller.transform.position.z);
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
                Debug.Log("Normal");
                moving = true;
                Timing.RunCoroutine(Move(), coroutineName);
            }

            else
            {
                Debug.Log("Readjusted");
                finalPosition = controller.transform.position + Random.insideUnitSphere * jumpTargetDistance/2;
                moving = true;
                Timing.RunCoroutine(Move(), coroutineName);
            }
        }

        else
        {
            Debug.Log("Readjusted");
            finalPosition = controller.transform.position + Random.insideUnitSphere * jumpTargetDistance/2;
            moving = true;
            Timing.RunCoroutine(Move(), coroutineName);
        }
    }


    IEnumerator<float> Move()
    {

        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Jumping);

        while (!doneStartingJump)
        {
            yield return 0.0f;
        }

        Vector3 initialPosition = controller.transform.position;
        Vector3 targetPosition = new Vector3(finalPosition.x, 0.2f, finalPosition.z);

        Vector3 smoothMidpoint1 = initialPosition + ((targetPosition - initialPosition) * 0.2f);
        Vector3 midpoint = initialPosition + ((targetPosition - initialPosition) * 0.5f);
        Vector3 smoothMidpoint2 = initialPosition + ((targetPosition - initialPosition) * 0.8f);

        midpoint.y += height;
        smoothMidpoint1.y += height * 0.7f;
        smoothMidpoint2.y += height * 0.7f;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(initialPosition, smoothMidpoint1, myStats.moveSpeed*2.5f), coroutineName));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(smoothMidpoint1, midpoint, myStats.moveSpeed * 2.0f), coroutineName));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(midpoint, smoothMidpoint2, myStats.moveSpeed * 2.5f), coroutineName));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LerpToNextPosition(smoothMidpoint2, targetPosition, myStats.moveSpeed * 3.0f), coroutineName));

        if(gotStunned)
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Stunned);
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
    }


    private IEnumerator<float> LerpToNextPosition(Vector3 initialPosition, Vector3 targetPosition, float lerpSpeed)
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

}
