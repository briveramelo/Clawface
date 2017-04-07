using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMod : Mod {


    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float maxDistance;
    #endregion

    #region Private Fields
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        type = ModType.Boomerang;
        category = ModCategory.Ranged;
        if (modCanvas)
        {
            modCanvas.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    #region Public Methods
    public override void Activate()
    {
        
    }

    public override void ActivateModCanvas()
    {
        if (modCanvas && !isAttached)
        {
            modCanvas.SetActive(true);
        }        
    }

    public override void AlternateActivate(bool isHeld, float holdTime)
    {
        
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        isAttached = true;
        this.wielderMovable = wielderMovable;        
        this.wielderStats = wielderStats;
        pickupCollider.enabled = false;
    }

    public override void DeActivate()
    {
        
    }

    public override void DeactivateModCanvas()
    {
        if (modCanvas)
        {
            modCanvas.SetActive(false);
        }
    }

    public override void DetachAffect()
    {
        isAttached = false;
        pickupCollider.enabled = true;
        wielderMovable = null;
        setModSpot(ModSpot.Default);
    }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
