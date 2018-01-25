using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;
using Turing.VFX;
using ModMan;

public class LightningProjectile : MonoBehaviour {

    #region Serialized Unity Inspector Fields

    [SerializeField] LightningChain chainEffect;

    #endregion

    #region Private Fields
    private LightningGun.ProjectileProperties projectileProperties;    
    private Transform target;
    private int enemyCount;
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
        chainEffect.Reset();
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
                transform.forward = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);                
            }
            //Check for max distance
            if (CalculateChainLength() >= projectileProperties.maxDistance)
            {
                ResetToDefaults();
                return;
            }
            //Move forward
            transform.position = transform.position + (transform.forward * projectileProperties.projectileSpeed * Time.deltaTime);
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
        projectileProperties = properties;
        transform.position = startingTransform.position;
        startingPosition = transform.position;
        transform.forward = startingTransform.forward;
        this.enemyCount = enemyCount;

        
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
    }

    public void ResetToDefaults()
    {
        chainEffect.Reset();
        projectileProperties = null;
        startingPosition = Vector3.zero;
        target = null;
        enemyCount = 0;
        ignoreTargets = new List<Transform>();
        gameObject.SetActive(false);
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
            ignoreTargets.Add(target);
            nextProjectile.Init(newProperties, transform, enemyCount, ignoreTargets);
            SFXManager.Instance.Play(projectileProperties.lightningSFX, transform.position);
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
            
            //Increment enemy count
            enemyCount++;
            //Check for max enemies
            if (enemyCount < projectileProperties.maxChainableEnemies)
            {
                //Spawn next chain
                SpawnNextProjectile();
            }

            //Do damage
            IDamageable damageable = target.GetComponent<IDamageable>();
            EnemyBase enemyBase = target.GetComponent<EnemyBase>();
            GameObject affectTarget = null;
            if (enemyBase)
            {
                affectTarget = target.GetComponent<EnemyBase>().GetAffectObject();
            }
            if (damageable != null)
            {
                damager.impactDirection = transform.forward;
                damager.damage = projectileProperties.projectileHitDamage;
                damageable.TakeDamage(damager);
                GameObject vfx = ObjectPool.Instance.GetObject (PoolObjectType.VFXLightningGunImpact);
                if (vfx && affectTarget)
                {
                    vfx.transform.SetParent (affectTarget.transform);
                    vfx.transform.position = transform.position;
                }
            }

            gameObject.SetActive(false);
        }
        else if (other.CompareTag(Strings.Tags.WALL))
        {
            gameObject.SetActive(false);
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
