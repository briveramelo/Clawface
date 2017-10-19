using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    int currentSpawner = 0;
    float time = 0.0f;

    bool spawnClear = true;
    bool LevelClear = false;

    private Spawner spawner;

    public List<SpawnerUnit> spawners = new List<SpawnerUnit>();

    void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextSpawner);
        EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXTWAVEENEMIES);

        if (spawners.Count == 0)
        {
            Debug.Log("SpawnManager is Empty");
        }
    }

    private void Update()
    {
        if(currentSpawner == spawners.Count && LevelClear == true)
        {
            LevelClear = false;
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_COMPLETED);
        }
    }

    private void CallNextSpawner(params object[] parameter)
    {
        StartCoroutine(WaitAndSpawn(time));
    }

    IEnumerator WaitAndSpawn(float waitTime)
    {
        if(currentSpawner < spawners.Count)
        {
            time = spawners[currentSpawner].Time;
            spawner = spawners[currentSpawner++].Prefab.GetComponent<Spawner>();
            yield return new WaitForSeconds(waitTime);
            spawner.Activate();
        }
        else
        {
            LevelClear = true;
        }
    }
}

[System.Serializable]
public class SpawnerUnit
{
    public GameObject Prefab;
    public float Time;
}
