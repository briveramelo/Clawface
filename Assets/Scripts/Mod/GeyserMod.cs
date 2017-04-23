using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class GeyserMod : Mod {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private float shortRangeDistance;
    [SerializeField] private float longRangeDistance;
    [SerializeField] private float maxScaleMultiplier;
    [SerializeField] private float minScaleMultiplier;
    [SerializeField] private GameObject targetCanvas;
    #endregion

    #region Private Fields
    private Transform foot;
    private Transform originalParent;
    private ProjectileProperties projectileProperties = new ProjectileProperties();
    private GameObject geyserShield;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        DeactivateModCanvas();
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        targetCanvas.SetActive(false);
        originalParent = null;
    }
    #endregion

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null){
        base.Activate(onCompleteCoolDown, onActivate);
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
        foot = ((VelocityBody)wielderMovable).foot;
    }

    public override void DeActivate() { }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }

    public void SetFoot(Transform foot)
    {
        this.foot = foot;
    }
    #endregion


    #region Private Methods
    protected override void BeginChargingArms() { }
    protected override void RunChargingArms() { Charging(); }
    protected override void ActivateStandardArms() { Erupt(); }
    protected override void ActivateChargedArms() { MegaErupt(); }


    protected override void BeginChargingLegs() { }
    protected override void RunChargingLegs() { }
    protected override void ActivateStandardLegs()
    {
        if (geyserShield == null || !geyserShield.activeSelf)
        {
            SetGeyserShield();
            geyserShield.GetComponent<GeyserShield>().Fire();
        }
    }
    protected override void ActivateChargedLegs()
    {
        if (geyserShield == null || !geyserShield.activeSelf)
        {
            SetGeyserShield();
            geyserShield.GetComponent<GeyserShield>().FireCharged();
        }
    }

    private void Erupt()
    {
        GameObject geyser = GetGeyser();
        Vector3 forwardVector = wielderMovable.GetForward().NormalizedNoY();
        geyser.transform.position = targetPosition;
        FinishFiring();
    }

    private GameObject GetGeyser()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserProjectile);
        if (projectile)
        {
            projectileProperties.Initialize(GetWielderInstanceID(), Attack);
            projectile.GetComponent<GeyserProjectile>().SetProjectileProperties(projectileProperties);
        }
        return projectile;
    }

    private void Charging()
    {
        if (originalParent == null)
        {
            originalParent = targetCanvas.transform.parent;
            targetCanvas.transform.SetParent(null);
        }
        if (!targetCanvas.activeSelf)
        {
            targetCanvas.SetActive(true);
        }
        Vector3 canvasPosition = targetPosition + Vector3.up * 0.2f;
        targetCanvas.transform.position = canvasPosition;
        targetCanvas.transform.rotation = Quaternion.identity;
        targetCanvas.transform.localScale = chargeScale;
    }

    private void MegaErupt()
    {
        GameObject geyser = GetGeyser();
        if (geyser)
        {
            geyser.transform.position = targetPosition;
            geyser.transform.localScale = chargeScale;
        }
        FinishFiring();
    }
    private void FinishFiring()
    {
        targetCanvas.transform.SetParent(originalParent);
        targetCanvas.transform.position = targetPosition;
        targetCanvas.transform.localScale = chargeScale;
        targetCanvas.SetActive(false);
        originalParent = null;
    }

    private Vector3 targetPosition
    {
        get
        {
            return foot.position + wielderMovable.GetForward().NormalizedNoY() * (shortRangeDistance + energySettings.chargeFraction * (longRangeDistance - shortRangeDistance));
        }
    }

    private Vector3 chargeScale
    {
        get
        {
            return Vector3.one * (minScaleMultiplier + (maxScaleMultiplier - minScaleMultiplier) * energySettings.chargeFraction);
        }
    }
    private void SetGeyserShield()
    {
        geyserShield = ObjectPool.Instance.GetObject(PoolObjectType.GeyserShield);
        if (geyserShield != null)
        {
            geyserShield.transform.position = foot.position;
            geyserShield.GetComponent<GeyserShield>().SetProjectileProperties(projectileProperties);
        }
    }
    #endregion

    #region Private Structures
    #endregion

}
