﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterMod : Mod {
    
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private PoolObjectType projectileBullet;

    private ShooterProperties shooterProperties= new ShooterProperties();
    private Animator animator;

    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        type = ModType.Blaster;
        category = ModCategory.Ranged;   
        animator = GetComponentInChildren<Animator>();
    }

    
    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null){
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        onActivate = ()=> { SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position);};
        base.Activate(onCompleteCoolDown, onActivate);
        //SFXManager.Instance.Stop(SFXType.BlasterCharge);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);        
    }

    public override void DeActivate()
    {
        //Nothing to do
    }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }
    
    protected override void DoWeaponActions(){
        Shoot();
    }    

    private BlasterBullet Shoot(){
        SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position);
        PoolObjectType poolObjType = PoolObjectType.VFXBlasterShoot;
        GameObject vfx = ObjectPool.Instance.GetObject(poolObjType);
        vfx.transform.position = bulletSpawnPoint.position;
        vfx.transform.rotation = transform.rotation;
        if (animator != null) animator.SetTrigger("Shoot");
        BlasterBullet bullet = SpawnBullet();        
        return bullet;
    }

    private BlasterBullet SpawnBullet()
    {
        PoolObjectType poolObjType = projectileBullet;
        BlasterBullet blasterBullet = ObjectPool.Instance.GetObject(poolObjType).GetComponent<BlasterBullet>();


        if (blasterBullet){
            blasterBullet.transform.position = bulletSpawnPoint.position;
            blasterBullet.transform.forward = transform.forward;
            blasterBullet.transform.rotation = Quaternion.Euler(0f, blasterBullet.transform.rotation.eulerAngles.y, 0f);

            shooterProperties.Initialize(GetWielderInstanceID(), Attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
            blasterBullet.SetShooterProperties(shooterProperties);

            if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
            {
                blasterBullet.SetShooterType(false);
            }
            else
            {
                blasterBullet.SetShooterType(true);
            }
        }
        else
        {
            Debug.LogWarning("Blaster mod " + transform.name + " is trying to spawn a bullet that does not have a BlasterBullet component on it.");
        }
        return blasterBullet;
    }
}
