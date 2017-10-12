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


    private void Update()
    {
        if(spawnClear)
        {
            if(currentSpawner < spawners.Count)
            {
                StartCoroutine(WaitAndSpawn(time));
            }
            else if (currentSpawner == spawners.Count && LevelClear == false)
            {
                LevelClear = true;
                EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_COMPLETED);
            }
        }

        spawnClear = spawner.IsAllEnemyClear() && spawner.IsLastWave();
    }


    IEnumerator WaitAndSpawn(float waitTime)
    {
        time = spawners[currentSpawner].Time;
        spawner = spawners[currentSpawner++].Prefab.GetComponent<Spawner>();
        yield return new WaitForSeconds(waitTime);
        spawner.Activate();
    }
}

[System.Serializable]
public class SpawnerUnit
{
    public GameObject Prefab;
    public float Time;
}