using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Turing.VFX;
using ModMan;
using MovementEffects;
using System;

public enum PushDirection{
    BACK,
    DOWN,
    BACK_DOWN
}

public abstract class EnemyBase : RoutineRunner, IStunnable, IDamageable, IEatable, ISpawnable
{
    #region serialized fields
    [SerializeField] protected AIController controller;
    [SerializeField] protected VelocityBody velBody;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected NavMeshObstacle navObstacle;
    [SerializeField] protected Transform bloodEmissionLocation;
    [SerializeField] private GameObject grabObject;
    [SerializeField] private GameObject affectObject;
    [SerializeField] protected HitFlasher hitFlasher;
    [SerializeField] private SFXType hitSFX;
    [SerializeField] private SFXType deathSFX;
    [SerializeField] Rigidbody pushRoot;
    [SerializeField] PushDirection pushDirection;
    [SerializeField] GameObject hips;
    [SerializeField] protected SpawnType enemyType;
    #endregion

    #region 3. Private fields
    private bool isStunFlashing = false; 
    private bool lastChance = false;
    private bool aboutTobeEaten = false;
    private Collider[] playerColliderList = new Collider[10];
    private Rigidbody[] jointRigidBodies;
    private List<float> rigidBodyMasses;
    private Vector3 grabStartPosition;
    private bool isIndestructable;
    private int id;
    private bool ragdollOn;
    private float currentStunTime = 0.0f;
    private int bufferHealth = 3;
    private Vector3 spawnPosition;
    #endregion

    #region 0. Protected fields
    protected bool alreadyStunned = false;
    protected int eatHealth;
    protected int scoreValue;
    protected float stunnedTime;
    protected Stats myStats;
    protected Damaged damaged = new Damaged();
    protected DamagePack damagePack = new DamagePack();
    protected Will will = new Will();
    protected List<AIState> aiStates;
    protected TransformMemento transformMemento=new TransformMemento();
    protected Transform poolParent;
    #endregion

    #region 4. Unity Lifecycle

