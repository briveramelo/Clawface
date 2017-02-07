using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    public static BulletPool instance;

    [SerializeField]
    private GameObject stunMinePrefab;

    [SerializeField]
    private int stunMinePoolsize;

    List<GameObject> stunMinePool;

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
        stunMinePool = new List<GameObject>();
        for (int index=0; index < stunMinePoolsize; index++)
        {
            GameObject stunMine = Instantiate(stunMinePrefab);
            stunMine.SetActive(false);
            stunMinePool.Add(stunMine);
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
}
