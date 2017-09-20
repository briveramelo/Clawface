
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
        if(gunProperties.numberOfBulletsInEachShot - 1 != 0)
        {
            incrementAngle = gunProperties.spreadAngle / (gunProperties.numberOfBulletsInEachShot - 1);
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
                bullet.transform.SetParent(null);
                bullet.transform.position = bulletSpawnTransform.position;
                bullet.transform.forward = CalculateForward(i);
                SpreadGunBullet spreadBullet = bullet.GetComponent<SpreadGunBullet>();
                if (spreadBullet)
                {
                    spreadBullet.Init(gunProperties.bulletSpeed, gunProperties.bulletMaxDistance, gunProperties.bulletDamage);
                }
            }
        }
    }

    private Vector3 CalculateForward(int count)
    {
        Vector3 forwardVector2D = new Vector3(transform.forward.x, 0f, transform.forward.z);
        float rotationAngle = -gunProperties.spreadAngle / 2.0f + (incrementAngle * count);        
        return Quaternion.Euler(0f, rotationAngle, 0f) * forwardVector2D;
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
        [Tooltip("How far you want the bois to go?")]
        public float bulletMaxDistance;
        [Tooltip("How much the bois gon rek?")]
        public float bulletDamage;
    }
#endregion

}
