using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ObjectPool : Singleton<ObjectPool> {

    #region Unity Inspector Fields
    [SerializeField] private Pool 
        stunMinePool,
        blasterBulletPool,
        blasterImpactEffectPool,
        mallCopPool,
        targetExplosionEffectPool,
        stunMineExplosionEffect,
        bloodDecalPool;
    //Add new pools here
    #endregion

    #region Private Fields
    private Dictionary<PoolObjectType, Pool> pools;
    #endregion

    #region Unity LifeCycle
    private void Start () {
        stunMinePool.Initialize(transform);
        blasterBulletPool.Initialize(transform);
        blasterImpactEffectPool.Initialize(transform);
        mallCopPool.Initialize(transform);
        targetExplosionEffectPool.Initialize(transform);
        stunMineExplosionEffect.Initialize(transform);
        bloodDecalPool.Initialize(transform);


        pools = new Dictionary<PoolObjectType, Pool>()
        {
            { PoolObjectType.Mine, stunMinePool },
            { PoolObjectType.BlasterBullet, blasterBulletPool },
            { PoolObjectType.BlasterImpactEffect, blasterImpactEffectPool },
            { PoolObjectType.MallCop, mallCopPool },
            { PoolObjectType.TargetExplosionEffect, targetExplosionEffectPool },
            { PoolObjectType.MineExplosionEffect, stunMineExplosionEffect },
            {PoolObjectType.BloodDecal, bloodDecalPool }
            
            //Add new pools here
        };
    }
    #endregion

    #region Public Methods
    public GameObject GetObject(PoolObjectType poolObject) {
        if (pools.ContainsKey(poolObject)) {
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
        public List<GameObject> prefabs;
        public int size;
        [HideInInspector] public List<GameObject> objects;

        public Pool(ref List<GameObject> prefabs, int size) {
            this.objects = new List<GameObject>();
            this.prefabs = prefabs;
            this.size = size;
        }

        public void Initialize(Transform greatGrandParent) {
            GameObject grandParent = prefabs.Count > 1 ? new GameObject(prefabs[0].name + "s") : greatGrandParent.gameObject;
            grandParent.transform.SetParent(greatGrandParent);

            int cummulativeObjectsSpawned = 0;
            for (int prefabListIndex = 0; prefabListIndex < prefabs.Count; prefabListIndex++) {

                GameObject parent = new GameObject(prefabs[prefabListIndex].name);
                parent.transform.SetParent(grandParent.transform);
                int numToSpawn = size / prefabs.Count;
                if ((prefabListIndex == prefabs.Count - 1) && (size - cummulativeObjectsSpawned>0)) {
                    numToSpawn = size - cummulativeObjectsSpawned;
                }

                for (int i = 0; i < numToSpawn; i++) {
                    GameObject item = Instantiate(prefabs[prefabListIndex], parent.transform) as GameObject;
                    item.SetActive(false);
                    objects.Add(item);
                    cummulativeObjectsSpawned++;
                }
            }
        }

        public GameObject GetObject()
        {
            GameObject objToReturn = objects.GetRandom(obj => !obj.activeSelf);
            //GameObject thing = objects.Find(obj => !obj.activeSelf);
            if (objToReturn==null) {
                string debugMessage = "No more" + prefabs[0].name + " objects found! Make your pool bigger than " + size;
                Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
            }
            return objToReturn;
        }
    }
    #endregion
}
