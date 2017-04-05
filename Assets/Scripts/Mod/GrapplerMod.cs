using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerMod : Mod {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Hook hook;
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
        if (!sharedVariables.throwHook && !sharedVariables.retractHook)
        {
            sharedVariables.throwHook = true;
        }
    }

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        if (isHeld && !sharedVariables.specialAttack)
        {
            rotateHook = true;
            sharedVariables.specialAttack = true;
        }
        else if(!isHeld)
        {
            rotateHook = false;
            sharedVariables.throwHook = true;            
        }
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
        isAttached = false;
        pickupCollider.enabled = true;
        sharedVariables.wielderMovable = null;
        wielderMovable = null;
        sharedVariables.modSpot = ModSpot.Default;
    }

    public bool HitTargetThisShot() { return sharedVariables.hitTargetThisShot; }
    #endregion

    #region Private Methods
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
    #endregion

    #region Private Structures
    public class SharedVariables
    {
        public bool throwHook;
        public bool retractHook;
        public bool specialAttack;
        public bool hitTargetThisShot;
        public IMovable wielderMovable;
        public ModSpot modSpot;
    }
    #endregion

}
