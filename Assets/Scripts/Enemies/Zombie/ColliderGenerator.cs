using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour {

    [SerializeField] private GameObject prefabWithCollider;
    [SerializeField] private GameObject tentacleBone;
    [SerializeField] private float colliderGenerationTime;

    [SerializeField] private Zombie zombieParent;
    [SerializeField] private ZombieBeserker zombieBeserkerParent;

    private float currentGenerationTime = 0.0f;

	// Update is called once per frame
	void Update () {

        gameObject.transform.position = new Vector3(tentacleBone.transform.position.x, tentacleBone.transform.position.y, tentacleBone.transform.position.z) + tentacleBone.transform.right * 2f - tentacleBone.transform.forward * 2f;

        currentGenerationTime += Time.deltaTime;

        if (currentGenerationTime > colliderGenerationTime)
        {
            GameObject acidCollider = ObjectPool.Instance.GetObject(PoolObjectType.AcidTrigger);
            acidCollider.transform.position = transform.position;
            if(zombieParent)
                acidCollider.GetComponent<AcidTrigger>().SetZombieParent(zombieParent);
            else
                acidCollider.GetComponent<AcidTrigger>().SetZombieParent(zombieBeserkerParent);
            currentGenerationTime = 0.0f;
        }
	}
}
