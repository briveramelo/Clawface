using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour {

    [SerializeField] private GameObject prefabWithCollider;
    [SerializeField] private GameObject tentacleBone;

    [SerializeField] private Zombie zombieParent;
    [SerializeField] private ZombieBeserker zombieBeserkerParent;
    [SerializeField] private ZombieAcider zombieAciderParent;

    private float colliderGenerationTime;
    private float currentGenerationTime = 0.0f;
    private float currentAcidTriggerLife;

	// Update is called once per frame
	void Update () {

        gameObject.transform.position = new Vector3(tentacleBone.transform.position.x, tentacleBone.transform.position.y, tentacleBone.transform.position.z) + tentacleBone.transform.right * 2f - tentacleBone.transform.forward * 2f;

        currentGenerationTime += Time.deltaTime;

        if (currentGenerationTime > colliderGenerationTime)
        {
            GameObject acidCollider = ObjectPool.Instance.GetObject(PoolObjectType.AcidTrigger);
            acidCollider.transform.position = transform.position;
            acidCollider.GetComponent<AcidTrigger>().SetAcidTriggerLife(currentAcidTriggerLife);
            acidCollider.GetComponent<AcidTrigger>().SetZombieParent(zombieAciderParent);
            currentGenerationTime = 0.0f;
        }
	}

    public void SetStats(float newColliderGenerationTime,float newAcidTriggerLife)
    {
        colliderGenerationTime = newColliderGenerationTime;
        currentAcidTriggerLife = newAcidTriggerLife;
    }

}
