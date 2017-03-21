using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ObjectPool : Singleton<ObjectPool> {

    #region Unity Inspector Fields
    [SerializeField] private List<Pool> pools;
    //Add new pools here
    #endregion

    #region Unity LifeCycle
    private void Start () {
        pools.ForEach(pool => pool.Initialize(transform));       
    }
    #endregion

    #region Public Methods
    public GameObject GetObject(PoolObjectType poolObject) {
        Pool currentPool = pools.Find(pool => pool.poolObjectType == poolObject);
        if (currentPool != null) {            
            return currentPool.GetObject();
        }
        string debugMessage = "No object found. Create a pool for this object";
        Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
        return null;
    }
    #endregion

    #region Internal Structures
    [System.Serializable]
    private class Pool
    {
        public List<GameObject> prefabs;
        public int size;
        public PoolObjectType poolObjectType;
        [HideInInspector] public List<GameObject> objects;

        public Pool(ref List<GameObject> prefabs, PoolObjectType poolObjectType, int size) {
            this.objects = new List<GameObject>();
            this.prefabs = prefabs;
            this.size = size;
            this.poolObjectType = poolObjectType;
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
                string debugMessage = "No more" + poolObjectType.ToString() + " objects found! Make your pool bigger than " + size;
                Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
            }
            else{
                objToReturn.SetActive(true);
            }
            return objToReturn;
        }
    }
    #endregion
}
