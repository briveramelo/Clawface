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

    bool isSwinging;

    [SerializeField]
    private VFXStunBatonImpact impactEffect;

    public override void Activate()
    {
        switch (getModSpot())
        {
            case ModSpot.ArmL:
                Swing();
                break;
            case ModSpot.ArmR:
                Swing();
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
    }

    void Swing()
    {        
        if (!isSwinging)
        {
            AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing);
            
            isSwinging = true;
            StartCoroutine(HitCoolDown());
        }
    }

    IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(attackTime);
        recentlyHitEnemies.Clear();
        isSwinging = false;
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
        if (isSwinging)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null && !recentlyHitEnemies.Contains(damageable))
            {
                if (transform.root.CompareTag(Strings.Tags.PLAYER))
                {
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
        wielderStats.Modify(StatType.Attack, attackBoostValue);
    }

    void RemoveAttackBoost()
    {
        wielderStats.Modify(StatType.Attack, 1 / attackBoostValue);
    }

    public override void AttachAffect(ref Stats i_playerStats, ref MoveState playerMovement)
    {
        attackCollider.enabled = true;
        wielderStats = i_playerStats;
        pickupCollider.enabled = false;
        attackValue = wielderStats.GetStat(StatType.Attack);
    }

    public override void DetachAffect()
    {
        attackCollider.enabled = false;
        pickupCollider.enabled = true;
        attackValue = 0.0f;
    }

    public override void AlternateActivate()
    {
        
    }
}
