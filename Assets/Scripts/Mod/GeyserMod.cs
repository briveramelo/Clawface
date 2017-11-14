﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

public class GeyserMod : Mod {

    #region Serialized Unity Inspector fields
    [SerializeField] private float geyserStartDistanceOffset;
    [SerializeField] private float fissureSpeed;    
    [SerializeField] private float fissureLiveTime;
    #endregion

    #region Private Fields
    const float SHOOT_OFFSET = 0.0f;
    private Animator animator;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        animator = GetComponentInChildren<Animator>();
        base.Awake();
    }

    protected override void Update()
    {
        if (wielderMovable != null)
        {
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }

    #endregion

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        base.Activate(onCompleteCoolDown, onActivate);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
        
    }

    public override void DeActivate() { }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }
    
    #endregion


    #region Private Methods    
    protected override void DoWeaponActions() { Erupt(); }

    private void Erupt()
    {
        
        if (GetGeyser())
        {
            SFXManager.Instance.Play(shootSFX, transform.position);
        }
    }

    private GameObject GetGeyser()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserFissure);
        if (projectile)
        {
            projectile.transform.position = transform.position + transform.forward * SHOOT_OFFSET;
            projectile.transform.forward = transform.forward;
            projectile.transform.rotation = Quaternion.Euler(0f, projectile.transform.rotation.eulerAngles.y, 0f);
            projectile.GetComponent<GeyserFissure>().Initialize(fissureSpeed, damage, fissureLiveTime);
        }

        animator.SetTrigger("Shoot");
        return projectile;
    }

    #endregion

    #region Private Structures
    #endregion

}
