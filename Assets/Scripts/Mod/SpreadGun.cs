
using System;
using UnityEngine;

public class SpreadGun : Mod {

#region Serialized fields
    [SerializeField] private SpreadGunProperties gunProperties;
    [SerializeField] private Transform bulletSpawnTransform;
    #endregion

#region Private fields
    private float incrementAngle;
#endregion

    #region Unity lifecycle

    // Use this for initialization
    protected override void Awake()
    {
        setModType(ModType.ForceSegway);
        if(gunProperties.numberOfBulletsInEachShot != 0)
        {
            incrementAngle = gunProperties.spreadAngle / (float)gunProperties.numberOfBulletsInEachShot;
        }
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

    protected override void ActivateStandardArms()
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
        for (int i = 0; i < gunProperties.numberOfBulletsInEachShot; i++)
        {
            //Get bullet and initialize
            GameObject bullet = ObjectPool.Instance.GetObject(PoolObjectType.SpreadGunBullet);
            if (bullet)
            {
                bullet.transform.position = bulletSpawnTransform.position;
                bullet.transform.forward = CalculateForward(i);
                SpreadGunBullet spreadBullet = bullet.GetComponent<SpreadGunBullet>();
                if (spreadBullet)
                {
                    spreadBullet.Init(gunProperties.bulletSpeed, gunProperties.bulletMaxLifeTime, gunProperties.bulletMaxDistance, gunProperties.bulletDamage);
                }
            }
        }
    }

    private Vector3 CalculateForward(int count)
    {
        float rotationAngle = -gunProperties.spreadAngle / 2.0f + (incrementAngle * count);
        return transform.forward;
    }
    #endregion

    #region Public Structures
    [Serializable]
    public class SpreadGunProperties
    {
        [Range(0, 180)]
        [Tooltip("How much you want this baby to spread?")]        
        public float spreadAngle;
        [Tooltip("The name is quite descriptive")]
        public int numberOfBulletsInEachShot;
        [Tooltip("How fast you want the bois to travel?")]
        public float bulletSpeed;
        [Tooltip("How long you want the bois to live?")]
        public float bulletMaxLifeTime;
        [Tooltip("How far you want the bois to go?")]
        public float bulletMaxDistance;
        [Tooltip("How much the bois gon rek?")]
        public float bulletDamage;
    }
#endregion

}
