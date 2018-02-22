using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BlasterMod : Mod {

    #region serialized fields
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletLiveTime;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private PoolObjectType bulletType;
    #endregion

    #region private fields
    private Animator animator;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        type = ModType.Blaster;
        category = ModCategory.Ranged;   
        animator = GetComponentInChildren<Animator>();
    }

    
    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null && wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
        {
            transform.forward = wielderMovable.GetForward();
        }
        base.Update();
    }
    #endregion

    #region public functions
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        onActivate = ()=> { SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position);};
        base.Activate(onCompleteCoolDown, onActivate);
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

    public void SetBulletStats(float newBulletLiveTime, float newBulletSpeed, float newDamage)
    {
        bulletLiveTime = newBulletLiveTime;
        bulletSpeed = newBulletSpeed;
        damage = newDamage;
    }

    
    protected override void DoWeaponActions(){
        Shoot();
    }
    #endregion

    #region private functions
    private BlasterBullet Shoot(){
        SFXManager.Instance.Play(shootSFX, transform.position);
        PoolObjectType poolObjType = PoolObjectType.VFXBlasterShoot;
        GameObject vfx = ObjectPool.Instance.GetObject(poolObjType);
        if (vfx != null)
        {
            vfx.transform.SetParent (transform);
            vfx.transform.position = bulletSpawnPoint.position;
            vfx.transform.rotation = transform.rotation;
        }
        if (animator != null) animator.SetTrigger("Shoot");
        BlasterBullet bullet = SpawnBullet();        
        return bullet;
    }

    private BlasterBullet SpawnBullet()
    {
        BlasterBullet blasterBullet = null;
        GameObject go = ObjectPool.Instance.GetObject(bulletType);
        if (go) {
            blasterBullet = go.GetComponent<BlasterBullet>();
            if (blasterBullet){
                blasterBullet.transform.position = bulletSpawnPoint.position;
                blasterBullet.transform.forward = transform.forward;
                blasterBullet.transform.rotation = Quaternion.Euler(0f, blasterBullet.transform.rotation.eulerAngles.y, 0f);
            
                blasterBullet.Initialize(bulletLiveTime, bulletSpeed, damage);

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
        }
        return blasterBullet;
    }
    #endregion
}
