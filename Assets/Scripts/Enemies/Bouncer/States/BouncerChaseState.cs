using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MovementEffects;

public class BouncerChaseState : AIState {

    private Vector3 jumpTarget;
    private float jumpTargetDistance = 10f;
    private bool moving = false;
    private float height = 12.0f;
    private float tolerance = 0.35f;
    private int jumpCount = 0;
    private int maxJumpCount;
    private NavMeshHit hit;
    private Vector3 finalPosition;


    public bool doneStartingJump;
    public bool doneLandingJump;


    public override void OnEnter()
    {

        controller.AttackTarget = controller.FindPlayer();
        jumpCount = 0;
        maxJumpCount = properties.bounces;
        moving = false;
        doneStartingJump = false;
        doneLandingJump = false;
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

    private bool IsHittingWall(Vector3 movementDirection, float checkDistance)
    {
        Vector3 rayOrigin = new Vector3(controller.transform.position.x, controller.transform.position.y + 0.5f, controller.transform.position.z);
        Ray raycast = new Ray(rayOrigin, movementDirection);
        List<RaycastHit> rayCastHits = new List<RaycastHit>(Physics.RaycastAll(raycast, checkDistance));
        if (rayCastHits.Any(hit => (
            hit.collider.tag == Strings.Tags.UNTAGGED ||
            hit.collider.tag == Strings.Tags.ENEMY ||
            hit.transform.gameObject.layer == (int)Layers.Ground)))
        {
            return true;
        }
        return false;
    }


    private void GetNewChaseTarget()
    {
            bool gotChasePoint = false;
            Vector3 moveDirection = controller.DirectionToTarget;
            jumpTarget = controller.transform.position + (moveDirection.normalized * jumpTargetDistance);
            
            if (navAgent.SetDestination(jumpTarget))
            {
                finalPosition = jumpTarget;
                gotChasePoint = true;
            }

            if (gotChasePoint)
            {
            moving = true;
            Timing.RunCoroutine(Move(), coroutineName);
        }
    }


    IEnumerator<float> Move()
    {

        animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.StartingJump);

        while (!doneStartingJump)
        {
            yield return 0.0f;
        }

        Vector3 initialPosition = controller.transform.position;
        Vector3 targetPosition = new Vector3(finalPosition.x, 0.2f, finalPosition.z);

        Vector3 midpoint = (initialPosition + targetPosition) * 0.5f;
        midpoint.y += height;


        float interpolation = 0.0f;

        while (interpolation < 1.0f)
        {

            animator.SetInteger(Strings.ANIMATIONSTATE, (int)AnimationStates.Jumping);

            interpolation += Time.deltaTime * myStats.moveSpeed;

            controller.transform.position = Vector3.Lerp(initialPosition, midpoint, interpolation);

            if (Vector3.Distance(controller.transform.position, midpoint) < tolerance)
            {
                controller.transform.position = midpoint;
                break;
            }

            yield return 0.0f;
        }
        controller.transform.position = midpoint;

        interpolation = 0.0f;

        while (interpolation < 1.0f)
        {

            interpolation += Time.deltaTime * myStats.moveSpeed;

            controller.transform.position = Vector3.Lerp(midpoint, targetPosition, interpolation);

            if (Vector3.Distance(controller.transform.position, targetPosition) < tolerance)
            {
                controller.transform.position = targetPosition;
                
                break;
            }
               

            yield return 0.0f;
        }

        controller.transform.position = targetPosition;
        interpolation = 1.0f;

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
}
