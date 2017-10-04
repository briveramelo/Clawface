﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;

public class LightningProjectile : MonoBehaviour {
    
    #region Private Fields
    private LightningGun.ProjectileProperties projectileProperties;    
    private Transform target;
    private int enemyCount;
    private LightningProjectile nextHook;
    private List<Transform> ignoreTargets;
    private Damager damager;
    private Vector3 startingPosition;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {        
        ignoreTargets = new List<Transform>();
    }

    // Use this for initialization
    void Start () {
        
    }

    private void OnDisable()
    {
        ResetToDefaults();
    }

    private void Update()
    {
        //Are hook properties set and have we not hit a target
        if (projectileProperties != null)
        {
            //Check for target if no target
            if (!target || (target && !target.gameObject.activeSelf))
            {
                target = null;
                CheckForTargets();
            }
            else
            {
                // Look at target
                transform.LookAt(target);
                // Ensure the forward vector is in 2D
                transform.forward = new Vector3(transform.forward.x, 0f, transform.forward.z);                
            }
            //Check for max distance
            if (CalculateChainLength() >= projectileProperties.maxDistance)
            {
                ResetToDefaults();
                return;
            }
            //Move forward
            transform.position = transform.position + (transform.forward * projectileProperties.projectileSpeed);
            
        }
    }
    #endregion

    #region Public Methods
    public Transform GetTarget()
    {
        return target;
    }

    public void ClearTarget()
    {
        target = null;
    }

    public void ClearNextHook()
    {
        nextHook = null;
    }

    public void Init(LightningGun.ProjectileProperties properties, Transform startingTransform, int enemyCount = 0, List<Transform> ignoreEnemies = null)
    {
        projectileProperties = properties;
        transform.position = startingTransform.position;
        startingPosition = transform.position;
        transform.forward = startingTransform.forward;
        this.enemyCount = enemyCount;
        if (ignoreEnemies != null)
        {
            ignoreTargets = ignoreEnemies;
        }
        damager = new Damager();
        damager.damagerType = DamagerType.GrapplingHook;
    }

    public void ResetToDefaults()
    {
        projectileProperties = null;
        startingPosition = Vector3.zero;
        target = null;
        enemyCount = 0;
        ResetNextHook();
        ignoreTargets = new List<Transform>();
        gameObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void ResetNextHook()
    {
        if (nextHook)
        {
            nextHook.transform.SetParent(null);
            nextHook.ResetToDefaults();
            nextHook = null;
        }
    }

    private float CalculateChainLength()
    {
       return Vector3.Distance(startingPosition, transform.position);
    }
    
    private void SpawnNextProjectile()
    {
        LightningGun.ProjectileProperties newProperties = new LightningGun.ProjectileProperties(projectileProperties);
        newProperties.maxDistance = projectileProperties.maxDistancePerSubChain;
        //Spawn next hook
        GameObject nextHookObject = ObjectPool.Instance.GetObject(PoolObjectType.LightningProjectile);
        if (nextHookObject)
        {
            //Initialize
            nextHook = nextHookObject.GetComponent<LightningProjectile>();
            ignoreTargets.Add(target);
            nextHook.Init(newProperties, transform, enemyCount, ignoreTargets);
        }
    }

    private void OnTargetHit(Collider other)
    {
        // Is the collided object an enemy if not already attached
        if (other.tag == Strings.Tags.ENEMY && !ignoreTargets.Contains(other.transform))
        {
            //If a target is set
            if (other.transform != target)
            {
                target = other.transform;
            }
            //Do damage
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damager.impactDirection = transform.forward;
                damager.damage = projectileProperties.projectileHitDamage;
                damageable.TakeDamage(damager);
            }
            //Increment enemy count
            enemyCount++;
            //Check for max enemies
            if (enemyCount < projectileProperties.maxChainableEnemies)
            {
                //Spawn next chain
                SpawnNextProjectile();
            }
        }
    }

    private void CheckForTargets()
    {
        //Sphere cast to get all enemies
        Collider[] hits = Physics.OverlapSphere(transform.position, projectileProperties.homingRadius, LayerMask.GetMask(Strings.Layers.ENEMY));
        if (hits != null && hits.Length > 0)
        {
            float closestEnemyDistance = 0f;
            Transform closestEnemy = null;
            foreach (Collider hit in hits)
            {
                if (!ignoreTargets.Contains(hit.transform))
                {
                    //Check if enemy is within homing angle
                    float angle = Mathf.Abs(Vector3.Angle(transform.forward, hit.transform.position));
                    if (angle < projectileProperties.homingAngle / 2f)
                    {
                        //target = hit.transform;
                        //break;
                        // Check if the enemy is closest
                        if (closestEnemy)
                        {
                            float distance = Vector3.Distance(transform.position, hit.transform.position);
                            if (distance < closestEnemyDistance)
                            {
                                closestEnemyDistance = distance;
                                closestEnemy = hit.transform;
                            }
                        }
                        else
                        {
                            closestEnemyDistance = Vector3.Distance(transform.position, hit.transform.position);
                            closestEnemy = hit.transform;
                        }
                    }
                }
            }
            // Acquire target
            target = closestEnemy;
        }
    }
    #endregion

}