    public void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, DoPlayerKilledState);
        EventSystem.Instance.RegisterEvent(Strings.Events.ENEMY_INVINCIBLE, SetInvincible);
        id = GetInstanceID();
        //if (will.willHasBeenWritten)
        //{
            ResetForRebirth();
        //}
    }

    public void Update()
    {
        if (alreadyStunned)
        {
            currentStunTime += Time.deltaTime;

            if (currentStunTime > stunnedTime)
            {
                OnDeath();
            }
        }

        //Kill if it falls off the world
        if(hips.transform.position.y < -100.0f)
        {
            OnDeath();
        }
    }

    public virtual void Awake()
    {
        poolParent = transform.parent;
        transformMemento.Initialize(transform);        
        InitRagdoll();        
        if (grabObject != null)
        {
            grabStartPosition = grabObject.transform.localPosition;
        }        
        ResetForRebirth();        
    }

    private new void OnDisable()
    {
        EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, DoPlayerKilledState);
        EventSystem.Instance.UnRegisterEvent(Strings.Events.ENEMY_INVINCIBLE, SetInvincible);
        base.OnDisable();
    }
    #endregion

    #region 5. Public Methods   

    public abstract void DoPlayerKilledState(object[] parameters);

    public abstract Vector3 ReCalculateTargetPosition();

    void IDamageable.TakeDamage(Damager damager)
    {
        if (myStats.health > 0 && !isIndestructable)
        {
            DoHitReaction(damager);
            myStats.TakeDamage(damager.damage);
            damagePack.Set(damager, damaged);
            SFXManager.Instance.Play(hitSFX, transform.position);
            DamageFXManager.Instance.EmitDamageEffect(damagePack);
            

            if (myStats.health <= 0)
            {
                myStats.health = bufferHealth;
            }


            if (myStats.health <= myStats.skinnableHealth)
            {
                    myStats.health = bufferHealth;
                    alreadyStunned = true;
            }
        }

        if (alreadyStunned)
        {
            if (!isStunFlashing)
            {
                hitFlasher.SetStunnedState();
                isStunFlashing = true;
            }
        }
        else
        {
            if (!isStunFlashing)
            {
                hitFlasher.HitFlash();
            }
        }
    }

    float IDamageable.GetHealth()
    {
        return myStats.health;
    }

    void ISpawnable.WarpToNavMesh(Vector3 position)
    {
        transform.position = position;
        navAgent.enabled = true;
        bool sucessfulWarp = navAgent.Warp(position);
        if (!sucessfulWarp) {
            Debug.LogWarning("Failed to warp!");
        }
    }


    public bool IsEatable()
    {
        return alreadyStunned;
    }

    int IEatable.Eat()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.EAT_ENEMY);
        aboutTobeEaten = true;
        Timing.RunCoroutine(DelayAction(OnDeath), coroutineName);
        return eatHealth;
    }

    void IStunnable.Stun()
    {
    }

    public bool HasWillBeenWritten() { return will.willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        will.willHasBeenWritten = true;
        will.onDeath = onDeath;
    }

    public virtual void DoHitReaction(Damager damager)
    {
    }

    public void ResetHealth()
    {
        myStats.health = myStats.maxHealth;
        alreadyStunned = false;
        isStunFlashing = false;


        hitFlasher.StopAllCoroutines();
        hitFlasher.SetStrength(0.0f);
        controller.SetDefaultState();
        controller.ActivateAI();
        currentStunTime = 0.0f;
    }

    public GameObject GetAffectObject()
    {
        return affectObject;
    }


    public virtual void OnDeath()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.DEATH_ENEMY, gameObject, scoreValue);

        if (!will.isDead)
        {
            will.isDead = true;
            if (will.willHasBeenWritten)
            {
                will.onDeath();
            }

            if (!aboutTobeEaten)
            {
                GameObject mallCopParts = ObjectPool.Instance.GetObject(PoolObjectType.VFXMallCopExplosion);
                if (mallCopParts)
                {
                    SFXManager.Instance.Play(SFXType.BloodExplosion, transform.position);
                    mallCopParts.transform.position = transform.position + Vector3.up * 3f;
                    mallCopParts.transform.rotation = transform.rotation;
                }
            }
            UpgradeManager.Instance.AddEXP(Mathf.FloorToInt(myStats.exp));
            navAgent.speed = 0;
            navAgent.enabled = false;
            gameObject.SetActive(false);
            aboutTobeEaten = false;
            SFXManager.Instance.Play(deathSFX, transform.position);
            AIEnemyData testData = new AIEnemyData(controller.GetInstanceID());
            currentStunTime = 0.0f;
            if (AIManager.Instance != null)
            {
                AIManager.Instance.Remove(testData);
            }
            ClearAffecters();
        }
    }

    public virtual void ResetForRebirth()
    {
        DisableRagdoll();        
        GetComponent<CapsuleCollider>().enabled = true;
        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
        will.Reset();
        transform.SetParent(poolParent);
        transform.localScale = transformMemento.startScale;
        lastChance = false;
        isStunFlashing = false;
        alreadyStunned = false;
        isIndestructable = false;        
    }

    public void DisableCollider()
    {
        GetComponent<CapsuleCollider>().enabled = false;
    }

    public void EnableRagdoll(float weight = 1.0f)
    {        
        if (jointRigidBodies != null)
        {
            //Ignore the first entry (its the self rigidbody)
            for (int i = 1; i < jointRigidBodies.Length; i++)
            {
                jointRigidBodies[i].mass *= weight;
                jointRigidBodies[i].useGravity = true;
                jointRigidBodies[i].isKinematic = false;                
            }
        }
        animator.enabled = false;
        AIController aiController = GetComponent<AIController>();
        if (aiController)
        {
            aiController.DeActivateAI();
        }
        navAgent.enabled = false;
        ragdollOn = true;
    }

    public void DisableRagdoll()
    {        
        if (jointRigidBodies != null)
        {
            //Ignore the first entry (its the self rigidbody)
            for (int i = 1; i < jointRigidBodies.Length; i++)
            {
                RagdollHandler ragdollHandler = jointRigidBodies[i].GetComponent<RagdollHandler>();
                if (ragdollHandler)
                {
                    ragdollHandler.ResetBone();
                }
                jointRigidBodies[i].useGravity = false;
                jointRigidBodies[i].velocity = Vector3.zero;
                jointRigidBodies[i].angularVelocity = Vector3.zero;
                jointRigidBodies[i].isKinematic = true;
                if (rigidBodyMasses != null)
                {
                    jointRigidBodies[i].mass = rigidBodyMasses[i];
                }                
            }
        }
        animator.enabled = true;
        
        if (grabObject)
        {
            grabObject.transform.parent = transform;
            grabObject.transform.localPosition = grabStartPosition;
            grabObject.transform.localScale = Vector3.one;
        }        
        ragdollOn = false;
    }

    public GameObject GetGrabObject()
    {
        return grabObject;
    }

    public void MakeIndestructable()
    {
        isIndestructable = true;
    }

    public void CloserToEat(bool isClose)
    {
        if (isClose)
        {
            hitFlasher.SetCloseToEatState();
        }
        else
        {
            hitFlasher.SetStunnedState();
        }
    }

    public void Push(float force)
    {
        Push(force, pushDirection);
    }

    public void Push(float force, PushDirection direction)
    {
        if (!ragdollOn)
        {
            FallDown();
            switch (direction)
            {
                case PushDirection.BACK:
                    AddForce(force * -velBody.GetForward());
                    break;

                case PushDirection.DOWN:
                    AddForce(force * Vector3.down);
                    break;
                case PushDirection.BACK_DOWN:
                    AddForce(force * (-velBody.GetForward() + Vector3.down).normalized);
                    break;
            }
            StopCoroutine(WaitAndGetUp());
            StartCoroutine(WaitAndGetUp());
        }
    }

    public void SpawnWithRagdoll(Vector3 position)
    {
        spawnPosition = position;
        transform.position = spawnPosition;
        SpawnWithRagdoll();
    }
    #endregion

    #region 6. Private Methods
    private void SpawnWithRagdoll()
    {
        Push(20.0f, PushDirection.DOWN);
    }

    private void EnableCollider()
    {
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private IEnumerator WaitAndGetUp()
    {
        yield return new WaitForSeconds(3.0f);
        while (PlayerIsNear() || IsFalling())
        {
            yield return new WaitForEndOfFrame();
        }
        GetUp();
    }

    private bool IsFalling()
    {
        bool result = Physics.CheckSphere(hips.transform.position, 2.0f, LayerMask.GetMask(Strings.Layers.GROUND));
        return !result;
    }

    private bool PlayerIsNear()
    {
        Transform playerTransform = controller.FindPlayer();
        if (playerTransform)
        {
            return Vector3.Distance(transform.position, playerTransform.position) <= 4.0f;
        }
        else
        {
            return false;
        }
    }

    private void ActivateAIMethods()
    {
        if (gameObject.activeSelf)
        {
            Vector3 position = hips.transform.position;
            Ray ray = new Ray(position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Strings.Layers.GROUND)))
            {
                position = hit.point;
                GetComponent<ISpawnable>().WarpToNavMesh(position);
                AIController aiController = GetComponent<AIController>();
                if (aiController)
                {
                    aiController.ActivateAI();
                }
            }
        }
    }

    private void GetUp()
    {
        EnableCollider();
        DisableRagdoll();
        ActivateAIMethods();
    }

    private void FallDown()
    {
        DisableCollider();
        EnableRagdoll();
    }

    private void SetInvincible(object[] parameters)
    {
        isIndestructable = (bool)parameters[0];
    }

    private void ClearAffecters()
    {
        foreach (Transform child in affectObject.transform)
        {
            if(child.name.Contains("VFX"))
            child.parent = null;
        }
    }

    private void ExtractRbWeights()
    {
        if(rigidBodyMasses == null)
        {
            rigidBodyMasses = new List<float>(jointRigidBodies.Length);
        }
        foreach(Rigidbody rb in jointRigidBodies)
        {
            rigidBodyMasses.Add(rb.mass);
        }
    }

    private void InitRagdoll()
    {
        jointRigidBodies = GetComponentsInChildren<Rigidbody>();       
        ExtractRbWeights();
    }

    private void AddForce(Vector3 force)
    {
        if (pushRoot)
        {
            pushRoot.AddForce(force, ForceMode.Impulse);
        }
    }
    #endregion
}
