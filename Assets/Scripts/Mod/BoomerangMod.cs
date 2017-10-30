﻿using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMod : Mod {

    #region Serialized
    [SerializeField] private Transform bulletSpawnPoint;
    #endregion

    #region Privates
    private ShooterProperties shooterProperties = new ShooterProperties();
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        type = ModType.Boomerang;
        category = ModCategory.Ranged;
    }


    // Update is called once per frame
    protected override void Update()
    {
        if (wielderMovable != null)
        {
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }
    #endregion

    #region Inherited
    public override void Activate(Action onCompleteCoolDown = null, Action onActivate = null)
    {
        onActivate = () => { SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position); };
        base.Activate(onCompleteCoolDown, onActivate);
        //SFXManager.Instance.Stop(SFXType.BlasterCharge);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
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

    protected override void DoWeaponActions()
    {
        Shoot();
    }
    #endregion


    private BoomerangBullet Shoot()
    {
        SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position);
        PoolObjectType poolObjType = PoolObjectType.VFXBlasterShoot;
        GameObject vfx = ObjectPool.Instance.GetObject(poolObjType);
        vfx.transform.position = bulletSpawnPoint.position;
        vfx.transform.rotation = transform.rotation;
        BoomerangBullet bullet = SpawnBullet();
        return bullet;
    }

    private BoomerangBullet SpawnBullet()
    {
        PoolObjectType poolObjType = PoolObjectType.BoomerangProjectile;
        BoomerangBullet boomerangBullet = ObjectPool.Instance.GetObject(poolObjType).GetComponent<BoomerangBullet>();
        if (boomerangBullet)
        {
            boomerangBullet.transform.position = bulletSpawnPoint.position;
            boomerangBullet.transform.forward = this.transform.forward;
            boomerangBullet.transform.rotation = Quaternion.Euler(0f, boomerangBullet.transform.rotation.eulerAngles.y, 0f);
            shooterProperties.Initialize(GetWielderInstanceID(), Attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
            boomerangBullet.SetShooterProperties(shooterProperties);

            if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
            {
                boomerangBullet.SetShooterType(false);
            }
            else
            {
                boomerangBullet.SetShooterType(true);
            }
        }
        return boomerangBullet;
    }
}
