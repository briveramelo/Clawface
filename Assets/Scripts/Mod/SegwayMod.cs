// Adam Kay

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

    private List<Transform> attackedGameobjects;

    public override void Activate()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                Hit();
                
                break;
            case ModSpot.ArmR:
                Hit();
                Debug.Log("Hit");
                break;
            case ModSpot.Head:
                break;
            case ModSpot.Legs:
                AoeAttack();
                Debug.Log("Aoe");
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking)
        {
            Debug.Log("Attack");
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
            Debug.Log("Aoe");
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
                if (Input.GetButton(Strings.LEFT))
                {
                    Activate();
                    Debug.Log("Hit");
                }
                break;
            case ModSpot.ArmR:
                if (Input.GetButton(Strings.RIGHT))
                {
                    Activate();
                    Debug.Log("Hit");
                }
                break;
            case ModSpot.Legs:
                if (Input.GetButton(Strings.DOWN))
                {
                    Activate();
                    Debug.Log("Aoe");
                }
                break;
            default:
                break;
        }
    }
}
