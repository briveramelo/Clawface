﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using Turing.VFX;
using ModMan;

public class LightningProjectile : MonoBehaviour {

    #region Serialized Unity Inspector Fields

    [SerializeField] LightningChain chainEffect;
    [SerializeField] private ParticleSystem vfxLightning;
    [SerializeField] 
    [Tooltip("Max time projectile will remain after hitting an enemy (defines how long you can see the lightning)")]
    private float maxLiveTimeAfterHittingEnemy = 1.0f;

    #endregion

    #region Private Fields
    private LightningGun.ProjectileProperties projectileProperties;    
    private Transform target;
    private int enemyCount;
    private List<Transform> ignoreTargets;
    private Damager damager;
    private Vector3 startingPosition;
    private bool markedForRemoval;
    private float removalTime;

    private float yPosition;
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
        chainEffect.Reset();
        ResetToDefaults();
    }

    private void Update()
    {
        if (markedForRemoval)
        {
            removalTime += Time.deltaTime;
            if(removalTime > maxLiveTimeAfterHittingEnemy)
            {                
                ResetToDefaults();
            }
        }

        //Are hook properties set and have we not hit a target
        if (projectileProperties != null)
        {

            ////Cancel out y of the target so that the projectile stays at the same level
            //if (target)
            //{
            //    Vector3 targetPosition = target.position;
            //    targetPosition.y = transform.position.y;
            //    target.position = targetPosition;
            //}

            if (target && !target.gameObject.activeSelf)
            {
                if (!CheckForTargets())
                {
                    ResetToDefaults();
                    return;
                }
            }

            if (target)
            {
                // Look at target
                transform.LookAt(target);

                // transform.rotation.eulerAngles.Set(0f, transform.rotation.eulerAngles.y, 0f);

                // Ensure the forward vector is in 2D
                //transform.forward = new Vector3(0f, transform.forward.y, 0f);
            }
            else
            {
                if (enemyCount == 0) CheckForTargets();
            }

            //Check for max distance
            if (CalculateChainLength() >= projectileProperties.maxDistance && !target)
            {
                ResetToDefaults();
                return;
            }
            //Move forward
            transform.position = transform.position + (transform.forward * projectileProperties.projectileSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
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

    public void Init(LightningGun.ProjectileProperties properties, Transform startingTransform, int enemyCount = 0, List<Transform> ignoreEnemies = null)
    {
        chainEffect.Reset();
        vfxLightning.Clear();
        projectileProperties = properties;
        transform.position = startingTransform.position;
        startingPosition = transform.position;
        transform.forward = startingTransform.forward;
        this.enemyCount = enemyCount;
        yPosition = transform.position.y;
        
        if (ignoreEnemies != null)
        {
            GameObject affectTarget = ignoreEnemies.Tail().GetComponent<EnemyBase>().GetAffectObject();
            if (affectTarget)
            {
                chainEffect.SetOrigin(affectTarget.transform);
            }
            ignoreTargets = ignoreEnemies;
        }
        damager = new Damager();
        damager.damagerType = DamagerType.GrapplingHook;
        
        chainEffect.SetTarget (transform);

        if (enemyCount == 0)
        {
            chainEffect.SetOrigin(transform);
            chainEffect.SetTarget(transform);
        }
    }

    public void ResetToDefaults()
    {
        chainEffect.Reset();
        vfxLightning.Clear();
        projectileProperties = null;
        startingPosition = Vector3.zero;
        target = null;
        enemyCount = 0;
        ignoreTargets = new List<Transform>();
        gameObject.SetActive(false);
        markedForRemoval = false;
        removalTime = 0.0f;
        vfxLightning.gameObject.SetActive(true);
        yPosition = 0f;
    }
    #endregion

    #region Private Methods
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
            LightningProjectile nextProjectile = nextHookObject.GetComponent<LightningProjectile>();
            nextProjectile.Init(newProperties, transform, enemyCount, ignoreTargets);

            if (!nextProjectile.CheckForTargets())
                nextProjectile.ResetToDefaults();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Is the collided object an enemy if not already attached
        if (other.CompareTag(Strings.Tags.ENEMY) && !ignoreTargets.Contains(other.transform))
        {
            //If a target is set
            if (other.transform != target)
            {
                target = other.transform;
            }

            //Do damage
            IDamageable damageable = target.GetComponent<IDamageable>();
            EnemyBase enemyBase = target.GetComponent<EnemyBase>();
            GameObject affectTarget = null;
            if (enemyBase)
            {
                affectTarget = target.GetComponent<EnemyBase>().GetAffectObject();
            }
            if (!damageable.IsNull())
            {
                damager.impactDirection = transform.forward;
                damager.damage = projectileProperties.projectileHitDamage;
                damageable.TakeDamage(damager);
                ignoreTargets.Add(other.transform);

                GameObject blood = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodSpurt);
                if (blood) blood.transform.position = damageable.GetPosition();

                GameObject vfx = ObjectPool.Instance.GetObject(PoolObjectType.VFXLightningGunImpact);
                if (vfx && affectTarget)
                {
                    vfx.transform.SetParent(affectTarget.transform);
                    vfx.transform.position = transform.position;
                }

                SFXManager.Instance.Play(projectileProperties.lightningSFX, transform.position);
            }

            //Increment enemy count
            enemyCount++;
            //Check for max enemies
            if (enemyCount < projectileProperties.maxChainableEnemies)
            {
                SpawnNextProjectile();
            }

            markedForRemoval = true;
            vfxLightning.gameObject.SetActive(false);
        }
        else if (other.CompareTag(Strings.Tags.WALL))
        {
            ResetToDefaults();
        }
    }

    /// <summary>
    /// Checks for targets in a sphere. Returns true if a target was found.
    /// </summary>
    /// <returns></returns>
    public bool CheckForTargets()
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

            if (target != null) return true;
        }
        return false;
    }
    #endregion

}
