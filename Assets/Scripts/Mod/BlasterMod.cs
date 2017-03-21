using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterMod : Mod {

    [SerializeField]
    private float rangeBoostValue;

    [SerializeField]
    private float kickbackMagnitude;


    [SerializeField]
    private float coolDownTime;

    [SerializeField]
    private float feetMultiplier;

    [SerializeField]
    VFXBlasterShoot blasterEffect;

    [SerializeField] private Transform bulletSpawnPoint;

    private bool readyToShoot;


    public override void Activate()
    {
        if (readyToShoot)
        {
            Shoot();
            readyToShoot = false;
            StartCoroutine(CoolDown());
        }
    }


    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDownTime);
        readyToShoot = true;
    }

    void Shoot()
    {
        AudioManager.Instance.PlaySFX(SFXType.ArmBlasterFire);
        GameObject blasterBullet = ObjectPool.Instance.GetObject(PoolObjectType.BlasterBullet);
        blasterBullet.transform.position = bulletSpawnPoint.position;
        blasterBullet.transform.rotation = transform.rotation;
        if (wielderMovable != null) {
            if (getModSpot() == ModSpot.Legs && wielderMovable != null) {
                KickBack(Vector3.up * feetMultiplier);
            }
            else {
                KickBack(-wielderMovable.GetForward());
            }            
        }
        
        blasterEffect.Emit();
    }

    private void KickBack(Vector3 direction)
    {
        wielderMovable.AddDecayingForce(direction * kickbackMagnitude);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        this.wielderMovable = wielderMovable;        
        this.wielderStats = wielderStats;
        pickupCollider.enabled = false;
    }

    void BoostRange()
    {
        //playerStats.Modify(StatType.MiniMapRange, rangeBoostValue);
    }

    public override void DeActivate()
    {
        //Nothing to do
    }

    public override void DetachAffect()
    {
        //playerStats.Modify(StatType.MiniMapRange, 1 / rangeBoostValue);
        pickupCollider.enabled = true;
        wielderMovable = null;
    }

    // Use this for initialization
    void Start () {
        readyToShoot = true;
        type = ModType.ArmBlaster;
        category = ModCategory.Ranged;
    }
	
	// Update is called once per frame
	void Update () {
        if (wielderMovable != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                transform.forward = wielderMovable.GetForward();
            }
        }
    }
}
