using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{

    #region Unity Inspector Fields
    [SerializeField]
    private Pool
        stunMinePool,
        blasterBulletPool,
        blasterImpactEffectPool,
        mallCopPool,
        targetExplosionEffectPool,
        stunMineExplosionEffect,
        bloodSplatterPool;
    //Add new pools here
    #endregion

    #region Private Fields
    private Dictionary<PoolObjectType, Pool> pools;
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        stunMinePool.Initialize(transform);
        blasterBulletPool.Initialize(transform);
        blasterImpactEffectPool.Initialize(transform);
        mallCopPool.Initialize(transform);
        targetExplosionEffectPool.Initialize(transform);
        stunMineExplosionEffect.Initialize(transform);


        pools = new Dictionary<PoolObjectType, Pool>()
        {
            { PoolObjectType.Mine, stunMinePool },
            { PoolObjectType.BlasterBullet, blasterBulletPool },
            { PoolObjectType.BlasterImpactEffect, blasterImpactEffectPool },
            { PoolObjectType.MallCop, mallCopPool },
            { PoolObjectType.TargetExplosionEffect, targetExplosionEffectPool },
            { PoolObjectType.MineExplosionEffect, stunMineExplosionEffect },
            { PoolObjectType.BloodDecal, bloodSplatterPool}
            //Add new pools here
        };
    }
    #endregion

    #region Public Methods
    public GameObject GetObject(PoolObjectType poolObject)
    {
        if (pools.ContainsKey(poolObject))
        {
            return pools[poolObject].GetObject();
        }
        string debugMessage = "No object found. Create a pool for this object";
        Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
        return null;
    }
    #endregion

    #region Internal Structures
    [System.Serializable]
    struct Pool
    {
        public GameObject prefab;
        public int size;
        [HideInInspector]
        public List<GameObject> objects;

        public Pool(ref GameObject prefab, int size)
        {
            this.objects = new List<GameObject>();
            this.prefab = prefab;
            this.size = size;
        }

        public void Initialize(Transform grandparent)
        {
            GameObject parent = new GameObject(prefab.name);
            parent.transform.SetParent(grandparent);
            for (int index = 0; index < size; index++)
            {
                GameObject item = Instantiate(prefab, parent.transform) as GameObject;
                item.SetActive(false);
                objects.Add(item);
            }
        }

        public GameObject GetObject()
        {
            GameObject objToReturn = objects.Find(obj => !obj.activeSelf);
            if (objToReturn == null)
            {
                string debugMessage = "No more" + prefab.name + " objects found! Make your pool bigger than " + size;
                Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
            }
            return objToReturn;
        }
    }
    #endregion
}
