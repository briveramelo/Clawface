using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool> {

    [SerializeField] private Pool stunMinePool, blasterBulletPool;

    // Use this for initialization
    private void Start () {
        stunMinePool.Initialize(transform);
        blasterBulletPool.Initialize(transform);
    }

    public GameObject GetStunMine()
    {
        return stunMinePool.GetObject();        
    }

    public GameObject GetBlasterBullet()
    {
        return blasterBulletPool.GetObject();
    }

    [System.Serializable]
    struct Pool
    {
        public GameObject prefab;
        public int size;
        [HideInInspector] public List<GameObject> objects;

        public Pool(ref GameObject prefab, int size) {
            this.objects = new List<GameObject>();
            this.prefab = prefab;
            this.size = size;
        }

        public void Initialize(Transform grandparent) {
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
            return objects.Find(obj => !obj.activeSelf);
        }
    }
}
