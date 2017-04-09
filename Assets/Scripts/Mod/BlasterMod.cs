using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class BlasterMod : Mod {

    [SerializeField]
    private float rangeBoostValue;

    [SerializeField]
    private float kickbackForce;

    [SerializeField]
    private float coolDownTime;

    [SerializeField]
    private float feetMultiplier;

    [SerializeField]
    private VFXBlasterShoot blasterEffect;

    [SerializeField]
    private float kickBackChargedMultiplier;

    [SerializeField] private Transform bulletSpawnPoint;

    private bool readyToShoot;    

    // Use this for initialization
    void Start () {
        readyToShoot = true;
        type = ModType.ArmBlaster;
        category = ModCategory.Ranged;             
    }

    
    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                transform.forward = wielderMovable.GetForward();
            }
        }
    }

    public override void Activate()
    {
        if (readyToShoot)
        {
            Action shootAction = chargeSettings.isCharged ? (Action)ActivateCharged : ActivateStandard;
            shootAction();
            readyToShoot = false;
            Timing.RunCoroutine(CoolDown());
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        isAttached = true;
        this.wielderMovable = wielderMovable;
        this.wielderStats = wielderStats;
        pickupCollider.enabled = false;
    }

    public override void DeActivate()
    {
        //Nothing to do
    }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }

    IEnumerator<float> CoolDown()
    {
        yield return Timing.WaitForSeconds(coolDownTime);
        readyToShoot = true;
    }

    protected override void ActivateStandard()
    {
        Shoot();
        FireKickBack();
    }

    protected override void ActivateCharged()
    {
        BlasterBullet blasterBullet = Shoot();
        if (blasterBullet){
            blasterBullet.isCharged = true;
        }
        FireKickBack();
    }

    private BlasterBullet Shoot()
    {
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
            blasterBullet.SetShooterInstanceID(GetWielderInstanceID());
        }
        return blasterBullet;
    }

    private void FireKickBack() {        
        if (wielderMovable != null){
            if (getModSpot() == ModSpot.Legs){
                KickBack(Vector3.up * feetMultiplier);
            }
            else{
                KickBack(-wielderMovable.GetForward());
            }
        }
    }

    private void KickBack(Vector3 direction){
        float chargeMult = chargeSettings.isCharged ? kickBackChargedMultiplier : 1f;
        wielderMovable.AddDecayingForce(direction * kickbackForce * chargeMult);
    }

    private void BoostRange()
    {
        //playerStats.Modify(StatType.MiniMapRange, rangeBoostValue);
    }

}
