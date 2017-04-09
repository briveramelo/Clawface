using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mod : MonoBehaviour {

    #region Public fields
    public int GetWielderInstanceID() {
        if (wielderStats != null) {
            return wielderStats.gameObject.GetInstanceID();
        }
        return 0;
    }
    public ChargeSettings modChargeSettings {
        get { return chargeSettings; }
    }
    #endregion

    #region Protected Fields
    protected ModType type;
    protected ModCategory category;
    protected Stats wielderStats;
    protected IMovable wielderMovable;
    protected List<IDamageable> recentlyHitEnemies = new List<IDamageable>();
    protected bool isAttached;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] protected Collider pickupCollider;
    [SerializeField] protected GameObject modCanvas;
    [SerializeField] protected ChargeSettings chargeSettings;
    #endregion

    #region Private Fields
    ModSpot spot;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake(){
        if (modCanvas) {
            modCanvas.SetActive(false);
        }
    }
    protected virtual void Update()
    {
        HandleChargeEffects();
    }
    #endregion

    #region Public Methods
    public bool IsCharged() {
        return chargeSettings.isCharged;
    }

    public void PlayCharged() {
        //vfxModCharge.Enable();
    }
    public void StopCharged() {
        //vfxModCharge.Disable();
    }

    public void UpdateChargeTime(float deltaTime) {        
        chargeSettings.timeCharged += deltaTime;
    }
    public void ResetChargeTime() {
        chargeSettings.Reset();
    }   

    public abstract void Activate();

    protected abstract void ActivateStandard();
    protected abstract void ActivateCharged();

    public abstract void DeActivate();

    public abstract void AttachAffect(ref Stats wielderStats, IMovable wielderMovable);

    public virtual void DetachAffect() {
        isAttached = false;
        wielderMovable = null;
        wielderStats = null;
        recentlyHitEnemies.Clear();
        chargeSettings.Reset();
        pickupCollider.enabled = true;
        setModSpot(ModSpot.Default);
    }

        
    public virtual void ActivateModCanvas()
    {
        if (modCanvas && !isAttached)
        {
            modCanvas.SetActive(true);
        }
    }

    public virtual void DeactivateModCanvas()
    {
        if (modCanvas)
        {
            modCanvas.SetActive(false);
        }
    }

    public void setModType(ModType modType)
    {
        type = modType;
    }

    public ModType getModType()
    {
        return type;
    }

    public ModCategory getModCategory()
    {
        return category;
    }

    public void setModSpot(ModSpot modSpot)
    {
        spot = modSpot;
    }

    public ModSpot getModSpot()
    {
        return spot;
    }
    #endregion

    #region Private Methods
    private void HandleChargeEffects() {
        if (chargeSettings.isStarting) {
            //vfxModCharge.Enable();
        }
        if (chargeSettings.isCharged) {

        }
    }
    #endregion

    #region Private Structures
    [System.Serializable]
    public class ChargeSettings {

        public float timeToCharge;
        [HideInInspector] public float timeCharged;

        public ChargeSettings(float timeToCharge) {
            this.timeToCharge = timeToCharge;
        }

        public bool isCharged { get { return timeCharged > timeToCharge; } }
        public bool isCharging { get { return timeCharged > 0f; } }
        public bool isStarting { get { return timeCharged ==0f; } }

        public void Reset() {
            timeCharged = 0f;
        }

    }
    #endregion

}
