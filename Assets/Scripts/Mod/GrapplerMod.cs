using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerMod : Mod {

    #region Public fields
    public IMovable WielderMovable { get { return wielderMovable; } }
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Transform hookTransform;    
    [SerializeField] private HookProperties hookProperties;
    #endregion

    #region Private Fields
    private bool hitTargetThisShot;
    private Hook currentHook;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.Grappler;
        category = ModCategory.Ranged;
        currentHook = null;
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update () {
        if (wielderMovable != null)
        {
            Vector3 forward = wielderMovable.GetForward().normalized;
            forward.y = 0f;
            transform.forward = forward;
        }        
	}
    #endregion

    #region Public Methods
    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {  
        onActivate = () => {            
            SFXManager.Instance.Play(SFXType.GrapplingGun_Shoot, transform.position);
        };   
        base.Activate(onCompleteCoolDown, onActivate);
    }
    
    protected override void ActivateStandardArms(){
        //Is there a projectile already out there?
        if (currentHook)
        {
            currentHook.transform.SetParent(null);
            currentHook.ResetToDefaults();
        }
        //Get Projectile
        GameObject hookObject = ObjectPool.Instance.GetObject(PoolObjectType.GrapplingHook);        
        if (hookObject)
        {
            //Initialize
            currentHook = hookObject.GetComponent<Hook>();
            currentHook.transform.SetParent(hookTransform);
            HookProperties newProperties = new HookProperties(hookProperties);            
            currentHook.Init(hookProperties, hookTransform);
        }
    }    

    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable){
        base.AttachAffect(ref wielderStats, wielderMovable);        
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect(){
        base.DetachAffect();
    }

    public bool GetHitTargetThisShot() { return hitTargetThisShot; }
    public void SetHitTargetThisShot(bool hitTarget) { hitTargetThisShot = hitTarget;}
    #endregion

    #region Public structures
    [Serializable]
    public class HookProperties
    {
        [Tooltip("How fast you want the hook to go bruh?")]
        public float projectileSpeed;
        [Tooltip("How hard you want the hook to hit bruh?")]
        public float projectileHitDamage;
        [Tooltip("Angle within which the hook will spot his bitch-ass")]
        public float homingAngle;
        [Tooltip("Radius within which the hook will spot his bitch-ass")]
        public float homingRadius;
        [Tooltip("Damage hook finna do after attaching")]
        public float projectileDamagePerSecond;
        [Tooltip("Max fucc-bois the hook can attach to")]
        public int maxChainableEnemies;
        [Tooltip("Max distance the hook can travel")]
        public float maxDistance;
        [Tooltip("Max distance the chain homies can travel")]
        public float maxDistancePerSubChain;

        public HookProperties(HookProperties other)
        {
            projectileSpeed = other.projectileSpeed;
            projectileHitDamage = other.projectileHitDamage;
            homingAngle = other.homingAngle;
            homingRadius = other.homingRadius;
            projectileDamagePerSecond = other.projectileDamagePerSecond;
            maxChainableEnemies = other.maxChainableEnemies;
            maxDistance = other.maxDistance;
            maxDistancePerSubChain = other.maxDistancePerSubChain;
        }
    }
    #endregion

}
