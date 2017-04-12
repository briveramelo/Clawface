using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;

public abstract class Mod : MonoBehaviour {

    #region Public fields
    public int GetWielderInstanceID() {
        if (wielderStats != null) {
            return wielderStats.gameObject.GetInstanceID();
        }
        return 0;
    }
    public EnergySettings modEnergySettings {
        get { return energySettings; }
    }
    #endregion

    #region Protected Fields
    protected ModType type;
    protected ModCategory category;
    protected DamagerType damageType;
    public bool hasState;
    protected Stats wielderStats;
    protected IMovable wielderMovable;
    protected List<IDamageable> recentlyHitEnemies = new List<IDamageable>();
    protected VFXModCharge vfxModCharge;
    protected bool isAttached;
    protected Damager damager=new Damager();
    public float Attack {
        get {
            if (wielderStats != null) {
                return wielderStats.attack + energySettings.attack;
            }
            return energySettings.attack;
        }
    }
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] protected GameObject vfxModChargePrefab;
    [SerializeField] protected Collider pickupCollider;
    [SerializeField] protected GameObject modCanvas;
    [SerializeField] protected EnergySettings energySettings;
    #endregion

    #region Private Fields
    ModSpot spot;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake(){
        DeactivateModCanvas();
        Mod mod = this;
        energySettings.Initialize(ref mod);
        vfxModCharge=Instantiate(vfxModChargePrefab, transform).GetComponent<VFXModCharge>();
    }
    protected virtual void Update()
    {        
    }
    #endregion

    #region Public Methods
    public bool IsCharged() {
        return energySettings.IsCharged;
    }

    public void PlayCharged() {
        //vfxModCharge.Enable();
    }
    public void StopCharged() {
        //vfxModCharge.Disable();
    }

    public virtual void Activate(Action onComplete=null) {
        if (!energySettings.isInUse){
            Timing.RunCoroutine(BeginCoolDown(onComplete));            
            useAction();
        }
    }


    public virtual void BeginCharging() {
        if (!energySettings.isInUse) {
            vfxModCharge.StartCharging(energySettings.timeToCharge);
            energySettings.hasStartedCharging = true;
            if (getModSpot()==ModSpot.Legs) {
                BeginChargingLegs();
            }
            else {
                BeginChargingArms();
            }
        }
    }
    int i = 0;
    public virtual void RunCharging() {
        if (!energySettings.isInUse && energySettings.hasStartedCharging) {
            energySettings.timeCharged += Time.deltaTime;
            if (getModSpot()==ModSpot.Legs) {
                RunChargingLegs();
            }
            else {
                RunChargingArms();
            }
            i++;
        }
    }
    public void EndCharging() {
        vfxModCharge.StopCharging();
        energySettings.Reset();
    }

    protected abstract void BeginChargingArms();
    protected abstract void RunChargingArms();
    protected abstract void ActivateStandardArms();
    protected abstract void ActivateChargedArms();

    protected abstract void BeginChargingLegs();
    protected abstract void RunChargingLegs();
    protected abstract void ActivateStandardLegs();
    protected abstract void ActivateChargedLegs();

    public abstract void DeActivate();

    public virtual void AttachAffect(ref Stats wielderStats, IMovable wielderMovable) {
        isAttached = true;
        this.wielderStats = wielderStats;
        this.wielderMovable = wielderMovable;
        pickupCollider.enabled = false;
    }

    public virtual void DetachAffect() {
        isAttached = false;
        wielderMovable = null;
        wielderStats = null;
        recentlyHitEnemies.Clear();
        energySettings.Reset();
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
    public DamagerType getDamageType() {return damageType; }        
    #endregion

    #region Private Methods
    protected IEnumerator<float> BeginCoolDown(Action onComplete){
        energySettings.CheckToSetAsCharged();
        recentlyHitEnemies.Clear();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(energySettings.BeginCoolDown()));
        if (onComplete!=null) {
            onComplete();
        }
    }

    private Action useAction {
        get {
            if (getModSpot()==ModSpot.ArmL || getModSpot()==ModSpot.ArmR) {
                return IsCharged() ? (Action)ActivateChargedArms : ActivateStandardArms;
            }
            return IsCharged() ? (Action)ActivateChargedLegs : ActivateStandardLegs;
        }
    }

    #endregion

    #region Private Structures
    [System.Serializable]
    public class EnergySettings {

        public float timeToChargeArm;
        public float timeToChargeLeg;
        public AttackSettings standardArmAttackSettings;
        public AttackSettings chargedArmAttackSettings;
        public AttackSettings standardLegAttackSettings;
        public AttackSettings chargedLegAttackSettings;

        [HideInInspector] public float timeCharged;
        [HideInInspector] public bool isCoolingDown;

        private Mod mod;
        private bool isCharged;
        public bool isActive;
        public bool hasStartedCharging;

        public bool IsCharged { get { return isCharged; } }
        public float timeToCharge { get { return mod.getModSpot() == ModSpot.Legs ? timeToChargeLeg : timeToChargeArm; } }
        public bool isCharging { get { return timeCharged > 0f; } }
        public bool isStarting { get { return timeCharged == 0f; } }
        public bool isInUse { get { return isCoolingDown || isActive;} }

        public float coolDownTime { get { return attackSettings.timeToCoolDown; } }
        public float attack { get { return attackSettings.attack; } }
        public float hitStopTime { get { return attackSettings.timeToHitStop; } }
        public float timeToAttack { get { return attackSettings.timeToAttack; } }
        public float chargeFraction { get { return Mathf.Clamp01(timeCharged/timeToCharge);}}
        public AttackSettings attackSettings {
            get {
                if (mod.getModSpot()==ModSpot.Legs) {
                    return IsCharged ? chargedLegAttackSettings : standardLegAttackSettings;
                }
                return IsCharged ? chargedArmAttackSettings : standardArmAttackSettings;
            }
        }

        public IEnumerator<float> BeginCoolDown() {            
            isCoolingDown = true;
            yield return Timing.WaitForSeconds(coolDownTime);
            isCoolingDown = false;
            isCharged = false;
        }
        public void Initialize(ref Mod mod) {
            this.mod = mod;
        }
        public void Reset() {
            timeCharged = 0f;
            hasStartedCharging=false;
        }
        public void CheckToSetAsCharged() {
            if (timeCharged>timeToCharge) {
                isCharged = true;
            }
        }
    }

    [System.Serializable]
    public class AttackSettings {
        public float timeToCoolDown;
        public float timeToAttack;
        public float attack;
        public float timeToHitStop;
    }
    #endregion

}
