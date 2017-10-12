using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MovementEffects;
using System.Linq;
using UnityEditorInternal;

[System.Serializable]
public class Spawner : RoutineRunner
{
    public bool useIntensityCurve, manualEdits;
    public AnimationCurve intensityCurve;

    public List<Wave> waves = new List<Wave>();

    public int currentWaveNumber = 0;
    public int currentNumEnemies = 0;

    #region private variables

    private int currentWave = 0;

    List<Transform> spawnPoints = new List<Transform>();


    private PoolObjectType GetPoolObject(SpawnType spawnType)
    {
        switch (spawnType)
        {
            case SpawnType.Blaster:
                return PoolObjectType.MallCopBlaster;
            case SpawnType.Zombie:
                return PoolObjectType.Zombie;
            case SpawnType.Bouncer:
                return PoolObjectType.Bouncer;
            case SpawnType.Kamikaze:
                return PoolObjectType.Kamikaze;
        }
        return PoolObjectType.MallCopBlaster;
    }


    #endregion

    #region Unity LifeCycle
    void Start()
    {

    }

    private void Update()
    {

    }
    #endregion

    public void Activate()
    {
        foreach (Transform child_point in transform)
        {
            spawnPoints.Add(child_point);
        }

        CheckToSpawnEnemyCluster();
    }


    #region Private Methods
    private void ReportDeath()
    {
        currentNumEnemies--;

        if (currentWave < waves.Count - 1 && currentNumEnemies <= 1 * spawnPoints.Count)
        {
            GoToNextWave();
        }
    }

    private void GoToNextWave()
    {
        currentWave++;
        currentWaveNumber = currentWave;
        CheckToSpawnEnemyCluster();
    }

    private void CheckToSpawnEnemyCluster()
    {
        if (Application.isPlaying)
        {
            if (currentWave < waves.Count)
            {
                Timing.RunCoroutine(SpawnEnemyCluster(), coroutineName);
            }
        }
    }

    private IEnumerator<float> SpawnEnemyCluster()
    {
        yield return Timing.WaitForSeconds(0.0f);

        for(int i = 0; i < waves[currentWave].monsterList.Count; i++)
        {
            for(int j = 0; j < waves[currentWave].monsterList[i].Count; j++)
            {
                GameObject spawnedObject = ObjectPool.Instance.GetObject(GetPoolObject(waves[currentWave].monsterList[i].Type));

                if (spawnedObject)
                {
                    ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                    if (!spawnable.HasWillBeenWritten())
                    {
                        spawnable.RegisterDeathEvent(ReportDeath);
                    }

                    Vector3 spawnPosition = spawnPoints.GetRandom().position;
                    spawnedObject.transform.position = spawnPosition;
                    spawnable.WarpToNavMesh(spawnPosition);

                    currentNumEnemies++;
                }
                else
                {
                    Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECT" + "</color>");
                }
            }
        }


        /*
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            yield return Timing.WaitForSeconds(Random.Range(waves[currentWave].SpawningTime.Min, waves[currentWave].SpawningTime.Max));

            foreach (Transform point in spawnPoints)
            {
                GameObject spawnedObject = ObjectPool.Instance.GetObject(GetPoolObject(spawnType));

                if (spawnedObject)
                {
                    ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                    if (!spawnable.HasWillBeenWritten())
                    {
                        spawnable.RegisterDeathEvent(ReportDeath);
                    }

                    spawnedObject.transform.position = point.position;
                    spawnable.WarpToNavMesh(point.position);

                    currentNumEnemies++;
                }
                else
                {
                    Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECT" + "</color>");
                }
            }
        }
        */


    }


    #endregion

    public bool IsLastWave()
    {
        return currentWave >= waves.Count - 1 ? true : false;
    }

    public bool IsAllEnemyClear()
    {
        return currentNumEnemies == 0 ? true : false;
    }
}



[System.Serializable]
public class WaveType
{
    public SpawnType Type;
    public int Count;
}

[System.Serializable]
public class Wave
{
    #region const parameters

    const int spawnMin = 1;

    const float timeBetweenMin = 0.25f;
    const float timeBetweenMax = 2.0f;

    const float TimeToNextWave_Max = 60.0f;

    #endregion

    public List<int> spawnedHashCodes = new List<int>();

    public List<WaveType> monsterList;

    [HideInInspector] public int remainingSpawns;
    [SerializeField, Range(0, 1)] public float intensity;
    [SerializeField, Range(0, 10)] public int NumToNextWave;

    [SerializeField, Range(0, TimeToNextWave_Max)] float TimeToNextWave;

    public int spawnOffset;
    public float spawnTimeOffset;

    public float Intensity
    {
        get { return intensity; }
        set
        {            
            intensity = Mathf.Clamp01(value);
            ApplyIntensityValue();
        }
    }

    public float Time
    {
        get { return TimeToNextWave; }
        set
        {
            value = value < 0 ? 0 : value;
            value = value > 1 ? 1 : value;

            TimeToNextWave = TimeToNextWave_Max * value;
        }
    }


    public void ApplyIntensityValue()
    {
        SetTimeBetweenSpawns(intensity);
    }

    public FloatRange SpawningTime;
    
    void SetTimeBetweenSpawns(float intensity)
    {
        float timeBase = Mathf.Clamp(timeBetweenMax * (1 - intensity), timeBetweenMin, timeBetweenMax);
        SpawningTime.Min = Mathf.Clamp(timeBase - spawnTimeOffset, timeBetweenMin, timeBetweenMax);
        SpawningTime.Max = Mathf.Clamp(timeBase + spawnTimeOffset, timeBetweenMin, timeBetweenMax);
    }

    public void Reset()
    {
        spawnedHashCodes.Clear();
    }

    public bool ContainsHash(int itemHash)
    {
        return spawnedHashCodes.Contains(itemHash);
    }

    public void RemoveItemHash(int itemHash)
    {
        spawnedHashCodes.Remove(itemHash);
    }

    public IEnumerator<float> IERunSpawnSequence(System.Func<int> onSpawn)
    {
        Reset();
        while (true)
        {
            spawnedHashCodes.Add(onSpawn());
            remainingSpawns--;
            if (remainingSpawns <= 0)
            {
                break;
            }
            yield return Timing.WaitForSeconds(SpawningTime.GetRandomValue());
        }
    }

}