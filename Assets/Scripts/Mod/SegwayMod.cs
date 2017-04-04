﻿// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class SegwayMod : Mod {

    [SerializeField] private Collider aoeCollider;
    [SerializeField] private Collider attackCollider;

    [SerializeField] private VFXSegway segwayVFX;

    [SerializeField] private float speedBoostMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rangedAccuracyLoss;

    [SerializeField] private float pushDamageMultiplier;
    [SerializeField] private float pushForce;
    [SerializeField] private float pushTime;

    [SerializeField] private float aoeDamageMultiplier;
    [SerializeField] private float aoeForce;
    [SerializeField] private float aoeTime;


    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";

    private bool canActivate;
    private bool isPushing;
    private bool isAoeAttacking;   

    private void Awake()
    {
        type = ModType.ForceSegway;
    }

    // Use this for initialization
    void Start()
    {
        setModType(ModType.ForceSegway);
    }

    void Update()
    {
        if (wielderMovable != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                //transform.up = -wielderMovable.GetForward();
            }
        }
    }    
    
    private void OnTriggerStay(Collider other)
    {
        if (GetWielderInstanceID() != other.gameObject.GetInstanceID())
        {            
            if (isPushing || isAoeAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();
                float damage = isPushing ? pushDamageMultiplier : aoeDamageMultiplier;
                float force = isPushing ? pushForce : aoeForce;

                if (damageable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    if (this.transform.root.tag == Strings.Tags.PLAYER)
                    {
                        AnalyticsManager.Instance.AddModDamage(this.getModType(), (wielderStats.GetStat(StatType.Attack) * damage));
                    }
                    else
                    {
                        AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), (wielderStats.GetStat(StatType.Attack) * damage));
                    }
                    damageable.TakeDamage(wielderStats.GetStat(StatType.Attack) * damage);
                }
                if (movable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    Vector3 pushDirection = (other.transform.position - this.transform.position).normalized;
                    movable.AddDecayingForce(pushDirection * force);
                }
                if (damageable != null || movable != null)
                {
                    recentlyHitEnemies.Add(damageable);
                }
            }            
        }
    }

    public override void AttachAffect(ref Stats i_playerStats, IMovable wielderMovable)
    {
        this.wielderMovable = wielderMovable;
        wielderStats = i_playerStats;
        pickupCollider.enabled = false;
        segwayVFX.SetIdle(false);

        //TODO ask art to set default rotation to this value
        transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
        transform.localPosition = new Vector3(0f, 0.015f, 0.09f);
        //

        if (getModSpot() == ModSpot.Legs)
        {
            segwayVFX.SetMoving(true);
            wielderStats.Modify(StatType.MoveSpeed, speedBoostMultiplier);
            wielderStats.Modify(StatType.RangedAccuracy, rangedAccuracyLoss);
            this.wielderMovable = wielderMovable;
            this.wielderMovable.SetMovementMode(MovementMode.ICE);
        }
        else
        {
            segwayVFX.SetMoving(false);
        }
    }

    public override void Activate()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                if (!isPushing) {
                    ForcePush();
                }
                break;
            case ModSpot.ArmR:
                if (!isPushing) {
                    ForcePush();
                }
                break;
            case ModSpot.Legs:
                if (!isAoeAttacking) {
                    AoeAttack();
                }
                if (wielderMovable.IsGrounded())
                {
                    Jump();
                }
                break;
        }
        
    }
    public override void AlternateActivate(bool isHeld, float holdTime)
    {

    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect()
    {
        pickupCollider.enabled = true;
        segwayVFX.SetMoving(false);
        segwayVFX.SetIdle(true);
        if (getModSpot() == ModSpot.Legs)
        {
            wielderStats.Modify(StatType.MoveSpeed, 1f / speedBoostMultiplier);
            wielderStats.Modify(StatType.RangedAccuracy, 1f / rangedAccuracyLoss);
            this.wielderMovable.SetMovementMode(MovementMode.PRECISE);
        }        
    }

    void BoostSpeed()
    {
        wielderStats.Modify(StatType.MoveSpeed, speedBoostMultiplier);
    }

    void RemoveSpeedBoost()
    {
        wielderStats.Modify(StatType.MoveSpeed, 1 / speedBoostMultiplier);
    }

    void AoeAttack()
    {
        AudioManager.Instance.PlaySFX(SFXType.ForceSegwayPush);
        EnableAoeCollider();
        isAoeAttacking = true;
        recentlyHitEnemies.Clear();
        Invoke(SegwayMod.DISABLEAOECOLLIDER, aoeTime);
    }

    void ForcePush()
    {
        AudioManager.Instance.PlaySFX(SFXType.ForceSegwayPush);
        GameObject blasterFX = ObjectPool.Instance.GetObject(PoolObjectType.VFXSegwayBlaster);
        if (blasterFX) {
            blasterFX.DeActivate(1.1f);
            blasterFX.transform.position = transform.position;
            blasterFX.transform.forward = -transform.up;
        }
        EnableAttackCollider();
        isPushing = true;
        recentlyHitEnemies.Clear();
        Invoke(SegwayMod.DISABLEATTACKCOLLIDER, pushTime);
    }

    void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
        isPushing = false;
    }

    void EnableAoeCollider()
    {
        aoeCollider.enabled = true;
    }

    void DisableAoeCollider()
    {
        aoeCollider.enabled = false;
        isAoeAttacking = false;
    }    

    void Jump() {
        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
    }
}
