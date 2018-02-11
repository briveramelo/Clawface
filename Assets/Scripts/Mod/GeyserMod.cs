﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

public class GeyserMod : Mod {

    #region Serialized Unity Inspector fields
    [SerializeField] FishEmitter fishEmitter;
    [SerializeField] private float geyserStartDistanceOffset;
    [SerializeField] private float fissureSpeed;
    [SerializeField] private FloatRange fishLaunchSpeed;
    [SerializeField] private float fissureLiveTime;
    [SerializeField] private Transform muzzle;

    [SerializeField]
    private float timeToCharge;
    [SerializeField]
    private float chargeDamage;
    [SerializeField]
    private float chargeSpeed;
    #endregion

    #region Private Fields
    private Animator animator;
    private bool isCharged;
    private string geyserCoroutineString = "geyserModCharge";
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        animator = GetComponentInChildren<Animator>();
        base.Awake();
    }

    protected void Start()
    {
        Timing.KillCoroutines(geyserCoroutineString);
        Timing.RunCoroutine(Charge(), geyserCoroutineString);
    }

    protected override void Update()
    {

        /* Again, do we need this anymore?
        if (wielderMovable != null)
        {
            transform.forward = wielderMovable.GetForward();
        }
        */
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
    private IEnumerator<float> Charge()
    {
        yield return Timing.WaitForSeconds(timeToCharge);
        isCharged = true;
    }

    protected override void DoWeaponActions() { Erupt(); }

    private void Erupt()
    {
        
        if (GetGeyser())
        {
            SFXManager.Instance.Play(shootSFX, transform.position);
            GameObject shootEffect = ObjectPool.Instance.GetObject (PoolObjectType.VFXGeyserShoot);
            if (shootEffect)
            {
                shootEffect.transform.SetParent (muzzle);
                shootEffect.transform.position = muzzle.position;
                shootEffect.transform.rotation = muzzle.rotation;
            }
            Timing.KillCoroutines(geyserCoroutineString);
            Timing.RunCoroutine(Charge(), geyserCoroutineString);
        }
    }

    private GameObject GetGeyser()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserFissure);
        if (projectile)
        {
            projectile.transform.position = transform.position + transform.forward * geyserStartDistanceOffset;
            projectile.transform.forward = transform.forward;
            projectile.transform.rotation = Quaternion.Euler(0f, projectile.transform.rotation.eulerAngles.y, 0f);

            if (isCharged)
            {
                projectile.GetComponent<GeyserFissure>().Initialize(chargeSpeed, chargeDamage, fissureLiveTime);
            }
            else
            {
                projectile.GetComponent<GeyserFissure>().Initialize(fissureSpeed, damage, fissureLiveTime);
            }
            isCharged = false;
            fishEmitter.SetBaseEmissionSpeed(fishLaunchSpeed.GetRandomValue());
            fishEmitter.Play();
        }

        animator.SetTrigger("Shoot");
        return projectile;
    }

    #endregion

    #region Private Structures
    #endregion

}
