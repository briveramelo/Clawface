using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class ObjectPool : Singleton<ObjectPool> {

    protected ObjectPool() { }

    #region Unity Inspector Fields
    [SerializeField] public List<Pool> pools;
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
    public void ResetPools()
    {
        pools.ForEach(pool => { pool.Reset(); });
    }
    void OnDisable() {
        //pools.ForEach(pool => print(pool.poolObjectType + " uses: " + pool.mostUsed + " stored: " + pool.size));
    }
    #endregion

}

#region Internal Structures
[System.Serializable]
public class Pool
{
    [HideInInspector] public string name;
    [HideInInspector] public Transform grandParent;
    public PoolObjectType poolObjectType;
    public List<GameObject> prefabs = new List<GameObject>();
    public int size;
    public int mostUsed;
    [HideInInspector] public List<GameObject> objects = new List<GameObject>();

    public Pool(ref List<GameObject> prefabs, PoolObjectType poolObjectType, int size) {
        this.prefabs = prefabs;
        this.size = size;
        this.poolObjectType = poolObjectType;
        this.name = poolObjectType.ToString();
    }
    public Pool() {
        this.size = 1;
        this.poolObjectType = PoolObjectType.BlasterBullet;
        this.name = "";
    }

    public void Initialize(Transform greatGrandParent) {
        this.name = poolObjectType.ToString();
        grandParent = (prefabs.Count > 1 ? new GameObject(prefabs[0].name + "s") : greatGrandParent.gameObject).transform;
        grandParent.SetParent(greatGrandParent);

        int cummulativeObjectsSpawned = 0;
        for (int prefabListIndex = 0; prefabListIndex < prefabs.Count; prefabListIndex++) {

            GameObject parent = new GameObject(prefabs[prefabListIndex].name);
            parent.transform.SetParent(grandParent);
            int numToSpawn = size / prefabs.Count;
            if ((prefabListIndex == prefabs.Count - 1) && (size - cummulativeObjectsSpawned>0)) {
                numToSpawn = size - cummulativeObjectsSpawned;
            }

            for (int i = 0; i < numToSpawn; i++) {
                CreateItem(prefabListIndex, parent);
            }
        }
    }

    void CreateItem(int prefabListIndex, GameObject parent) {
        GameObject item = MonoBehaviour.Instantiate(prefabs[prefabListIndex], parent.transform) as GameObject;
        item.AddComponent<ObjectPoolItem>().SetParent(parent.transform);
        item.SetActive(false);
        objects.Add(item);
    }

    public GameObject GetObject()
    {
        GameObject objToReturn = objects.Find(obj => obj!=null && !obj.activeSelf);
        if (objToReturn==null) {
            string debugMessage = "No more " + poolObjectType.ToString() + " objects found! Make your pool bigger than " + size;
            Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
        }
        else{
            int currentIndex = objects.FindIndex(obj => obj == objToReturn);
            if (currentIndex>mostUsed) {
                mostUsed = currentIndex+1;
            }
            objToReturn.SetActive(true);
        }
        return objToReturn;
    }

    public void Reset()
    {
        objects.ForEach(obj => {
            if (obj) {
                obj.SetActive(false);
                obj.GetComponent<ObjectPoolItem>().ResetParent();
            }
            else {
                objects.Remove(obj);
                int prefabIndex = Random.Range(0, prefabs.Count);
                CreateItem(prefabIndex, grandParent.gameObject);
            }
        });
    }
}
#endregion
