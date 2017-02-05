using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterMod : Mod {

    [SerializeField]
    private float rangeBoostValue;
    IMovable playerMoveable;

    [SerializeField]
    private float kickbackMagnitude;

    public override void Activate()
    {
        Shoot();
    }

    void Shoot()
    {
        GameObject blasterBullet = BulletPool.instance.getBlasterBullet();
        blasterBullet.transform.position = transform.position;
        if (getModSpot() == ModSpot.Legs)
        {
            blasterBullet.GetComponent<BlasterBullet>().setDirection(-transform.up);
            KickBack(transform.up);
        }
        else
        {
            blasterBullet.GetComponent<BlasterBullet>().setDirection(transform.forward);
            KickBack(-transform.forward);
        }
        blasterBullet.SetActive(true);
    }

    public void setPlayerMoveable(IMovable moveable)
    {
        playerMoveable = moveable;
    }

    void KickBack(Vector3 direction)
    {
        playerMoveable.AddExternalForce(direction * kickbackMagnitude);
    }

    public override void AttachAffect(ref Stats i_playerStats, ref IMovable moveable)
    {
        playerMoveable = moveable;
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
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetButton(Strings.PREPARETOPICKUPORDROP) && !Input.GetButton(Strings.PREPARETOSWAP))
        {
            switch (getModSpot())
            {
                case ModSpot.ArmL:
                    if (Input.GetButton(Strings.LEFT))
                    {
                        Activate();
                    }
                    break;
                case ModSpot.ArmR:
                    if (Input.GetButton(Strings.RIGHT))
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
