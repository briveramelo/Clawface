using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

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
    #endregion

    #region Public Methods
    public override void Activate(Action onComplete=null){
        base.Activate();
    }

    protected override void ActivateStandardArms(){ Erupt();}
    protected override void ActivateChargedArms(){ MegaErupt();}
    protected override void ActivateStandardLegs(){ }
    protected override void ActivateChargedLegs(){ }
    protected override void BeginChargingLegs(){ }
    protected override void RunChargingLegs(){ }
    protected override void BeginChargingArms(){ }
    protected override void RunChargingArms(){ }

    private void Erupt(){ 
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserProjectile);
        Vector3 forwardVector = wielderMovable.GetForward().NormalizedNoY();
        projectile.transform.position = foot + forwardVector * shortRangeDistance;
        projectile.GetComponent<GeyserProjectile>().damage = attack;
    }

    private void Charging(){ 
        if (chargePosition == Vector3.zero){
            chargePosition = foot;
        }
        if(originalParent == null){
            originalParent = targetCanvas.transform.parent;
            targetCanvas.transform.parent = null;
        }
        Vector3 forwardVector = wielderMovable.GetForward().NormalizedNoY();            
        chargePosition = Vector3.Lerp(chargePosition, foot + forwardVector * longRangeDistance, distanceIncreaseSpeed);
        chargeScale = Vector3.Lerp(chargeScale, Vector3.one * maxScaleMultiplier, scaleIncreaseSpeed);
        Vector3 canvasPosition = chargePosition;
        canvasPosition.y += 0.2f;
        targetCanvas.transform.position = canvasPosition;
        targetCanvas.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        targetCanvas.transform.localScale = chargeScale;
        if (!targetCanvas.activeSelf){
            targetCanvas.SetActive(true);
        }
    }

    private void MegaErupt(){
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

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);        
    }

    public override void DeActivate(){}

    public override void DetachAffect(){
        base.DetachAffect();
    }

    public void SetFootPosition(Vector3 foot){
        this.foot = foot;
    }
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
