using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonMod : Mod {

    [SerializeField]
    private float attackBoostValue;
    private float attackValue;

    [SerializeField]
    private Collider pickupCollider;

    [SerializeField]
    private Collider attackCollider;
    bool isHitting;

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
                LayMine();
                break;
            default:
                break;
        }
    }

    public override void DeActivate()
    {
        //Nothing to do here
    }

    // Use this for initialization
    void Start () {
        setModType(ModType.StunBaton);
        attackCollider.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetButton(Strings.PREPARETOPICKUPORDROP) && !Input.GetButton(Strings.PREPARETOSWAP))
        {
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
                case ModSpot.Head:
                    Activate();
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

    void Hit()
    {        
        if (!isHitting)
        {
            isHitting = true;
            print("Hitting");
            StartCoroutine(HitCoolDown());
        }
    }

    IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isHitting = false;
        print("Not Hitting");
    }

    void LayMine()
    {
        GameObject stunMine = BulletPool.instance.getStunMine();
        if (stunMine != null)
        {
            stunMine.transform.position = transform.position;
            stunMine.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Strings.ENEMY && isHitting)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(attackValue);
        }
    }

    void BoostAttack()
    {
        playerStats.Modify(StatType.Attack, attackBoostValue);
    }

    void RemoveAttackBoost()
    {
        playerStats.Modify(StatType.Attack, 1 / attackBoostValue);
    }

    public override void AttachAffect(ref Stats i_playerStats)
    {
        //TODO:Disable pickup collider
        playerStats = i_playerStats;
        pickupCollider.enabled = false;
        if (getModSpot() == ModSpot.Head)
        {
            BoostAttack();
        }else
        {
            attackValue = playerStats.GetStat(StatType.Attack);
        }
    }

    public override void DetachAffect()
    {
        //TODO:Enable pickup collider
        pickupCollider.enabled = true;
        if (getModSpot() == ModSpot.Head)
        {
            RemoveAttackBoost();
        }else
        {
            attackValue = 0.0f;
        }
    }
}
