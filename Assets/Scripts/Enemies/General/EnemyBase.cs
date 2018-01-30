﻿using System.Collections;
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
    [SerializeField] protected Stats myStats;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected NavMeshObstacle navObstacle;
    [SerializeField] protected int eatHealth;
    [SerializeField] protected Transform bloodEmissionLocation;
    [SerializeField] protected int scoreValue = 200;
    [SerializeField] protected int bufferHealth = 3;
    [SerializeField] private GameObject grabObject;
    [SerializeField] private GameObject affectObject;
    [SerializeField] protected HitFlasher hitFlasher;
    [SerializeField] private SFXType hitSFX;
    [SerializeField] private SFXType deathSFX;
    [SerializeField] Rigidbody pushRoot;
    [SerializeField] PushDirection pushDirection;
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
    #endregion

    #region 0. Protected fields
    protected bool alreadyStunned = false;
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

                if (alreadyStunned)
                    lastChance = true;

                if (lastChance && alreadyStunned)
                {
                    OnDeath();
                }
                else
                {
                    myStats.health = bufferHealth;
                    lastChance = true;
                    alreadyStunned = true;
                }
            }


            if (myStats.health <= myStats.skinnableHealth)
            {
                if (!alreadyStunned)
                {
                    
                    myStats.health = bufferHealth;
                    alreadyStunned = true;
                }
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
        bool sucessfulWarp = navAgent.Warp(position);
        if (!sucessfulWarp) {
            Debug.LogWarning("Failed to warp!");
        }
        navAgent.enabled = true;
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
        ragdollOn = true;
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
                if (rigidBodyMasses != null)
                {
                    jointRigidBodies[i].mass = rigidBodyMasses[i];
                }
                RagdollHandler ragdollHandler = jointRigidBodies[i].GetComponent<RagdollHandler>();
                if (ragdollHandler)
                {
                    ragdollHandler.ResetBone();
                }
            }
        }
        animator.enabled = true;
        AIController aiController = GetComponent<AIController>();
        if (aiController)
        {
            aiController.ActivateAI();
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
        if (!ragdollOn)
        {
            DisableCollider();
            EnableRagdoll();
            switch (pushDirection)
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
            Timing.CallDelayed(5.0f, GetUp);
        }
    }

    private void AddForce(Vector3 force)
    {
        if (pushRoot)
        {
            pushRoot.AddForce(force, ForceMode.Impulse);
        }
    }


    #endregion

    #region 6. Private Methods
    private void EnableCollider()
    {
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private void GetUp()
    {
        EnableCollider();
        DisableRagdoll();
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
    #endregion
}
