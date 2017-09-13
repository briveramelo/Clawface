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
            currentHook.gameObject.SetActive(false);
        }
        //Get Projectile
        GameObject hookObject = ObjectPool.Instance.GetObject(PoolObjectType.GrapplingHook);        
        if (hookObject)
        {
            //Initialize
            currentHook = hookObject.GetComponent<Hook>();
            currentHook.transform.SetParent(hookTransform);
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

    #region Private Methods
    #endregion

    #region Private Structures
    [Serializable]
    public class HookProperties
    {        
        public float projectileSpeed;
        public float projectileHitDamage;
        public float projectileDamagePerFrame;
        public float homingAngle;
        public float homingDistance;
        public int maxChainableEnemies;
        public float maxDistance;
    }
    #endregion

}
