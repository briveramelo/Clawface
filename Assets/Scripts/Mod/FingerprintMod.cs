﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerprintMod : Mod {

    IUnlockable unlockableObject;
    bool attached;

    [SerializeField]
    private Collider unlockColliderVolume;

    public override void Activate()
    {
        if (attached && unlockableObject != null)
        {
            unlockableObject.Unlock();
        }
    }

    public override void AttachAffect(ref Stats playerStats, ref PlayerMovement playerMovement)
    {
        attached = true;
        unlockColliderVolume.enabled = true;
        pickupCollider.enabled = false;
    }

    public override void DeActivate()
    {
        //Nothing to do here
    }

    public override void DetachAffect()
    {
        attached = false;
        pickupCollider.enabled = true;
        unlockColliderVolume.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Strings.UNLOCKABLE)
        {
            unlockableObject = other.gameObject.GetComponent<IUnlockable>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Strings.UNLOCKABLE)
        {
            if(unlockableObject == other.gameObject.GetComponent<IUnlockable>())
            {
                unlockableObject = null;
            }
        }
    }

    // Use this for initialization
    void Start () {
        attached = false;
        unlockableObject = null;
        unlockColliderVolume.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown(Strings.UP) || Input.GetButtonDown(Strings.DOWN)|| Input.GetButtonDown(Strings.LEFT)|| Input.GetButtonDown(Strings.RIGHT) || Input.GetAxis(Strings.RIGHTTRIGGER) != 0 || Input.GetAxis(Strings.LEFTTRIGGER) != 0)
        {
            Activate();
        }
	}
}
