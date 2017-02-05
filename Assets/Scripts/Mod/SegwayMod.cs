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
    private Collider pickupCollider;

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

    private static string ENABLEATTACKCOLLIDER = "EnableAttackCollider";
    private static string DISABLEATTACKCOLLIDER = "DisableAttackCollider";
    private static string ENABLEAOECOLLIDER = "EnableAoeCollider";
    private static string DISABLEAOECOLLIDER = "DisableAoeCollider";

    [SerializeField]
    private bool isAttacking;

    [SerializeField]
    private bool isAoeAttacking;

    private List<Transform> attackedGameobjects = new List<Transform>();

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
        if (other.tag != "Player")
        {

            if (isAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();

                if (damageable != null && !attackedGameobjects.Contains(other.transform))
                {
                    damageable.TakeDamage(attackValue * attackDamageMod);
                }
                if (movable != null && !attackedGameobjects.Contains(other.transform))
                {
                    Vector3 normalizedDistance = other.transform.position - this.transform.position;
                    normalizedDistance = normalizedDistance.normalized;
                    movable.AddExternalForce(normalizedDistance * attackForce);
                }

                if (damageable != null || movable != null)
                {
                    attackedGameobjects.Add(other.transform);
                }
            }
            else if (isAoeAttacking)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                IMovable movable = other.GetComponent<IMovable>();

                if (damageable != null && !attackedGameobjects.Contains(other.transform))
                {
                    damageable.TakeDamage(attackValue * aoeDamageMod);
                }
                if (movable != null && !attackedGameobjects.Contains(other.transform))
                {
                    Vector3 normalizedDistance = other.transform.position - this.transform.position;
                    normalizedDistance = normalizedDistance.normalized;
                    movable.AddExternalForce(normalizedDistance * aoeForce);
                }

                if (damageable != null || movable != null)
                {
                    attackedGameobjects.Add(other.transform);
                }
            }
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
        else if (getModSpot() == ModSpot.Legs)
        {
            playerStats.Modify(StatType.MoveSpeed, speedBoostValue);
            playerStats.Modify(StatType.RangedAccuracy, rangedAccuracyLoss);
            attackValue = playerStats.GetStat(StatType.Attack);
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
        else if (getModSpot() == ModSpot.Legs)
        {
            playerStats.Modify(StatType.MoveSpeed, 1f / speedBoostValue);
            playerStats.Modify(StatType.RangedAccuracy, 1f / rangedAccuracyLoss);
            attackValue = playerStats.GetStat(StatType.Attack);
        }
        else
        {
            attackValue = 0.0f;
        }
    }

    void BoostSpeed()
    {
        playerStats.Modify(StatType.MoveSpeed, speedBoostValue);
        Debug.Log("Boosted speed value: " + playerStats.GetStat(StatType.MoveSpeed));
    }

    void RemoveSpeedBoost()
    {
        playerStats.Modify(StatType.MoveSpeed, 1 / speedBoostValue);
        Debug.Log("Boosted speed value: " + playerStats.GetStat(StatType.MoveSpeed));
    }

    void AoeAttack()
    {
        EnableAoeCollider();
        isAoeAttacking = true;
        attackedGameobjects.Clear();
        Invoke(SegwayMod.DISABLEAOECOLLIDER, aoeTime);
    }

    void Hit()
    {
        EnableAttackCollider();
        isAttacking = true;
        attackedGameobjects.Clear();
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
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                if (Input.GetButtonDown(Strings.LEFT))
                {
                    Activate();
                }
                break;
            case ModSpot.ArmR:
                if (Input.GetButtonDown(Strings.RIGHT))
                {
                    Activate();
                }
                break;
            case ModSpot.Legs:
                if (Input.GetButtonDown(Strings.DOWN))
                {
                    Activate();
                }
                break;
            default:
                break;
        }
    }
}
