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
            case ModSpot.Head:                
                    Activate();
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

    void Hit()
    {
        //Hit code
        isHitting = true;
    }

    void LayMine()
    {
        //LayCode
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Strings.ENEMY && isHitting)
        {
            //TODO:Do damage
        }
    }

    void BoostAttack()
    {
        Stats stats = GetComponent<Stats>();
        stats.Modify(StatType.Attack, attackBoostValue);
    }

    void RemoveAttackBoost()
    {
        Stats stats = GetComponent<Stats>();
        stats.Modify(StatType.Attack, 1 / attackBoostValue);
    }

    public override void AttachAffect()
    {
        //TODO:Disable pickup collider
        pickupCollider.enabled = false;
        if (getModSpot() == ModSpot.Head)
        {
            BoostAttack();
        }
    }

    public override void DetachAffect()
    {
        //TODO:Enable pickup collider
        pickupCollider.enabled = true;
        if (getModSpot() == ModSpot.Head)
        {
            RemoveAttackBoost();
        }
    }
}
