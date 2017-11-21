using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningGun : Mod {

    #region Public fields
    public IMovable WielderMovable { get { return wielderMovable; } }
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private Transform muzzleTransform;    
    [SerializeField] private ProjectileProperties projectileProperties;
    #endregion

    private Animator animator;

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Awake() {
        type = ModType.LightningGun;
        category = ModCategory.Ranged;
        animator = GetComponentInChildren<Animator>();
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
            SFXManager.Instance.Play(shootSFX, transform.position);
        };   
        base.Activate(onCompleteCoolDown, onActivate);
    }
    
    protected override void DoWeaponActions(){        
        //Get Projectile
        GameObject hookObject = ObjectPool.Instance.GetObject(PoolObjectType.LightningProjectile);        
        if (hookObject)
        {
            //Initialize
            LightningProjectile currentHook = hookObject.GetComponent<LightningProjectile>();
            ProjectileProperties newProperties = new ProjectileProperties(projectileProperties);            
            currentHook.Init(projectileProperties, muzzleTransform);
            animator.SetTrigger("Shoot");
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
    #endregion

    #region Public structures
    [Serializable]
    public class ProjectileProperties
    {
        [Tooltip("How fast you want the lightning to go bruh?")]
        public float projectileSpeed;
        [Tooltip("How hard you want the lightning to hit bruh?")]
        public float projectileHitDamage;
        [Tooltip("Angle within which the lightning will spot his bitch-ass")]
        public float homingAngle;
        [Tooltip("Radius within which the lightning will spot his bitch-ass")]
        public float homingRadius;
        [Tooltip("Max fucc-bois the lightning can attach to")]
        public int maxChainableEnemies;
        [Tooltip("Max distance the lightning can travel")]
        public float maxDistance;
        [Tooltip("Max distance the chain homies can travel")]
        public float maxDistancePerSubChain;

        public ProjectileProperties(ProjectileProperties other)
        {
            projectileSpeed = other.projectileSpeed;
            projectileHitDamage = other.projectileHitDamage;
            homingAngle = other.homingAngle;
            homingRadius = other.homingRadius;
            maxChainableEnemies = other.maxChainableEnemies;
            maxDistance = other.maxDistance;
            maxDistancePerSubChain = other.maxDistancePerSubChain;
        }
    }
    #endregion

}
