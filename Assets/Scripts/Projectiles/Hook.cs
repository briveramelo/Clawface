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
    private Hook nextHook;
    private List<Transform> ignoreTargets;
    private Damager damager;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (endingPoint)
        {
            HookEndPoint hookEndPoint = endingPoint.GetComponent<HookEndPoint>();
            if (hookEndPoint.targetHitEvent == null)
            {
                hookEndPoint.targetHitEvent = new HookEndPoint.HitEvent();
            }
            hookEndPoint.targetHitEvent.AddListener(OnTargetHit);
        }
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
        if (hookProperties != null && !isTargetHit)
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
                endingPoint.LookAt(target);
                // Ensure the forward vector is in 2D
                endingPoint.forward = new Vector3(endingPoint.forward.x, 0f, endingPoint.forward.z);                
            }
            //Check for max distance
            if (CalculateChainLength() >= hookProperties.maxDistance)
            {
                ResetToDefaults();
                return;
            }
            //Move forward
            endingPoint.position = endingPoint.position + (endingPoint.forward * hookProperties.projectileSpeed);
            
        } else if (isTargetHit)
        {
            endingPoint.position = new Vector3(target.position.x, endingPoint.position.y, target.position.z);
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable.GetHealth() <= 0f)
            {
                ConnectToNextSegment();
                if (!target)
                {
                    ResetToDefaults();
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public Transform GetEndingPoint()
    {
        return endingPoint;
    }

    public Hook GetNextHook()
    {
        return nextHook;
    }

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

    public void Init(GrapplerMod.HookProperties properties, Transform hookTransform, int enemyCount = 0, List<Transform> ignoreEnemies = null)
    {
        hookProperties = properties;
        transform.position = hookTransform.position;
        transform.forward = hookTransform.forward;
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
        hookProperties = null;
        target = null;
        isTargetHit = false;
        enemyCount = 0;
        startingPoint.localPosition = Vector3.zero;
        endingPoint.SetParent(transform);
        endingPoint.localPosition = Vector3.zero;
        endingPoint.transform.forward = transform.forward;
        ResetNextHook();
        ignoreTargets = new List<Transform>();
        gameObject.SetActive(false);
        StopAllCoroutines();
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
       return Vector3.Distance(startingPoint.position, endingPoint.position);
    }
    
    private void SpawnNextChain()
    {
        GrapplerMod.HookProperties newProperties = new GrapplerMod.HookProperties(hookProperties);
        newProperties.maxDistance = hookProperties.maxDistancePerSubChain;
        //Spawn next hook
        GameObject nextHookObject = ObjectPool.Instance.GetObject(PoolObjectType.GrapplingHook);
        if (nextHookObject)
        {
            //Initialize
            nextHook = nextHookObject.GetComponent<Hook>();
            nextHook.transform.SetParent(endingPoint);
            ignoreTargets.Add(target);
            nextHook.Init(newProperties, endingPoint, enemyCount, ignoreTargets);
        }
    }

    private void OnTargetHit(Collider other)
    {
        // Is the collided object an enemy if not already attached
        if (!isTargetHit && other.tag == Strings.Tags.ENEMY && !ignoreTargets.Contains(other.transform))
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
                damager.impactDirection = endingPoint.transform.forward;
                damager.damage = hookProperties.projectileHitDamage;
                damageable.TakeDamage(damager);
            }
            StopAllCoroutines();
            StartCoroutine(DoDamagePerSecond());
            //Set bool
            isTargetHit = true;
            //Set endpoint position
            endingPoint.position = other.ClosestPoint(endingPoint.position);
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

    private IEnumerator DoDamagePerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (target)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damager.impactDirection = endingPoint.transform.forward;
                    damager.damage = hookProperties.projectileDamagePerSecond;
                    damageable.TakeDamage(damager);
                }
            }
            else
            {
                break;
            }
        }
    }

    private void ConnectToNextSegment()
    {
        if (nextHook)
        {
            Transform newEndingPoint = nextHook.GetEndingPoint();
            endingPoint.position = newEndingPoint.position;
            endingPoint.forward = newEndingPoint.forward;
            endingPoint.SetParent(newEndingPoint.parent);
            Hook newNextHook = nextHook.GetNextHook();
            Transform newTarget = nextHook.GetTarget();
            if (newNextHook)
            {
                newNextHook.transform.SetParent(endingPoint);
                newNextHook.transform.localPosition = Vector3.zero;
            }
            nextHook.ClearNextHook();
            nextHook.ClearTarget();
            ResetNextHook();
            nextHook = newNextHook;
            target = newTarget;
            if (!target)
            {
                isTargetHit = false;
            }
        }
        else
        {
            isTargetHit = false;
        }
    }

    private void CheckForTargets()
    {
        //Sphere cast to get all enemies
        Collider[] hits = Physics.OverlapSphere(endingPoint.position, hookProperties.homingRadius, LayerMask.GetMask(Strings.Layers.ENEMY));
        if (hits != null && hits.Length > 0)
        {
            float closestEnemyDistance = 0f;
            Transform closestEnemy = null;
            foreach (Collider hit in hits)
            {
                if (!ignoreTargets.Contains(hit.transform))
                {
                    //Check if enemy is within homing angle
                    float angle = Mathf.Abs(Vector3.Angle(endingPoint.forward, hit.transform.position));
                    if (angle < hookProperties.homingAngle / 2f)
                    {
                        //target = hit.transform;
                        //break;
                        // Check if the enemy is closest
                        if (closestEnemy)
                        {
                            float distance = Vector3.Distance(endingPoint.position, hit.transform.position);
                            if (distance < closestEnemyDistance)
                            {
                                closestEnemyDistance = distance;
                                closestEnemy = hit.transform;
                            }
                        }
                        else
                        {
                            closestEnemyDistance = Vector3.Distance(endingPoint.position, hit.transform.position);
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

    #region Private Structures
    #endregion

}
