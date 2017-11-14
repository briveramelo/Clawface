using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;
using Turing.VFX;

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
    protected string coroutineString { get { return GetInstanceID().ToString(); } }    
    #endregion
    
    #region Serialized Unity Inspector fields    
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
    }

    protected virtual void Update()
    {        
    }
    #endregion

    #region Public Methods    
    public void KillCoroutines() {
        Timing.KillCoroutines(coroutineString);
    }

    public virtual void Activate(Action onCompleteCoolDown=null, Action onActivate=null) {
        if (!energySettings.isInUse && !energySettings.isCoolingDown)
        {
            energySettings.isCoolingDown = true;
            Timing.RunCoroutine(RunCooldown(onCompleteCoolDown), Segment.FixedUpdate);
            DoWeaponActions();
            if (onActivate != null)
            {
                onActivate();
            }
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
    protected IEnumerator<float> RunCooldown(Action onComplete)
    {        
        recentlyHitObjects.Clear();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(energySettings.BeginCoolDown(), Segment.FixedUpdate));
        if (onComplete != null)
        {
            onComplete();
        }
    }
    #endregion

    #region Private Structures
    [Serializable]
    public class EnergySettings {

        public float timeToCoolDown;

        [HideInInspector] public bool isCoolingDown;
        [HideInInspector] public bool isActive;

        public bool isInUse { get { return isCoolingDown || isActive;} }

        public float coolDownTime { get { return timeToCoolDown; } }
        public float attack { get { return attack; } }
                
        public IEnumerator<float> BeginCoolDown() {            
            isCoolingDown = true;
            yield return Timing.WaitForSeconds(coolDownTime);
            isCoolingDown = false;
        }
    }
    #endregion

}
