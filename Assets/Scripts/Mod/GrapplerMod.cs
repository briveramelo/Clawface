using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class GrapplerMod : Mod {

    #region Public fields
    public IMovable WielderMovable { get { return wielderMovable; } }
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Hook hook;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceMultiplier;
    #endregion

    #region Private Fields
    private bool hitTargetThisShot;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        type = ModType.Grappler;
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
    #endregion

    #region Public Methods
    public override void Activate(Action onComplete=null)
    {
        base.Activate(onComplete);
    }    

    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }
    protected override void ActivateStandardArms(){ hook.Throw(false); }
    protected override void ActivateChargedArms(){ hook.Throw(true); }

    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateStandardLegs(){ Jump(); }
    protected override void ActivateChargedLegs(){ Jump(); }

    private void Jump() {
        if (wielderMovable.IsGrounded()) {
            float force = energySettings.IsCharged ? jumpForce * jumpForceMultiplier : jumpForce;
            wielderMovable.AddDecayingForce(Vector3.up * force);
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect(){
        base.DetachAffect();
    }

    public bool GetHitTargetThisShot() { return hitTargetThisShot; }
    public void SetHitTargetThisShot(bool hitTarget) { hitTargetThisShot = hitTarget;}
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
