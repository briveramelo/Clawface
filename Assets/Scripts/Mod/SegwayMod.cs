using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegwayMod : Mod {

    [SerializeField]
    private float speedBoostValue;
    private float speedValue;
    private float attackValue;

    [SerializeField]
    private Collider pickupCollider;

    [SerializeField]
    private Collider attackCollider;

    [SerializeField]
    private float attackTime;

    [SerializeField]
    private float attackDamageMod;

    [SerializeField]
    private Vector3 attackForce;

    [SerializeField]
    private Collider aoeCollider;

    [SerializeField]
    private float aoeTime;

    [SerializeField]
    private float aoeDamageMod;

    [SerializeField]
    private Vector3 aoeForce;

    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";

    private bool isAttacking;
    private bool isAoeAttacking;


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
            case ModSpot.Head:
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
        if (isAttacking)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            IMovable movable = other.GetComponent<IMovable>();
            damageable.TakeDamage(attackValue * attackDamageMod);
            movable.AddExternalForce()
        }
        else if (isAoeAttacking)
        {
            
        }
    }

    public override void AttachAffect(ref Stats i_playerStats)
    {
        //TODO:Disable pickup collider
        playerStats = i_playerStats;
        pickupCollider.enabled = false;
        if (getModSpot() == ModSpot.Head)
        {
            BoostSpeed();
        }
        else
        {
            attackValue = playerStats.GetStat(StatType.Attack);
        }
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect()
    {
        pickupCollider.enabled = true;

        if (getModSpot() == ModSpot.Head)
        {
            RemoveSpeedBoost();
        }
        else
        {
            attackValue = 0.0f;
        }
    }

    void BoostSpeed()
    {
        playerStats.Modify(StatType.MoveSpeed, speedBoostValue);
    }

    void RemoveSpeedBoost()
    {
        playerStats.Modify(StatType.MoveSpeed, 1 / speedBoostValue);
    }

    void AoeAttack()
    {
        EnableAoeCollider();
        Invoke(SegwayMod.DISABLEAOECOLLIDER, aoeTime);
    }

    void Hit()
    {
        EnableAttackCollider();
        Invoke(SegwayMod.DISABLEATTACKCOLLIDER, attackTime);
    }

    void EnableAttackCollider()
    {
        attackCollider.enabled = true;
        isAttacking = true;
    }

    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
        isAttacking = false;
    }

    void EnableAoeCollider()
    {
        aoeCollider.enabled = true;
        isAoeAttacking = true;
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
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                if (Input.GetButton(Strings.LEFT))
                {
                    Activate();
                }
                break;
            case ModSpot.ArmR:
                if (Input.GetButton(Strings.RIGHT))
                {
                    Activate();
                }
                break;
            case ModSpot.Legs:
                if (Input.GetButton(Strings.DOWN))
                {
                    Activate();
                }
                break;
            default:
                break;
        }
    }
}
