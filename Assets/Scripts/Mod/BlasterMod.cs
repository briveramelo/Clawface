﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterMod : Mod {

    [SerializeField]
    private float rangeBoostValue;
    PlayerMovement playerMovement;

    [SerializeField]
    private float kickbackMagnitude;

    private bool readyToShoot;

    [SerializeField]
    private float coolDownTime;

    [SerializeField]
    private float feetMultiplier;

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
        GameObject blasterBullet = BulletPool.instance.getBlasterBullet();
        blasterBullet.transform.position = transform.position;
        blasterBullet.transform.rotation = transform.rotation;
        if (getModSpot() == ModSpot.Legs)
        {            
            KickBack(playerMovement.gameObject.transform.up * feetMultiplier);
        }
        else
        {            
            KickBack(-playerMovement.gameObject.transform.forward);
        }
        blasterBullet.SetActive(true);
    }

    void KickBack(Vector3 direction)
    {
        playerMovement.AddExternalForce(direction * kickbackMagnitude);
    }

    public override void AttachAffect(ref Stats i_playerStats, ref PlayerMovement movement)
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
    }

    // Use this for initialization
    void Start () {
        readyToShoot = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetButton(Strings.PREPARETOPICKUPORDROP) && !Input.GetButton(Strings.PREPARETOSWAP))
        {
            switch (getModSpot())
            {
                case ModSpot.ArmL:
                    if (Input.GetButton(Strings.LEFT) || Input.GetAxis(Strings.LEFTTRIGGER) != 0)
                    {
                        Activate();
                    }
                    break;
                case ModSpot.ArmR:
                    if (Input.GetButton(Strings.RIGHT) || Input.GetAxis(Strings.RIGHTTRIGGER) != 0)
                    {
                        Activate();
                    }
                    break;               
                case ModSpot.Legs:
                    if (Input.GetButtonDown(Strings.DOWN))
                    {
                        Activate();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
