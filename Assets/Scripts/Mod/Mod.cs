using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using Turing.VFX;

public abstract class Mod : RoutineRunner {

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
    public float damage;
    #endregion

    #region Protected Fields
    protected ModType type;
    protected ModCategory category;
    protected DamagerType damageType;
    protected Stats wielderStats;
    protected IMovable wielderMovable;
    protected List<GameObject> recentlyHitObjects = new List<GameObject>();
    protected bool isAttached;
    protected Damager damager=new Damager();
    #endregion
    
    #region Serialized Unity Inspector fields    
    [SerializeField] protected Collider pickupCollider;    
    [SerializeField] protected EnergySettings energySettings;
    [SerializeField] protected SFXType shootSFX;
    #endregion

    #region Private Fields
    ModSpot spot;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake(){        
        Mod mod = this;
    }

    protected virtual void Update()
    {
        if (energySettings.isCoolingDown)
        {
            energySettings.UpdateCooldownTime(Time.deltaTime);
        }
    }
    #endregion

    #region Public Methods

    public virtual void Activate(Action onCompleteCoolDown=null, Action onActivate=null) {
        if (!energySettings.isInUse && !energySettings.isCoolingDown)
        {
            
            // Timing.RunCoroutine(RunCooldown(onCompleteCoolDown), Segment.Update, coroutineName);
            DoWeaponActions();
            if (onActivate != null)
            {
                onActivate();
            }
            RunCooldown(onCompleteCoolDown);

        }
    }

    protected abstract void DoWeaponActions();

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
        recentlyHitObjects.Clear();
        setModSpot(ModSpot.Default);
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
    protected void RunCooldown(Action onComplete)
    {        
        recentlyHitObjects.Clear();
        energySettings.BeginCoolDown(onComplete);
    }
    #endregion

    #region Private Structures
    [Serializable]
    public class EnergySettings {

        public float timeToCoolDown;

        public bool isCoolingDown;
        public bool isActive;

        public bool isInUse { get { return isCoolingDown || isActive;} }

        public float coolDownTime { get { return timeToCoolDown; } }
        public float attack { get { return attack; } }

        public float cooldownTimer;

        private Action onComplete;

        public void BeginCoolDown(Action onComplete) {            
            isCoolingDown = true;
            cooldownTimer = coolDownTime;
        }

        public void UpdateCooldownTime(float deltaTime)
        {
            cooldownTimer -= deltaTime;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                if (onComplete != null)
                {
                    onComplete();
                    onComplete = null;
                }
            }
        }
    }
    #endregion

}
