using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterMod : Mod {

    [SerializeField]
    private float rangeBoostValue;
    MoveState playerMovement;

    [SerializeField]
    private float kickbackMagnitude;

    private bool readyToShoot;

    [SerializeField]
    private float coolDownTime;

    [SerializeField]
    private float feetMultiplier;

    [SerializeField]
    VFXBlasterShoot blasterEffect; 

    public override void Activate()
    {
        if (readyToShoot && getModSpot() != ModSpot.Head)
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
        blasterBullet.transform.position = transform.position;
        blasterBullet.transform.rotation = transform.rotation;
        if (getModSpot() == ModSpot.Legs && playerMovement != null)
        {            
            KickBack(playerMovement.gameObject.transform.up * feetMultiplier);
        }
        else if (playerMovement != null)
        {            
            KickBack(-playerMovement.gameObject.transform.forward);
        }
        blasterBullet.SetActive(true);
        blasterEffect.Emit();
    }

    void KickBack(Vector3 direction)
    {
        playerMovement.AddExternalForce(direction * kickbackMagnitude);
    }

    public override void AttachAffect(ref Stats i_playerStats, ref MoveState movement)
    {
        playerMovement = movement;        
        playerStats = i_playerStats;
        pickupCollider.enabled = false;
        if (getModSpot() == ModSpot.Head)
        {
            BoostRange();
        }
    }

    void BoostRange()
    {
        playerStats.Modify(StatType.MiniMapRange, rangeBoostValue);
    }

    public override void DeActivate()
    {
        //Nothing to do
    }

    public override void DetachAffect()
    {
        playerStats.Modify(StatType.MiniMapRange, 1 / rangeBoostValue);
        pickupCollider.enabled = true;
        playerMovement = null;
    }

    // Use this for initialization
    void Start () {
        readyToShoot = true;
        type = ModType.ArmBlaster;
        category = ModCategory.Ranged;
    }
	
	// Update is called once per frame
	void Update () {
        if (playerMovement != null)
        {
            if (getModSpot() != ModSpot.Legs)
            {
                transform.forward = playerMovement.transform.forward;
            }
        }
    }
}
