﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMod : Mod {

    [SerializeField] private float missileSpeed;

    [SerializeField] private float closeDamage;
    [SerializeField] private float farDamage;

    [SerializeField] private float closeRadius;
    [SerializeField] private float farRadius;

    [SerializeField] private float projectileLifetime;

    public override void DeActivate()
    {
    }

    protected override void Awake()
    {
        type = ModType.Dice;
        category = ModCategory.Ranged;
        base.Awake();
    }

    protected override void DoWeaponActions()
    {
        Fire();
    }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }
    
    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	protected override void Update () {
        if (wielderMovable != null)
        {
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }

    public void Fire()
    {
        if (GetMissile())
        {
            SFXManager.Instance.Play(shootSFX, transform.position);
            // FinishFiring();
        }
    }

    private GameObject GetMissile()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.MissileProjectile);
        if (projectile)
        {
            projectile.transform.position = transform.position;
            projectile.transform.forward = transform.forward;
            projectile.transform.rotation = Quaternion.Euler(0f, projectile.transform.rotation.eulerAngles.y, 0f);
            projectile.GetComponent<Missile>().Init(missileSpeed, closeRadius, farRadius, closeDamage, farDamage, projectileLifetime);
        }

        return projectile;
    }
}