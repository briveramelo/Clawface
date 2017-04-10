﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterMod : Mod {

    [SerializeField] private VFXBlasterShoot blasterEffect;
    [SerializeField] private Transform bulletSpawnPoint;

    [SerializeField] private float kickbackForce;
    [SerializeField] private float kickbackForceCharged;
    [SerializeField] private float kickbackForceFeetMultiplier;

    private ShooterProperties shooterProperties= new ShooterProperties();

    // Use this for initialization
    void Start () {
        type = ModType.ArmBlaster;
        category = ModCategory.Ranged;             
    }

    
    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null){
            if (getModSpot() != ModSpot.Legs){
                transform.forward = wielderMovable.GetForward();
            }
        }
    }

    public override void Activate(Action onComplete=null){
        base.Activate(onComplete);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
        shooterProperties.Initialize(GetWielderInstanceID(),wielderStats.attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
    }

    public override void DeActivate()
    {
        //Nothing to do
    }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }

    protected override void ActivateStandardArms(){
        Shoot();
        FireKickBack();
    }

    protected override void ActivateChargedArms(){
        ActivateStandardArms();
    }

    protected override void ActivateStandardLegs(){
        ActivateStandardArms();
    }

    protected override void ActivateChargedLegs(){
        ActivateStandardArms();
    }

    private BlasterBullet Shoot(){
        AudioManager.Instance.PlaySFX(SFXType.BlasterShoot);
        blasterEffect.Emit();
        BlasterBullet bullet = SpawnBullet();        
        return bullet;
    }

    private BlasterBullet SpawnBullet()
    {
        BlasterBullet blasterBullet = ObjectPool.Instance.GetObject(PoolObjectType.BlasterBullet).GetComponent<BlasterBullet>();
        if (blasterBullet){
            blasterBullet.transform.position = bulletSpawnPoint.position;
            blasterBullet.transform.rotation = transform.rotation;
            blasterBullet.SetShooterProperties(shooterProperties);
        }
        return blasterBullet;
    }

    private void FireKickBack() {        
        if (wielderMovable != null){            
            wielderMovable.AddDecayingForce(KickBackDirection * KickBack);
        }
    }

    private float KickBack {
        get{
            float force = energySettings.IsCharged ? kickbackForceCharged : kickbackForce;
            if (getModSpot()==ModSpot.Legs) {                
                return kickbackForceFeetMultiplier * force;
            }            
            return force;
        }
    }
    private Vector3 KickBackDirection {
        get {
            if (getModSpot() == ModSpot.Legs){
                return Vector3.up;
            }                        
            return -wielderMovable.GetForward();            
        }
    }
}
