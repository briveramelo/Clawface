
using System;
using UnityEngine;

public class SpreadGun : Mod {

    #region Serialized fields
    [SerializeField] private SpreadGunProperties gunProperties;
    [SerializeField] private Transform bulletSpawnTransform;
    [SerializeField] private PoolObjectType shootVFX = PoolObjectType.VFXSpreadshotShoot;
    #endregion

    #region Private fields
    private Animator animator;
    #endregion

    #region Unity lifecycle

    // Use this for initialization
    protected override void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        setModType(ModType.SpreadGun);
        base.Awake();       
    }

    protected override void Update()
    {
        if (wielderMovable != null)
        {
            transform.forward = wielderMovable.GetForward();
        }
    }
    #endregion

    #region Public functions
    public override void AttachAffect(ref Stats wielderStats, IMovable wielderMovable)
    {
        base.AttachAffect(ref wielderStats, wielderMovable);
    }

    public override void Activate(Action onCompleteCoolDown=null, Action onActivate=null)
    {        
        base.Activate(onCompleteCoolDown, onActivate);   
    }

    protected override void DoWeaponActions()
    {
        Shoot();
    }

    public override void DeActivate()
    {
        
    }

    public override void DetachAffect()
    {
        base.DetachAffect();
    }
    #endregion

    #region Private functions
    private void Shoot()
    {
        //Get bullet and initialize
        GameObject bullet = ObjectPool.Instance.GetObject(PoolObjectType.SpreadGunBullet);
        if (bullet)
        {
            bullet.transform.SetParent(null);
            bullet.transform.position = bulletSpawnTransform.position;
            bullet.transform.forward = transform.forward;
            SpreadGunBullet spreadBullet = bullet.GetComponent<SpreadGunBullet>();
            if (spreadBullet)
            {
                spreadBullet.Init(gunProperties);
            }
        }
        SFXManager.Instance.Play(shootSFX, transform.position);
        animator.SetTrigger("Shoot");

        GameObject vfx = ObjectPool.Instance.GetObject(shootVFX);
        if (vfx)
        {
            vfx.transform.SetParent (transform);
            vfx.transform.position = bulletSpawnTransform.position;
            vfx.transform.rotation = bulletSpawnTransform.rotation;
        }
    }
    #endregion

    #region Public Structures
    [Serializable]
    public class SpreadGunProperties
    {
        public float bulletSpeed;
        public float bulletMaxDistance;
        public float bulletMinScale;
        public float bulletMaxScale;
        public float bulletMinDamage;
        public float bulletMaxDamage;
        public float bulletMinAlpha;
    }
#endregion

}
