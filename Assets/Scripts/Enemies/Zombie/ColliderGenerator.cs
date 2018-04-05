using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour {

    [SerializeField] private GameObject prefabWithCollider;

    private ZombieAcider zombieAciderParent;

    private TrailRenderer trailRenderer;
    private float colliderGenerationTime;
    private float currentGenerationTime = 0.0f;
    private float currentAcidTriggerLife;
    private List<GameObject> acidTriggers;
    private Vector3 lastZombiePosition;
    private float zombieY;
    private bool storedYPosition;

    private void OnEnable()
    {
        storedYPosition = false;
    }

    private void Start()
    {
        acidTriggers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update () {
        if (zombieAciderParent != null && zombieAciderParent.gameObject.activeSelf)
        {
            if (!storedYPosition)
            {
                lastZombiePosition = new Vector3(zombieAciderParent.gameObject.transform.position.x, zombieAciderParent.gameObject.transform.position.y, zombieAciderParent.gameObject.transform.position.z) + zombieAciderParent.gameObject.transform.right * 2f - zombieAciderParent.gameObject.transform.forward * 2f;
                zombieY = zombieAciderParent.gameObject.transform.position.y;
                storedYPosition = true;
            }
            else
            {
                lastZombiePosition = new Vector3(zombieAciderParent.gameObject.transform.position.x, zombieY, zombieAciderParent.gameObject.transform.position.z) + zombieAciderParent.gameObject.transform.right * 2f - zombieAciderParent.gameObject.transform.forward * 2f;
            }

            gameObject.transform.position = lastZombiePosition;

            currentGenerationTime += Time.deltaTime;

            if (currentGenerationTime > colliderGenerationTime)
            {
                GameObject acidCollider = ObjectPool.Instance.GetObject(PoolObjectType.AcidTrigger);
                if (acidCollider)
                {
                    acidCollider.transform.position = gameObject.transform.position;
                    AcidTrigger acidTrigger = acidCollider.GetComponent<AcidTrigger>();
                    acidTrigger.SetAcidTriggerLife(currentAcidTriggerLife);
                    acidTrigger.SetZombieParent(zombieAciderParent);
                    acidTriggers.Add(acidCollider);
                }
                currentGenerationTime = 0.0f;
            }
        }

        if (!zombieAciderParent.gameObject.activeSelf)
        {
            gameObject.transform.localPosition  = lastZombiePosition;

            if (trailRenderer.positionCount < 1)
            {
                for (int i = 0; i < acidTriggers.Count; i++)
                {
                    acidTriggers[i].gameObject.SetActive(false);
                }
                gameObject.SetActive(false);
                acidTriggers.Clear();
            }
        }

	}

    public void SetStats(float newColliderGenerationTime,float newAcidTriggerLife)
    {
        colliderGenerationTime = newColliderGenerationTime;
        currentAcidTriggerLife = newAcidTriggerLife;
    }

    public void SetZombieAciderParent(ZombieAcider zombie)
    {
        zombieAciderParent = zombie;
    }

    public void SetTrailRenderer(TrailRenderer trailRendererParameter)
    {
        trailRenderer = trailRendererParameter;
    }

}
