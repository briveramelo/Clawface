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
    [SerializeField] private float minScaleMultiplier;
    [SerializeField]
    private GameObject geyserTarget;    
    #endregion

    #region Private Fields
    private Transform foot;
    private Transform originalGeyserBaseParent;
    private Transform originalGeyserTargetParent;
    private ProjectileProperties projectileProperties = new ProjectileProperties();    
    private GameObject geyser;
    private Vector3 finalForwardVector;
    private Vector3 finalFootPosition;
    private GameObject geyserBase;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.Geyser;
        category = ModCategory.Ranged;
        base.Awake();
    }

    void Start () {
        DeactivateModCanvas();
        
        originalGeyserBaseParent = null;
        originalGeyserTargetParent = null;
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
        hasState = false;        
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
    protected override void ActivateStandardArms() { Erupt(); }

    private void Erupt()
    {
        geyser = GetGeyser();
        if (geyser)
        {
            SFXManager.Instance.Play(SFXType.GeyserMod_MiniSplash, transform.position);
            Vector3 forwardVector = wielderMovable.GetForward().NormalizedNoY();
            geyser.transform.position = targetPosition;
            GetGeyserBase();
            FinishFiring();
        }
    }

    private void GetGeyserBase()
    {

        geyserBase = ObjectPool.Instance.GetObject(PoolObjectType.GeyserBase);
        if (geyserBase)
        {
            geyserBase.SetActive(false);
            geyserBase.transform.position = targetPosition;
            geyserBase.transform.rotation = Quaternion.identity;
            geyserBase.transform.localScale = scale;
            geyserBase.SetActive(true);
        }
    }

    private GameObject GetGeyser()
    {
        GameObject projectile = ObjectPool.Instance.GetObject(PoolObjectType.GeyserProjectile);
        if (projectile)
        {            
            projectileProperties.Initialize(GetWielderInstanceID(), Attack);
            projectile.GetComponent<GeyserProjectile>().SetProjectileProperties(projectileProperties);

            if (wielderStats.gameObject.CompareTag(Strings.Tags.PLAYER))
            {
                projectile.GetComponent<GeyserProjectile>().SetShooterType(true);
            }
            else
            {
                projectile.GetComponent<GeyserProjectile>().SetShooterType(false);
            }
        }
        return projectile;
    }

    private void FinishFiring()
    {
        geyserTarget.transform.localScale = scale;
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
            //ResetGeyserBase();
        }
        yield return 0;
    }

    private Vector3 targetPosition
    {
        get
        {
            return foot.position + wielderMovable.GetForward().NormalizedNoY() * (shortRangeDistance);
        }
    }

    private Vector3 scale
    {
        get
        {
            return Vector3.one * (minScaleMultiplier);
        }
    }
    #endregion

    #region Private Structures
    #endregion

}
