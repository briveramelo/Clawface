// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegwayMod : Mod {

    [SerializeField] private Collider aoeCollider;
    [SerializeField] private Collider attackCollider;

    [SerializeField] private VFXBlasterShoot shootEffect;
    [SerializeField] private VFXSegway segwayVFX;

    [SerializeField] private float speedBoostValue;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rangedAccuracyLoss;
    [SerializeField] private float attackTime;
    [SerializeField] private float attackDamageMod;
    [SerializeField] private float attackForce;
    [SerializeField] private float aoeTime;
    [SerializeField] private float aoeDamageMod;
    [SerializeField] private float aoeForce;


    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";
    
    private bool isAttacking;
    private bool isAoeAttacking;
    private float attackValue;

    private void Awake()
    {
        type = ModType.ForceSegway;
    }

    public override void Activate()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                Hit();   
                break;
            case ModSpot.ArmR:
                Hit();
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
            if (isAttacking || isAoeAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();
                float damage = isAttacking ? attackDamageMod : aoeDamageMod;
                float force = isAttacking ? attackForce : aoeForce;

                if (damageable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    damageable.TakeDamage(attackValue * damage);
                }
                if (movable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    Vector3 normalizedDistance = (other.transform.position - this.transform.position).normalized;
                    movable.AddDecayingForce(normalizedDistance * force);
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
            attackValue = wielderStats.GetStat(StatType.Attack);
            this.wielderMovable = wielderMovable;
            this.wielderMovable.SetMovementMode(MovementMode.ICE);
        }
        else
        {
            attackValue = wielderStats.GetStat(StatType.Attack);
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
            attackValue = wielderStats.GetStat(StatType.Attack);
            this.wielderMovable.SetMovementMode(MovementMode.PRECISE);
        }
        else
        {
            attackValue = 0.0f;
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

    void Hit()
    {
        AudioManager.Instance.PlaySFX(SFXType.ForceSegwayPush);
        shootEffect.Emit();
        EnableAttackCollider();
        isAttacking = true;
        recentlyHitEnemies.Clear();
        Invoke(SegwayMod.DISABLEATTACKCOLLIDER, attackTime);
    }

    void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
        isAttacking = false;
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
