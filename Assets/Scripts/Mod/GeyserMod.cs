using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;

public class GeyserMod : Mod {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private float shortRangeDistance;
    [SerializeField] private float longRangeDistance;
    [SerializeField] private float maxScaleMultiplier;
    [SerializeField] private float minScaleMultiplier;
    [SerializeField]
    private GameObject geyserBase;
    [SerializeField]
    private GameObject geyserTarget;
    [SerializeField]
    private int numberOfSquirts;
    [SerializeField]
    private float squirtDistanceOffset;
    [SerializeField]
    private float timeBetweenSquirts;
    [SerializeField]
    private float timeForEachSquirt;
    [SerializeField]
    private float megaSquirtWaitTime;
    #endregion

    #region Private Fields
    private Transform foot;
    private Transform originalGeyserBaseParent;
    private Transform originalGeyserTargetParent;
    private ProjectileProperties projectileProperties = new ProjectileProperties();
    private GameObject geyserShield;
    private GameObject geyser;
    private Vector3 finalForwardVector;
    private Vector3 finalFootPosition;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        DeactivateModCanvas();
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        originalGeyserBaseParent = null;
        originalGeyserTargetParent = null;
        geyserBase.SetActive(false);
        geyserTarget.SetActive(false);
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
        if (getModSpot() == ModSpot.Legs)
        {
            hasState = false;
        }
        else
        {
            hasState = true;
        }
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
    protected override void ActivateChargedArms() {
        StartSquirtTrail();
    }

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

    private void StartSquirtTrail()
    {
        finalForwardVector = wielderMovable.GetForward().NormalizedNoY();
        finalFootPosition = foot.position;
        Timing.RunCoroutine(SquirtSquirt());
    }

    private IEnumerator<float> SquirtSquirt()
    {
        float distance = Vector3.Distance(finalFootPosition, targetPosition) - squirtDistanceOffset;
        distance /= numberOfSquirts;
        for(int i = 1; i <= numberOfSquirts; i++)
        {
            GameObject squirt = ObjectPool.Instance.GetObject(PoolObjectType.GeyserGushLine);
            squirt.transform.position = finalFootPosition + finalForwardVector * distance * i;
            squirt.GetComponent<GeyserLine>().Fire(i / (float)numberOfSquirts, timeForEachSquirt, projectileProperties);
            yield return Timing.WaitForSeconds(timeBetweenSquirts);
        }
        yield return Timing.WaitForSeconds(megaSquirtWaitTime);
        MegaErupt();
        yield return 0;
    }

    private void Erupt()
    {
        geyser = GetGeyser();
        if (geyser)
        {
            Vector3 forwardVector = wielderMovable.GetForward().NormalizedNoY();
            geyser.transform.position = targetPosition;
            InitializeGeyserBase();
            FinishFiring();
        }
    }

    private void InitializeGeyserBase()
    {
        originalGeyserBaseParent = geyserBase.transform.parent;
        geyserBase.transform.parent = null;
        geyserBase.SetActive(true);
        geyserBase.transform.position = targetPosition;
        geyserBase.transform.rotation = Quaternion.identity;
        geyserBase.transform.localScale = chargeScale;
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
        if (originalGeyserTargetParent == null)
        {
            originalGeyserTargetParent = geyserTarget.transform.parent;
            geyserTarget.transform.parent = null;
        }
        if (!geyserTarget.activeSelf)
        {
            geyserTarget.SetActive(true);
        }
        Vector3 canvasPosition = targetPosition + Vector3.up * 0.2f;
        geyserTarget.transform.position = canvasPosition;
        geyserTarget.transform.rotation = Quaternion.identity;
        geyserTarget.transform.localScale = chargeScale;
    }

    private void MegaErupt()
    {
        geyser = GetGeyser();
        if (geyser)
        {
            geyser.transform.position = finalFootPosition + finalForwardVector * longRangeDistance;
            geyser.transform.localScale = Vector3.one * maxScaleMultiplier;
            originalGeyserBaseParent = geyserBase.transform.parent;
            geyserBase.transform.parent = null;
            geyserBase.SetActive(true);
            geyserBase.transform.position = geyser.transform.position;
            geyserBase.transform.rotation = Quaternion.identity;
            geyserBase.transform.localScale = geyser.transform.localScale;
        }
        FinishFiring();
    }
    private void FinishFiring()
    {        
        //geyserTarget.transform.position = targetPosition;
        geyserTarget.transform.localScale = chargeScale;
        geyserTarget.transform.SetParent(originalGeyserTargetParent);
        geyserTarget.SetActive(false);
        Timing.RunCoroutine(WaitForGeyserToDie());
        originalGeyserBaseParent = null;
        originalGeyserTargetParent = null;
    }

    private IEnumerator<float> WaitForGeyserToDie()
    {
        if (geyser) {
            while (geyser.activeSelf)
            {
                yield return 0;
            }
            ResetGeyserBase();
        }
        yield return 0;
    }

    private void ResetGeyserBase()
    {        
        geyserBase.transform.localScale = chargeScale;
        geyserBase.transform.parent = originalGeyserBaseParent;
        geyserBase.SetActive(false);
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
