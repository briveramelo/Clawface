using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Turing.VFX;
using ModMan;
using MEC;
using System;
using System.Linq;
using UnityEngine.Events;

public enum PushDirection{
    BACK,
    DOWN,
    BACK_DOWN
}

public abstract class EnemyBase : EventSubscriber, IStunnable, IDamageable, IEatable, ISpawnable
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
    [SerializeField] protected GameObject hips;
    [SerializeField] protected SpawnType enemyType;
    [SerializeField] private GameObject skeletonRoot;
    [SerializeField] protected SFXType vocalizeSound = SFXType.None;
    [SerializeField] protected Vector2 vocalizeInterval = Vector2.one;
    [SerializeField] protected SFXType footstepSound = SFXType.None;
    [SerializeField] protected GibEmitter gibEmitter;
    #endregion

    #region 3. Private fields
    private bool isStunFlashing = false; 
    private bool lastChance = false;
    private bool aboutTobeEaten = false;
    private Collider[] playerColliderList = new Collider[10];
    private Rigidbody[] jointRigidBodies;
    private Rigidbody[] JointRigidBodies {
        get {
            if (jointRigidBodies == null) {
                jointRigidBodies = GetComponentsInChildren<Rigidbody>();
            }
            return jointRigidBodies;
        }
    }
    private List<TransformMemento> skeletonTransformMomentos;
    private Transform[] skeletonTransforms;
    private List<float> rigidBodyMasses;    
    private bool isIndestructable;
    private int id;
    private bool ragdollOn;
    private float currentStunTime = 0.0f;
    private Vector3 spawnPosition;
    private TransformMemento grabObjectMomento;
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
    protected List<Collider> myColliders;
    protected List<Collider> MyColliders {
        get {
            if (myColliders==null) {
                myColliders = GetComponentsInChildren<Collider>().ToList();
            }
            return myColliders;
        }
    }
    protected bool canVocalize = true;
    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.EnableDisable; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
                { Strings.Events.PLAYER_KILLED, DoPlayerKilledState},
                { Strings.Events.ENEMY_INVINCIBLE, SetInvincible },
                { Strings.Events.SHOW_TUTORIAL_TEXT, DisableVocalization },
                { Strings.Events.HIDE_TUTORIAL_TEXT, EnableVocalization }
            };
        }
    }
    #endregion

    #region 4. Unity Lifecycle

    protected override void OnEnable()
    {
        base.OnEnable();
        id = GetInstanceID();
        ResetForRebirth();
    }

    protected override void Awake()
    {
        base.Awake();
        poolParent = transform.parent;
        transformMemento.Initialize(transform);
        InitSkeleton();
        ExtractRbWeights();
        if (grabObject != null)
        {
            grabObjectMomento = new TransformMemento();
            grabObjectMomento.Initialize(grabObject.transform);
        }
        ResetForRebirth();
    }

    void Update()
    {
        if (alreadyStunned)
        {
            currentStunTime += Time.deltaTime;

            if (currentStunTime > stunnedTime && !isIndestructable)
            {
                OnDeath();
            }
        }

        //Kill if it falls off the world
        if (hips.transform.position.y < -100.0f)
        {
            OnDeath();
        }       
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CancelInvoke("Vocalize");
    }

    #endregion

    #region 5. Public Methods   

    public abstract void DoPlayerKilledState(object[] parameters);

    public abstract Vector3 ReCalculateTargetPosition();

    public Vector3 GetPosition () { return transform.position; }

    void IDamageable.TakeDamage(Damager damager)
    {
        if (myStats.health > 0 && !isIndestructable)
        {
            DoHitReaction(damager);
            myStats.TakeDamage(damager.damage);
            damagePack.Set(damager, damaged);
            SFXManager.Instance.Play(hitSFX, transform.position);
            GoreManager.Instance.EmitDirectionalBlood(damagePack);            

            // Blood effect
            GameObject blood = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodSpurt);
            if (blood) blood.transform.position = transform.position;

            if (myStats.health <= 0 || myStats.health <= myStats.skinnableHealth)
            {
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

    void IEatable.Eat(out int health)
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.EAT_ENEMY);
        aboutTobeEaten = true;
        health = eatHealth;
        OnDeath();
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
        controller.SetDefaultChaseState();
        controller.ActivateAI();
        currentStunTime = 0.0f;
    }

    public GameObject GetAffectObject()
    {
        return affectObject;
    }

    public void EnableNavAgent()
    {
        navAgent.enabled = true;
    }

    public void WarpNavAgent(Vector3 position)
    {
        navAgent.Warp(position);
    }

    public virtual void OnDeath()
    {
        
        if (!will.isDead)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.DEATH_ENEMY, gameObject, scoreValue);

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

            else
            {
                gibEmitter.Emit();
            }

            navAgent.speed = 0;
            navAgent.enabled = false;
            aboutTobeEaten = false;
            alreadyStunned = false;
            isIndestructable = false;
            SFXManager.Instance.Play(deathSFX, transform.position);
            AIEnemyData testData = new AIEnemyData(controller.GetInstanceID());
            
            currentStunTime = 0.0f;
            if (AIManager.Instance != null)
            {
                AIManager.Instance.Remove(testData);
            }
            ClearAffecters();
            gameObject.SetActive(false);
        }
    }

    public virtual void ResetForRebirth()
    {
        DisableRagdoll();
        ToggleColliders(true);
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

    public void ToggleColliders(bool enabled) {
        MyColliders.ForEach(collider => collider.enabled = enabled);
    }

    public virtual void DisableStateResidue()
    {

    }

    public void ToggleCollider(bool enabled)
    {
        GetComponent<CapsuleCollider>().enabled = enabled;
    }

    public void EnableRagdoll(float weight = 1.0f)
    {
        DisableStateResidue();
        if (JointRigidBodies != null)
        {
            //Ignore the first entry (its the self rigidbody)
            for (int i = 1; i < JointRigidBodies.Length; i++)
            {
                JointRigidBodies[i].mass = weight;
                JointRigidBodies[i].useGravity = true;
                JointRigidBodies[i].isKinematic = false;                
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
        if (JointRigidBodies != null)
        {
            //Ignore the first entry (its the self rigidbody)
            for (int i = 1; i < JointRigidBodies.Length; i++)
            {
                //RagdollHandler ragdollHandler = jointRigidBodies[i].GetComponent<RagdollHandler>();
                //if (ragdollHandler)
                //{
                //    ragdollHandler.ResetBone();
                //}
                JointRigidBodies[i].useGravity = false;
                JointRigidBodies[i].velocity = Vector3.zero;
                JointRigidBodies[i].angularVelocity = Vector3.zero;
                JointRigidBodies[i].isKinematic = true;
                if (rigidBodyMasses != null)
                {
                    JointRigidBodies[i].mass = rigidBodyMasses[i];
                }                
            }
        }
        //Reset skeleton
        if (skeletonRoot)
        {
            for(int i = 0; i < skeletonTransforms.Length; i++)
            {
                skeletonTransformMomentos[i].Reset(skeletonTransforms[i]);
            }
        }

                
        if (grabObject)
        {
            grabObject.transform.parent = transform;
            grabObjectMomento.Reset(grabObject.transform);
        }        
        ragdollOn = false;
    }

    public GameObject GetGrabObject()
    {
        return grabObject;
    }

    public void TemporaryTerminalIndestructable()
    {
        isIndestructable = true;
        StartCoroutine(FinalDeath());
    }

    public void CloserToEat(bool isClose)
    {
        if (isClose)
        {
            hitFlasher.SetCloseToEatState();
            isStunFlashing = true;
        }
        else
        {
            hitFlasher.SetStunnedState();
            isStunFlashing = true;
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
        Color fadedColor = Color.black;
        hitFlasher.SetOutlineColor(fadedColor);
        hitFlasher.FixColor(1f, 5f, fadedColor);
        Push(20.0f, PushDirection.DOWN);
    }

    private void EnableCollider()
    {
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private IEnumerator WaitAndGetUp()
    {
        yield return new WaitForSeconds(3.0f);
        bool findNearestTile = false;
        while ((PlayerIsNear() || IsFalling()) && !findNearestTile)
        {
            findNearestTile = IsOnObstacle();
            yield return new WaitForEndOfFrame();
        }
        GetUp(findNearestTile);
    }

    private bool IsOnObstacle()
    {
        return Physics.CheckSphere(hips.transform.position, 2.0f, LayerMask.GetMask(Strings.Layers.OBSTACLE));
    }

    protected virtual bool IsFalling()
    {        
        return !Physics.CheckSphere(hips.transform.position, 2.0f, LayerMask.GetMask(Strings.Layers.GROUND));
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

    private void ActivateAIMethods(Vector3 position)
    {
        navAgent.enabled = false;
        navObstacle.enabled = false;
        GetComponent<ISpawnable>().WarpToNavMesh(position);
        controller.ActivateAI();
    }

    private void GetUp(bool findNearestFloorTile = false)
    {
        EnableCollider();
        hitFlasher.ResetColors();
        Vector3 warpPosition;
        bool spaceFound = GetValidTile(out warpPosition, findNearestFloorTile);
        DisableRagdoll();
        if (spaceFound)
        {
            ActivateAIMethods(warpPosition);
            Vocalize();
        }
        else
        {
            //Kill
            OnDeath();
        }
    }

    void Vocalize ()
    {
        if (enabled && vocalizeSound != SFXType.None && canVocalize)
        {
            SFXManager.Instance.Play(vocalizeSound, transform.position);
            Invoke ("Vocalize", UnityEngine.Random.Range(vocalizeInterval.x, vocalizeInterval.y));
        }
    }

    void EnableVocalization (params object[] parameters) { canVocalize = true; }
    void DisableVocalization (params object[] parameters) { canVocalize = false; }

    void PlayFootstepSound ()
    {
        if (footstepSound != SFXType.None)
        {
            SFXManager.Instance.Play(footstepSound, transform.position);
        }
    }

    private bool GetValidTile(out Vector3 position, bool findNearestFloorTile)
    {
        bool spaceFound = false;
        position = Vector3.zero;
        if (gameObject.activeSelf)
        {
            position = hips.transform.position;
            Ray ray = new Ray(position, Vector3.down);
            int mask = LayerMask.GetMask(Strings.Layers.GROUND);
            RaycastHit hit;

            if (!findNearestFloorTile && Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                position = hit.point;
                spaceFound = true;
            }
            else
            {
                int i = 1;
                while (!spaceFound && i < 7)
                {
                    Collider[] tiles = Physics.OverlapSphere(hips.transform.position, 10.0f * i, mask);
                    if (tiles.Length > 0)
                    {
                        position = tiles[0].transform.position;
                        spaceFound = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
        return spaceFound;
    }

    private void FallDown()
    {
        ToggleCollider(false);
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

    private void InitSkeleton() {
        if (skeletonRoot) {
            skeletonTransformMomentos = new List<TransformMemento>();
            skeletonTransforms = skeletonRoot.GetComponentsInChildren<Transform>();
            foreach (Transform transform in skeletonTransforms) {
                TransformMemento transformMemento = new TransformMemento();
                transformMemento.Initialize(transform);
                skeletonTransformMomentos.Add(transformMemento);
            }
        }
    }

    private void ExtractRbWeights()
    {
        if(rigidBodyMasses == null)
        {
            rigidBodyMasses = new List<float>(JointRigidBodies.Length);
        }
        foreach(Rigidbody rb in JointRigidBodies)
        {
            rigidBodyMasses.Add(rb.mass);
        }
    }

    private void AddForce(Vector3 force)
    {
        if (pushRoot)
        {
            pushRoot.AddForce(force, ForceMode.Impulse);
        }
    }

    private IEnumerator FinalDeath()
    {
        float finalStunTime = 0.0f;

        while(finalStunTime < stunnedTime)
        {
            finalStunTime += Time.deltaTime;
            yield return null;
        }
        OnDeath();
    }
    #endregion
}
