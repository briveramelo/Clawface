// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class SegwayMod : Mod {

    [SerializeField] private Collider aoeCollider;
    [SerializeField] private Collider attackCollider;

    [SerializeField] private VFXSegway segwayVFX;

    [SerializeField] private float speedBoostValue;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rangedAccuracyLoss;

    [SerializeField] private float pushDamage;
    [SerializeField] private float pushForce;
    [SerializeField] private float pushTime;

    [SerializeField] private float aoeDamage;
    [SerializeField] private float aoeForce;
    [SerializeField] private float aoeTime;


    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";
    
    private bool isPushing;
    private bool isAoeAttacking;   

    private void Awake()
    {
        type = ModType.ForceSegway;
    }

    public override void Activate()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                ForcePush();   
                break;
            case ModSpot.ArmR:
                ForcePush();
                break;
            case ModSpot.Legs:
                AoeAttack();
                if (wielderMovable.IsGrounded()) {
                    Jump();
                }
                break;
            default:
                break;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player")
        {            
            if (isPushing || isAoeAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();
                float damage = isPushing ? pushDamage : aoeDamage;
                float force = isPushing ? pushForce : aoeForce;

                if (damageable != null && !recentlyHitEnemies.Contains(damageable))
                {
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
            wielderStats.Modify(StatType.MoveSpeed, speedBoostValue);
            wielderStats.Modify(StatType.RangedAccuracy, rangedAccuracyLoss);
            this.wielderMovable = wielderMovable;
            this.wielderMovable.SetMovementMode(MovementMode.ICE);
        }
        else
        {
            segwayVFX.SetMoving(false);
        }
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
            wielderStats.Modify(StatType.MoveSpeed, 1f / speedBoostValue);
            wielderStats.Modify(StatType.RangedAccuracy, 1f / rangedAccuracyLoss);
            this.wielderMovable.SetMovementMode(MovementMode.PRECISE);
        }        
    }

    void BoostSpeed()
    {
        wielderStats.Modify(StatType.MoveSpeed, speedBoostValue);
    }

    void RemoveSpeedBoost()
    {
        wielderStats.Modify(StatType.MoveSpeed, 1 / speedBoostValue);
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


    // Use this for initialization
    void Start () {
        setModType(ModType.ForceSegway);
	}
	
	// Update is called once per frame
	void Update () {        
    }

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        
    }

    void Jump() {
        wielderMovable.AddDecayingForce(Vector3.up * jumpForce);
    }
}
