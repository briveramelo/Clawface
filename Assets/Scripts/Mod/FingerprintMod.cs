using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerprintMod : Mod {

    ITriggerable unlockableObject;

    [SerializeField]
    private Collider unlockColliderVolume;

    protected override void Awake(){
        //type = ModType.FingerPrint;
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        isAttached = false;
        unlockableObject = null;
        unlockColliderVolume.enabled = false;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {
        if (isAttached && unlockableObject != null){
            unlockableObject.Activate();
        }
    }

    protected override void ActivateChargedArms(){

    }

    protected override void ActivateStandardArms(){

    }
    protected override void ActivateStandardLegs(){
        ActivateStandardArms();
    }

    protected override void ActivateChargedLegs(){
        ActivateStandardArms();
    }
    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);        
        unlockColliderVolume.enabled = true;        
    }

    public override void DeActivate()
    {
        //Nothing to do here
    }

    public override void DetachAffect()
    {
        unlockColliderVolume.enabled = false;
        if (unlockableObject!=null)
        {
            unlockableObject.Wait();
            unlockableObject = null;
        }
        base.DetachAffect();
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
}
