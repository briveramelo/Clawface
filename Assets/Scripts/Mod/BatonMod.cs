using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonMod : Mod {

    [SerializeField]
    private float attackBoostValue;
    private float attackValue;
    private float attackTime = 0.5f;

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
                    if (Input.GetButton(Strings.LEFT) || Input.GetAxis(Strings.LEFTTRIGGER) != 0)
                    {
                        Activate();
                    }
                    break;
                case ModSpot.ArmR:
                    if (Input.GetButton(Strings.RIGHT) || Input.GetAxis(Strings.RIGHTTRIGGER) != 0)
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
            StartCoroutine(HitCoolDown());
        }
    }

    IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(attackTime);
        recentlyHitEnemies.Clear();
        isHitting = false;
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
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (!recentlyHitEnemies.Contains(damageable)) {
                damageable.TakeDamage(attackValue);
                other.gameObject.GetComponent<IStunnable>().Stun();
                recentlyHitEnemies.Add(damageable);
            }                        
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

    public override void AttachAffect(ref Stats i_playerStats, ref PlayerMovement playerMovement)
    {
        //TODO:Disable pickup collider
        attackCollider.enabled = true;
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
        attackCollider.enabled = false;
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
