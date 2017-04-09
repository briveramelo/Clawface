using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserMod : Mod {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float shortRangeDistance;
    [SerializeField]
    private float longRangeDistance;
    [SerializeField]
    private float maxScaleMultiplier;
    [SerializeField]
    private float distanceIncreaseSpeed;
    [SerializeField]
    private float scaleIncreaseSpeed;
    [SerializeField]
    private float standarDamageMultiplier;
    [SerializeField]
    private float chargeDamageMultiplier;
    [SerializeField]
    private GameObject targetCanvas;
    [SerializeField]
    private GameObject targetImage;
    #endregion

    #region Private Fields
    private Vector3 foot;
    private Vector3 chargePosition;
    private Vector3 chargeScale;
    private Transform originalParent;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        DeactivateModCanvas();
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        targetCanvas.SetActive(false);
        chargePosition = Vector3.zero;
        originalParent = null;
        chargeScale = Vector3.one;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    #region Public Methods
    public override void Activate()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserProjectile);
        Vector3 forwardVector = wielderMovable.GetForward();
        forwardVector.y = 0.0f;
        projectile.transform.position = foot + forwardVector.normalized * shortRangeDistance;
        projectile.GetComponent<GeyserProjectile>().damage = wielderStats.GetStat(StatType.Attack) * standarDamageMultiplier;        
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
        if (isHeld)
        {
            if (chargePosition == Vector3.zero)
            {
                chargePosition = foot;
            }
            if(originalParent == null)
            {
                originalParent = targetCanvas.transform.parent;
                targetCanvas.transform.parent = null;
            }
            Vector3 forwardVector = wielderMovable.GetForward();
            forwardVector.y = 0.0f;
            chargePosition = Vector3.Lerp(chargePosition, foot + forwardVector * longRangeDistance, distanceIncreaseSpeed);
            chargeScale = Vector3.Lerp(chargeScale, Vector3.one * maxScaleMultiplier, scaleIncreaseSpeed);
            Vector3 canvasPosition = chargePosition;
            canvasPosition.y += 0.2f;
            targetCanvas.transform.position = canvasPosition;
            targetCanvas.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            targetCanvas.transform.localScale = chargeScale;
            if (!targetCanvas.activeSelf)
            {
                targetCanvas.SetActive(true);
            }
        }
        else
        {
            GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserProjectile);
            projectile.transform.position = chargePosition;
            projectile.GetComponent<GeyserProjectile>().damage = wielderStats.GetStat(StatType.Attack) * chargeDamageMultiplier;
            projectile.transform.localScale = chargeScale;
            chargePosition = Vector3.zero;
            chargeScale = Vector3.one;
            targetCanvas.transform.parent = originalParent;
            targetCanvas.transform.position = chargePosition;
            targetCanvas.transform.localScale = chargeScale;
            targetCanvas.SetActive(false);
            originalParent = null;
        }
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        this.wielderStats = wielderStats;
        this.wielderMovable = wielderMovable;
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
        this.wielderStats = null;
        this.wielderMovable = null;
        pickupCollider.enabled = true;
    }

    public void SetFootPosition(Vector3 foot)
    {
        this.foot = foot;
    }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
