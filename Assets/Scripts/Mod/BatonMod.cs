using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonMod : Mod {

    [SerializeField]
    private float attackBoostValue;
    private float attackValue;
    private float attackTime = 1f;

    [SerializeField]
    private Collider attackCollider;
    bool isHitting;

    [SerializeField]
    private VFXStunBatonImpact impactEffect;

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

    void Awake()
    {
        type = ModType.StunBaton;
        category = ModCategory.Melee;
        attackCollider.enabled = false;
    }

    // Use this for initialization
    void Start () {        
    }
	
	// Update is called once per frame
	void Update () {        
	}

    void Hit()
    {        
        if (!isHitting)
        {
            AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing);
            
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
        GameObject stunMine = ObjectPool.Instance.GetObject(PoolObjectType.Mine);
        if (stunMine != null)
        {
            stunMine.transform.position = transform.position;
            stunMine.SetActive(true);
            AudioManager.Instance.PlaySFX(SFXType.StunBatonLayMine);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isHitting)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null && !recentlyHitEnemies.Contains(damageable))
            {
                if (other.tag != Strings.Tags.PLAYER)
                {
                    //TODO check that the player swinging IS NOT a mallcop...
                    HitstopManager.Instance.StartHitstop(.2f);
                }
                impactEffect.Emit();
                damageable.TakeDamage(attackValue);
                recentlyHitEnemies.Add(damageable);
                IStunnable stunnable = other.gameObject.GetComponent<IStunnable>();
                if (stunnable != null)
                {
                    stunnable.Stun();
                        
                }
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
        attackCollider.enabled = true;
        playerStats = i_playerStats;
        pickupCollider.enabled = false;
        attackValue = playerStats.GetStat(StatType.Attack);
    }

    public override void DetachAffect()
    {
        attackCollider.enabled = false;
        pickupCollider.enabled = true;
        attackValue = 0.0f;
    }
}
