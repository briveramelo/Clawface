using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;

public abstract class Mod : MonoBehaviour {

    #region Public fields
    public bool hasState;
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
    protected Stats wielderStats;
    protected IMovable wielderMovable;
    protected List<GameObject> recentlyHitObjects = new List<GameObject>();
    protected bool isAttached;
    protected Damager damager=new Damager();
    protected string coroutineString { get { return GetInstanceID().ToString(); } }
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
            Timing.RunCoroutine(RunCooldown(onCompleteCoolDown), Segment.FixedUpdate);
            useAction();
            if (onActivate != null)
            {
                onActivate();
            }
        }
    }

    protected abstract void ActivateStandardArms();

    public abstract void DeActivate();

    public virtual void AttachAffect(ref Stats wielderStats, IMovable wielderMovable) {
        isAttached = true;
        this.wielderStats = wielderStats;
        this.wielderMovable = wielderMovable;
        pickupCollider.enabled = false;
    }

    public virtual void DetachAffect() {
        //Debug.Log("detached");
        isAttached = false;
        wielderMovable = null;
        wielderStats = null;
        recentlyHitObjects.Clear();
        //pickupCollider.enabled = true;
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

    private Action useAction {
        get {            
            return ActivateStandardArms;
        }
    }

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
    [System.Serializable]
    public class EnergySettings {

        
        
        public AttackSettings standardArmAttackSettings;        
        

        
        [HideInInspector] public bool isCoolingDown;
        [HideInInspector] public bool isActive;
        

        private Mod mod;

        public bool isInUse { get { return isCoolingDown || isActive;} }

        public float coolDownTime { get { return attackSettings.timeToCoolDown; } }
        public float attack { get { return attackSettings.attack; } }
        
        public float timeToAttack { get { return attackSettings.timeToAttack; } }
        
        public AttackSettings attackSettings {
            get {
                return standardArmAttackSettings;
            }
        }

        public IEnumerator<float> BeginCoolDown() {            
            isCoolingDown = true;
            yield return Timing.WaitForSeconds(coolDownTime);
            isCoolingDown = false;
        }
        public void Initialize(ref Mod mod) {
            this.mod = mod;
        }
    }

    [System.Serializable]
    public class AttackSettings {
        public float timeToCoolDown;
        public float timeToAttack;
        public float attack;
    }
    #endregion

}
