using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    public static BulletPool instance;

    [SerializeField]
    private GameObject stunMinePrefab;

    [SerializeField]
    private GameObject blasterBulletPrefab;

    [SerializeField]
    private int stunMinePoolSize;

    [SerializeField]
    private int blasterBulletPoolSize;

    List<GameObject> stunMinePool;

    List<GameObject> blasterBulletPool;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(instance);
    }

    // Use this for initialization
    void Start () {
        InitializeStunMinePool();
        InitializeBlasterBulletPool();
    }

    void InitializeStunMinePool()
    {
        stunMinePool = new List<GameObject>();
        for (int index = 0; index < stunMinePoolSize; index++)
        {
            GameObject stunMine = Instantiate(stunMinePrefab, DynamicObjectParent.instance.transform);
            stunMine.SetActive(false);
            stunMinePool.Add(stunMine);
        }
    }

    void InitializeBlasterBulletPool()
    {
        blasterBulletPool = new List<GameObject>();
        for (int index = 0; index < blasterBulletPoolSize; index++)
        {
            GameObject blasterBullet = Instantiate(blasterBulletPrefab, DynamicObjectParent.instance.transform);
            blasterBullet.SetActive(false);
            blasterBulletPool.Add(blasterBullet);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public GameObject getStunMine()
    {
        foreach(GameObject stunMine in stunMinePool)
        {
            if (!stunMine.activeSelf)
            {
                return stunMine;
            }
        }
        return null;
    }

    public GameObject getBlasterBullet()
    {
        foreach (GameObject blasterBullet in blasterBulletPool)
        {
            if (!blasterBullet.activeSelf)
            {
                return blasterBullet;
            }
        }
        return null;
    }
}
