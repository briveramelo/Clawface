using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerMod : Mod {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Hook hook;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceMultiplier;
    #endregion

    #region Private Fields
    private SharedVariables sharedVariables;
    private bool rotateHook;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        sharedVariables = new SharedVariables();
        sharedVariables.throwHook = false;
        sharedVariables.retractHook = false;
        rotateHook = false;
        type = ModType.Grappler;
        category = ModCategory.Ranged;
        hook.Init(ref sharedVariables);        
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
        if (sharedVariables.throwHook)
        {
            hook.Throw();
        }
        else if (sharedVariables.retractHook)
        {
            hook.Retract();
        }
        if (rotateHook)
        {
            hook.Rotate();
        }
	}
    #endregion

    #region Public Methods
    public override void Activate()
    {
        Action hookAction = chargeSettings.isCharged ? (Action)ActivateStandard : ActivateCharged;
        if (getModSpot() == ModSpot.ArmL || getModSpot() == ModSpot.ArmR) {
            hookAction();
        }
        else if (getModSpot() == ModSpot.Legs && wielderMovable.IsGrounded()) {
            Jump();
        }
    }

    protected override void ActivateStandard()
    {
        if (!sharedVariables.throwHook && !sharedVariables.retractHook)
        {
            sharedVariables.throwHook = true;
        }
    }

    protected override void ActivateCharged()
    {
        sharedVariables.isCharged = true;
        sharedVariables.throwHook = true;
    }

    private void Jump() {
        float force = chargeSettings.isCharged ? jumpForce * jumpForceMultiplier : jumpForce;
        sharedVariables.wielderMovable.AddDecayingForce(Vector3.up * force);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        isAttached = true;
        this.wielderMovable = wielderMovable;
        sharedVariables.wielderMovable = wielderMovable;
        this.wielderStats = wielderStats;
        pickupCollider.enabled = false;
        sharedVariables.modSpot = getModSpot();
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect()
    {
        sharedVariables.wielderMovable = null;
        sharedVariables.modSpot = ModSpot.Default;
        base.DetachAffect();
    }

    public bool HitTargetThisShot() { return sharedVariables.hitTargetThisShot; }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    public class SharedVariables
    {
        public bool throwHook;
        public bool retractHook;
        public bool isCharged;
        public bool hitTargetThisShot;
        public IMovable wielderMovable;
        public ModSpot modSpot;
    }
    #endregion

}
