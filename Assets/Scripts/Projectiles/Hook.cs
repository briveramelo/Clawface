using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;

public class Hook : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Transform startingPoint;
    [SerializeField] private Transform endingPoint;
    #endregion

    #region Private Fields
    private GrapplerMod.HookProperties hookProperties;    
    private Transform target;
    private bool isTargetHit;
    private int enemyCount;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        
    }

    private void OnDisable()
    {
        ResetToDefaults();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        //Are hook properties set and have we not hit a target
        if (hookProperties != null && !isTargetHit)
        {
            //Check for max distance
            if(CalculateChainLength() >= hookProperties.maxDistance)
            {
                gameObject.SetActive(false);
                return;
            }
            //Move forward
            endingPoint.position = endingPoint.position + (endingPoint.forward * hookProperties.projectileSpeed);
            //Check for target if no target
            if (target == null || (target != null && !target.gameObject.activeSelf))
            {
                target = null;
                CheckForTargets();
            }
            else
            {
                // Look at target
                endingPoint.LookAt(target);
                // Ensure the forward vector is in 2D
                endingPoint.forward = new Vector3(endingPoint.forward.x, 0f, endingPoint.forward.z);
            }
        }
    }

    private void CheckForTargets()
    {
        //Sphere cast to get all enemies
        RaycastHit[] hits = Physics.SphereCastAll(endingPoint.position, hookProperties.homingDistance, Vector3.zero, LayerMask.GetMask(Strings.Layers.ENEMY));
        if(hits != null && hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                //Check if enemy is within homing angle
                float angle = Mathf.Abs(Vector3.Angle(endingPoint.forward, hit.transform.position));
                if(angle < hookProperties.homingAngle / 2f)
                {
                    // Acquire target
                    target = hit.transform;
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Is the collided object an enemy
        if(other.tag == Strings.Tags.ENEMY)
        {
            //Set bool
            isTargetHit = true;
            //Parent endpoint to enemy
            endingPoint.SetParent(other.gameObject.transform);
            //Increment enemycount
            enemyCount++;
            //Check for max enemies
            if (enemyCount < hookProperties.maxChainableEnemies)
            {
                //Spawn next chain
                SpawnNextChain();
            }
        }
    }
    #endregion

    #region Public Methods
    public void Init(GrapplerMod.HookProperties properties, Transform hookTransform, int enemyCount = 0)
    {
        hookProperties = properties;
        transform.position = hookTransform.position;
        transform.forward = hookTransform.forward;
        this.enemyCount = enemyCount;
    }
    #endregion

    #region Private Methods
    private void ResetToDefaults()
    {
        hookProperties = null;
        target = null;
        isTargetHit = false;
        enemyCount = 0;
        startingPoint.localPosition = Vector3.zero;
        endingPoint.SetParent(transform);
        endingPoint.localPosition = Vector3.zero;
    }

    private float CalculateChainLength()
    {
       return Vector3.Distance(startingPoint.position, endingPoint.position);
    }
    
    private void SpawnNextChain()
    {
        GrapplerMod.HookProperties newProperties = hookProperties;
        newProperties.maxDistance -= CalculateChainLength();
        //Spawn enemy
    }
    #endregion

    #region Private Structures
    #endregion

}
