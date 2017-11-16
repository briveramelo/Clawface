using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Turing.VFX;
using ModMan;

public abstract class EnemyBase : RoutineRunner, IStunnable, IDamageable, IEatable, ISpawnable
{
    #region serialized fields
    [SerializeField] protected AIController controller;
    [SerializeField] protected VelocityBody velBody;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Stats myStats;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected NavMeshObstacle navObstacle;
    [SerializeField] protected int eatHealth;
    [SerializeField] protected CopUI copUICanvas;
    [SerializeField] protected Transform bloodEmissionLocation;
    [SerializeField] protected int scorePopupDelay = 2;
    [SerializeField] protected int scoreValue = 200;
    [SerializeField] private GameObject grabObject;
    [SerializeField] protected HitFlasher hitFlasher;
    [SerializeField] private SFXType hitSFX;
    [SerializeField] private SFXType deathSFX;
    #endregion

    #region 3. Private fields
    private int stunCount;
    private bool lastChance = false;
    private bool alreadyStunned = false;
    private Collider[] playerColliderList = new Collider[10];
    private Rigidbody[] jointRigidBodies;
    private Vector3 grabStartPosition;
    #endregion

    #region 0. Protected fields
    protected Damaged damaged = new Damaged();
    protected DamagePack damagePack = new DamagePack();
    protected Will will = new Will();
    protected List<AIState> aiStates;
    protected TransformMemento transformMemento=new TransformMemento();
    protected Transform poolParent;
    #endregion

    #region 4. Unity Lifecycle

    private void OnEnable()
    {

        if (will.willHasBeenWritten)
        {
            ResetForRebirth();
        }
    }    

    public virtual void Awake()
    {
        poolParent = transform.parent;
        transformMemento.Initialize(transform);
        jointRigidBodies = GetComponentsInChildren<Rigidbody>();
        if (grabObject != null)
        {
            grabStartPosition = grabObject.transform.localPosition;
        }
        ResetForRebirth();
    }

    #endregion

    #region 5. Public Methods   

    void IDamageable.TakeDamage(Damager damager)
    {
        if (myStats.health > 0)
        {
            myStats.TakeDamage(damager.damage);
            damagePack.Set(damager, damaged);
            SFXManager.Instance.Play(hitSFX, transform.position);
            DamageFXManager.Instance.EmitDamageEffect(damagePack);
            hitFlasher.Flash (1.0f, 0.15f);

            if (myStats.health <= 0)
            {
                if (alreadyStunned)
                    lastChance = true;

                if (lastChance && alreadyStunned)
                {
                    OnDeath();
                }
                else
                {
                    myStats.health = 1;
                    lastChance = true;
                }
            }


            if (myStats.health <= myStats.skinnableHealth)
            {
                copUICanvas.gameObject.SetActive(true);
                copUICanvas.ShowAction(ActionType.Skin);
                if (!alreadyStunned)
                {
                    myStats.health = 1;
                    alreadyStunned = true;
                }
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
        bool sucessfulWarp = navAgent.Warp(position);
        if (!sucessfulWarp) {
            Debug.LogWarning("Failed to warp!");
        }
        navAgent.enabled = true;
    }


    bool IEatable.IsEatable()
    {
        return myStats.health <= myStats.skinnableHealth;
    }

    int IEatable.Eat()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.ATE_ENEMY);
        Invoke("OnDeath", 0.1f);
        EventSystem.Instance.TriggerEvent(Strings.Events.EAT_ENEMY);
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

    public virtual void OnDeath()
    {

        EventSystem.Instance.TriggerEvent(Strings.Events.DEATH_ENEMY);

        if (!will.isDead)
        {
            will.isDead = true;
            if (will.willHasBeenWritten)
            {
                will.onDeath();
            }
                GameObject mallCopParts = ObjectPool.Instance.GetObject(PoolObjectType.VFXMallCopExplosion);
                if (mallCopParts)
                {
                    SFXManager.Instance.Play(SFXType.BloodExplosion, transform.position);
                    mallCopParts.transform.position = transform.position + Vector3.up * 3f;
                    mallCopParts.transform.rotation = transform.rotation;
                    mallCopParts.DeActivate(5f);
                }


            UpgradeManager.Instance.AddEXP(Mathf.FloorToInt(myStats.exp));

            GameObject worldScoreObject = ObjectPool.Instance.GetObject(PoolObjectType.WorldScoreCanvas);
            if (worldScoreObject)
            {
                worldScoreObject.GetComponent<Canvas>().GetComponent<RectTransform>().SetPositionAndRotation(transform.position, transform.rotation);                //worldScoreObject.transform.position = transform.position /*+ Vector3.up * 3f*/;
                WorldScoreUI popUpScore = worldScoreObject.GetComponent<WorldScoreUI>();

                int scoreBonus = scoreValue * ScoreManager.Instance.GetCurrentMultiplier();
                popUpScore.DisplayScoreAndHide(scoreBonus, scorePopupDelay);
                // ScoreManager.Instance.AddToScoreAndCombo(scoreBonus);
            }
            navAgent.speed = 0;
            navAgent.enabled = false;
            gameObject.SetActive(false);
            EventSystem.Instance.TriggerEvent(Strings.Events.KILLED_ENEMY, scoreValue);
            SFXManager.Instance.Play(deathSFX, transform.position);
        }
    }

    public virtual void ResetForRebirth()
    {
        DisableRagdoll();
        if (grabObject)
        {
            grabObject.transform.parent = transform;
            grabObject.transform.localPosition = grabStartPosition;
            grabObject.transform.localScale = Vector3.one;
        }
        GetComponent<CapsuleCollider>().enabled = true;
        myStats.ResetForRebirth();
        controller.ResetForRebirth();
        velBody.ResetForRebirth();
        will.Reset();
        transform.SetParent(poolParent);
        transform.localScale = transformMemento.startScale;
        lastChance = false;
        alreadyStunned = false;        
    }

    public void DisableCollider()
    {
        GetComponent<CapsuleCollider>().enabled = false;
    }

    public void EnableRagdoll()
    {
        if (jointRigidBodies != null)
        {
            //Ignore the first entry (its the self rigidbody)
            for (int i = 1; i < jointRigidBodies.Length; i++)
            {
                jointRigidBodies[i].useGravity = true;
                jointRigidBodies[i].isKinematic = false;
            }
        }
        animator.enabled = false;
    }

    public void DisableRagdoll()
    {
        if (jointRigidBodies != null)
        {
            //Ignore the first entry (its the self rigidbody)
            for (int i = 1; i < jointRigidBodies.Length; i++)
            {
                jointRigidBodies[i].useGravity = false;
                jointRigidBodies[i].isKinematic = true;
                RagdollHandler ragdollHandler = jointRigidBodies[i].GetComponent<RagdollHandler>();
                if (ragdollHandler)
                {
                    ragdollHandler.ResetBone();
                }
            }
        }
        animator.enabled = true;
    }

    public GameObject GetGrabObject()
    {
        return grabObject;
    }
    #endregion

    #region 6. Private Methods
    #endregion
}
