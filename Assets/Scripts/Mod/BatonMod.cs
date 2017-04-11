﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonMod : Mod {

    [SerializeField] private Collider attackCollider;
    [SerializeField] private Transform batonTip;
    [SerializeField] private VFXStunBatonImpact impactEffect;
    [SerializeField] private Rigidbody rigbod;
    [SerializeField] private float attackBoostValue;

    private VFXHandler vfxHandler;
    private float attackValue;
    private float attackTime = 1f;
    private bool isSwinging;

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

    public override void ActivateModCanvas()
    {
        if (modCanvas && !isAttached)
        {
            modCanvas.SetActive(true);
        }
    }

    public override void DeactivateModCanvas()
    {
        if (modCanvas)
        {
            modCanvas.SetActive(false);
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
        vfxHandler = new VFXHandler(transform);
        if (modCanvas) { modCanvas.SetActive(false); }       
    }

    void Swing()
    {        
        if (!isSwinging)
        {
            //AudioManager.Instance.PlaySFX(SFXType.StunBatonSwing);
            
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
            //AudioManager.Instance.PlaySFX(SFXType.StunBatonLayMine);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isSwinging){
            if (GetWielderInstanceID() != other.gameObject.GetInstanceID() &&
                (other.gameObject.CompareTag(Strings.Tags.PLAYER) ||
                other.gameObject.CompareTag(Strings.Tags.ENEMY))){

                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null && !recentlyHitEnemies.Contains(damageable)){
                    AudioManager.Instance.PlaySFX(SFXType.StunBatonImpact);

                    if (transform.root.CompareTag(Strings.Tags.PLAYER)){ 
                        HitstopManager.Instance.StartHitstop(.05f);
                        AnalyticsManager.Instance.AddModDamage(this.getModType(), attackValue);

                        if (damageable.GetHealth() <= 0.01f)
                        {
                            AnalyticsManager.Instance.AddModKill(this.getModType());
                        }
                    }
                    else
                    {
                        AnalyticsManager.Instance.AddEnemyModDamage(this.getModType(), attackValue);

                    }
                    if (!other.CompareTag(Strings.Tags.PLAYER)) {
                        EmitBlood();
                    }
                    impactEffect.Emit();
                    damageable.TakeDamage(attackValue);
                    recentlyHitEnemies.Add(damageable);
                    IStunnable stunnable = other.GetComponent<IStunnable>();
                    if (stunnable != null){
                        stunnable.Stun();
                    }
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

    public override void AttachAffect(ref Stats i_playerStats, IMovable wielderMovable)
    {
        isAttached = true;
        attackCollider.enabled = true;
        wielderStats = i_playerStats;
        pickupCollider.enabled = false;
        attackValue = wielderStats.GetStat(StatType.Attack);
   
    }

    public override void DetachAffect()
    {
        isAttached = false;
        attackCollider.enabled = false;
        pickupCollider.enabled = true;
        attackValue = 0.0f;
    }

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        
    }

    private void EmitBlood() {
        Vector3 bloodDirection = transform.root.rotation.eulerAngles;
        bloodDirection.x = 23.38f;
        Quaternion emissionRotation = Quaternion.Euler(bloodDirection);
        vfxHandler.EmitBloodInDirection(emissionRotation, transform.position);
    }
}
