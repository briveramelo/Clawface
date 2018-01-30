using System;
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
    #endregion

    #region Private Fields
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
            GameObject shootEffect = ObjectPool.Instance.GetObject (PoolObjectType.VFXGeyserShoot);
            if (shootEffect)
            {
                shootEffect.transform.position = muzzle.position;
                shootEffect.transform.rotation = muzzle.rotation;
            }
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
            projectile.GetComponent<GeyserFissure>().Initialize(fissureSpeed, damage, fissureLiveTime);
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
