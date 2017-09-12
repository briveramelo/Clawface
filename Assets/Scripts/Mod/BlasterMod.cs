using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterMod : Mod {
    
    [SerializeField] private Transform bulletSpawnPoint;

    [SerializeField] private float kickbackForce;
    [SerializeField] private float kickbackForceCharged;
    [SerializeField] private float kickbackForceFeetMultiplier;

    private ShooterProperties shooterProperties= new ShooterProperties();

    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        type = ModType.ArmBlaster;
        category = ModCategory.Ranged;             
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
        SFXManager.Instance.Stop(SFXType.BlasterCharge);
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

    public override void BeginCharging(Action onBegin=null){
        onBegin=()=> {  
            SFXManager.Instance.Play(SFXType.BlasterCharge, transform.position);
            };
        base.BeginCharging(onBegin);
    }
    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }
    
    protected override void ActivateStandardArms(){
        Shoot();
        FireKickBack();
    }
    protected override void ActivateChargedArms(){
        Shoot();
        FireKickBack();
    }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateChargedLegs(){
        Shoot();
        FireKickBack();
    }
    protected override void ActivateStandardLegs(){
        Shoot();
        FireKickBack();
    }    

    private BlasterBullet Shoot(){
        SFXManager.Instance.Play(SFXType.BlasterShoot, transform.position);
        PoolObjectType poolObjType = IsCharged() ? PoolObjectType.VFXBlasterShootCharged: PoolObjectType.VFXBlasterShoot;
        GameObject vfx = ObjectPool.Instance.GetObject(poolObjType);
        vfx.transform.position = bulletSpawnPoint.position;
        vfx.transform.rotation = transform.rotation;
        BlasterBullet bullet = SpawnBullet();        
        return bullet;
    }

    private BlasterBullet SpawnBullet()
    {
        PoolObjectType poolObjType = IsCharged() ? PoolObjectType.BlasterBulletCharged : PoolObjectType.BlasterBullet;
        BlasterBullet blasterBullet = ObjectPool.Instance.GetObject(poolObjType).GetComponent<BlasterBullet>();
        if (blasterBullet){
            blasterBullet.transform.position = bulletSpawnPoint.position;
            blasterBullet.transform.forward = transform.forward;
            blasterBullet.transform.rotation = Quaternion.Euler(0f, blasterBullet.transform.rotation.eulerAngles.y, 0f);

            shooterProperties.Initialize(GetWielderInstanceID(),Attack, wielderStats.shotSpeed, wielderStats.shotPushForce);
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
            return force;
        }
    }
    private Vector3 KickBackDirection {
        get {                   
            return -wielderMovable.GetForward();            
        }
    }
}
