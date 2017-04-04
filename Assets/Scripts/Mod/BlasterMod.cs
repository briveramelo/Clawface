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
    private VFXBlasterShoot blasterEffect;

    [SerializeField]
    private float kickBackChargedMultiplier;

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
        AudioManager.Instance.PlaySFX(SFXType.BlasterShoot);
        SpawnBullet();
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


    BlasterBullet SpawnBullet()
    {
        BlasterBullet blasterBullet = ObjectPool.Instance.GetObject(PoolObjectType.BlasterBullet).GetComponent<BlasterBullet>();
        blasterBullet.transform.position = bulletSpawnPoint.position;
        blasterBullet.transform.rotation = transform.rotation;
        blasterBullet.SetShooterInstanceID(GetWielderInstanceID());
        return blasterBullet;        
    }

    private void KickBack(Vector3 direction)
    {
        wielderMovable.AddDecayingForce(direction * kickbackMagnitude);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        isAttached = true;
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
        isAttached = false;
        //playerStats.Modify(StatType.MiniMapRange, 1 / rangeBoostValue);
        pickupCollider.enabled = true;
        wielderMovable = null;
        
    }

    // Use this for initialization
    void Start () {
        readyToShoot = true;
        type = ModType.ArmBlaster;
        category = ModCategory.Ranged;
        if (modCanvas) { modCanvas.SetActive(false); }
       
    }

    public override void ActivateModCanvas()
    {
        if (modCanvas && !isAttached)
        {
            modCanvas.SetActive(true);
        }
    }

    public override void DeactivateModCanvas()
    {
        if (modCanvas)
        {
            modCanvas.SetActive(false);
        }
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

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        if (isHeld)
        {
            blasterEffect.Emit();
        }
        else
        {
            BlasterBullet blasterBullet = SpawnBullet();
            blasterBullet.isCharged = true;
            if (getModSpot() == ModSpot.Legs && wielderMovable != null)
            {
                KickBack(Vector3.up * feetMultiplier * kickBackChargedMultiplier);
            }
            else if (wielderMovable != null)
            {
                KickBack(-wielderMovable.GetForward() * kickBackChargedMultiplier);
            }
        }
    }
}
