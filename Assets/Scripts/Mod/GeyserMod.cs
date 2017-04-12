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
    [SerializeField] private GameObject targetCanvas;
    [SerializeField] private GameObject targetImage;
    #endregion

    #region Private Fields
    private Transform foot;
    private Transform originalParent;
    private ProjectileProperties projectileProperties = new ProjectileProperties();
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
    public override void Activate(Action onComplete=null){
        base.Activate();
    }

    protected override void BeginChargingArms(){  }
    protected override void RunChargingArms(){ Charging(); }
    protected override void ActivateStandardArms(){ Erupt();}
    protected override void ActivateChargedArms(){ MegaErupt();}


    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void ActivateStandardLegs(){ }
    protected override void ActivateChargedLegs(){ }

    private void Erupt(){
        GameObject geyser = GetGeyser();
        Vector3 forwardVector = wielderMovable.GetForward().NormalizedNoY();
        geyser.transform.position = foot.position + forwardVector * shortRangeDistance;
        FinishFiring();
    }

    private GameObject GetGeyser() {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserProjectile);
        if (projectile) {            
            projectileProperties.Initialize(GetWielderInstanceID(), Attack);
            projectile.GetComponent<GeyserProjectile>().SetProjectileProperties(projectileProperties);
        }        
        return projectile;
    }

    private void Charging(){
        if (originalParent == null){
            originalParent = targetCanvas.transform.parent;
            targetCanvas.transform.SetParent(null);
        }
        if (!targetCanvas.activeSelf){
            targetCanvas.SetActive(true);
        }
        Vector3 canvasPosition = targetPosition + Vector3.up*0.2f;
        targetCanvas.transform.position = canvasPosition;
        targetCanvas.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        targetCanvas.transform.localScale = chargeScale;
    }

    private void MegaErupt(){
        GameObject geyser = GetGeyser();
        if (geyser) {
            geyser.transform.position = targetPosition;
            geyser.transform.localScale = chargeScale;
        }
        FinishFiring();                        
    }
    private void FinishFiring() {
        targetCanvas.transform.SetParent(originalParent);
        targetCanvas.transform.position = targetPosition;
        targetCanvas.transform.localScale = chargeScale;
        targetCanvas.SetActive(false);
        originalParent = null;
    }

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);
        this.foot = ((VelocityBody)wielderMovable).foot;
    }

    public override void DeActivate(){}

    public override void DetachAffect(){
        base.DetachAffect();
    }

    public void SetFoot(Transform foot){
        this.foot = foot;
    }
    #endregion

    private Vector3 targetPosition {
        get {
            return foot.position + wielderMovable.GetForward() * (energySettings.chargeFraction*longRangeDistance);
        }
    }

    private Vector3 chargeScale {
        get {
            return Vector3.one * maxScaleMultiplier * energySettings.chargeFraction;
        }
    }

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
