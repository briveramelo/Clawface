using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerprintMod : Mod {

    ITriggerable unlockableObject;
    bool attached;

    [SerializeField]
    private Collider unlockColliderVolume;

    private void Awake()
    {
        type = ModType.FingerPrint;
    }

    public override void Activate()
    {
        if (attached && unlockableObject != null)
        {
            unlockableObject.Activate();
        }
    }

    public override void AttachAffect(ref Stats playerStats, IMovable wielderMovable)
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
        if (unlockableObject!=null)
        {
            unlockableObject.Wait();
            unlockableObject = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Strings.Tags.UNLOCKABLE)
        {
            unlockableObject = other.gameObject.GetComponent<ITriggerable>();
            if (unlockableObject != null)
                unlockableObject.Notify();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Strings.Tags.UNLOCKABLE)
        {
            if(unlockableObject == other.gameObject.GetComponent<ITriggerable>())
            {
                unlockableObject.Wait();
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
	}

    public override void AlternateActivate(bool isHeld)
    {
        
    }
}
