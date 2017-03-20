// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegwayMod : Mod {

    [SerializeField]
    private float speedBoostValue;

    [SerializeField] private float rangedAccuracyLoss;

    private float speedValue;
    private float attackValue;

    [SerializeField]
    private Collider attackCollider;

    [SerializeField]
    private float attackTime;

    [SerializeField]
    private float attackDamageMod;

    [SerializeField]
    private float attackForce;

    [SerializeField]
    private Collider aoeCollider;

    [SerializeField]
    private float aoeTime;

    [SerializeField]
    private float aoeDamageMod;

    [SerializeField]
    private float aoeForce;

    [SerializeField]
    private VFXBlasterShoot shootEffect;

    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";

    [SerializeField]
    private bool isAttacking;

    [SerializeField]
    private bool isAoeAttacking;

    MoveState playerMovement;

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
                break;
            default:
                break;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player")
        {

            if (isAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();

                if (damageable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    damageable.TakeDamage(attackValue * attackDamageMod);
                }
                if (movable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    Vector3 normalizedDistance = other.transform.position - this.transform.position;
                    normalizedDistance = normalizedDistance.normalized;
                    movable.AddDecayingForce(normalizedDistance * attackForce);
                }

                if (damageable != null || movable != null)
                {
                    recentlyHitEnemies.Add(damageable);
                }
            }
            else if (isAoeAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();

                if (damageable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    damageable.TakeDamage(attackValue * aoeDamageMod);
                }
                if (movable != null && !recentlyHitEnemies.Contains(damageable))
                {
                    Vector3 normalizedDistance = other.transform.position - this.transform.position;
                    normalizedDistance = normalizedDistance.normalized;
                    movable.AddDecayingForce(normalizedDistance * aoeForce);
                }

                if (damageable != null || movable != null)
                {
                    recentlyHitEnemies.Add(damageable);
                }
            }
        }
    }

    public override void AttachAffect(ref Stats i_playerStats, ref MoveState playerMovement)
    {
        //TODO:Disable pickup collider
        wielderStats = i_playerStats;
        pickupCollider.enabled = false;
        if (getModSpot() == ModSpot.Legs)
        {
            wielderStats.Modify(StatType.MoveSpeed, speedBoostValue);
            wielderStats.Modify(StatType.RangedAccuracy, rangedAccuracyLoss);
            attackValue = wielderStats.GetStat(StatType.Attack);
            this.playerMovement = playerMovement;
            this.playerMovement.SetMovementMode(MovementMode.ICE);
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

        if (getModSpot() == ModSpot.Legs)
        {
            wielderStats.Modify(StatType.MoveSpeed, 1f / speedBoostValue);
            wielderStats.Modify(StatType.RangedAccuracy, 1f / rangedAccuracyLoss);
            attackValue = wielderStats.GetStat(StatType.Attack);
            this.playerMovement.SetMovementMode(MovementMode.PRECISE);
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

    public override void AlternateActivate()
    {
        
    }
}
