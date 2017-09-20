using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;
using System.Linq;
public class Spawner : MonoBehaviour
{
    public bool useIntensityCurve, manualEdits;
    public AnimationCurve intensityCurve;
    public AnimationCurve timingCurve;

    public List<Wave> waves = new List<Wave>();

    public int currentWaveNumber = 0;
    public int currentNumEnemies = 0;
    public float TimeToNextWave = 0.0f;

    #region Serialized Unity Fields
    [SerializeField] SpawnType spawnType;
    #endregion


    #region private variables

    private int currentWave = 0;

    List<Transform> spawnPoints = new List<Transform>();


    private PoolObjectType objectToSpawn
    {
        get
        {
            switch (spawnType)
            {
                case SpawnType.Blaster:
                    return PoolObjectType.MallCopBlaster;
                case SpawnType.Grappler:
                    return PoolObjectType.GrapplingBot;
            }
            return PoolObjectType.MallCopBlaster;
        }
    }

    #endregion

    #region Unity LifeCycle
    void Start()
    {
        if(waves.Count > 0) TimeToNextWave = waves[0].Time;

        foreach (Transform child_point in transform)
        {
            spawnPoints.Add(child_point);
        }

        CheckToSpawnEnemyCluster();
    }

    private void Update()
    {

        TimeToNextWave -= Time.deltaTime;

        if(TimeToNextWave < 0.0f)
        {
            if(currentWave < waves.Count)
            {
                GoToNextWave();
                TimeToNextWave = waves[currentWave].Time;
            }
            else
            {
                TimeToNextWave = 0.0f;
            }
        }
    }


    #endregion

    #region Private Methods
    private void ReportDeath()
    {
        currentNumEnemies--;

        if (currentWave < waves.Count && currentNumEnemies <= waves[currentWave].totalNumSpawns.Min * spawnPoints.Count)
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
                Timing.RunCoroutine(SpawnEnemyCluster());
            }
        }
    }

    private IEnumerator<float> SpawnEnemyCluster()
    {
        int enemiesToSpawn = waves[currentWave].totalNumSpawns.Max;

        for (int i = 0;  i < enemiesToSpawn; i++)
        {
            yield return Timing.WaitForSeconds(Random.Range(waves[currentWave].SpawningTime.Min, waves[currentWave].SpawningTime.Max));

            foreach (Transform point in spawnPoints)
            {
                GameObject spawnedObject = ObjectPool.Instance.GetObject(objectToSpawn);

                if (spawnedObject)
                {
                    ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                    if (!spawnable.HasWillBeenWritten())
                    {
                        spawnable.RegisterDeathEvent(ReportDeath);
                    }

                    spawnedObject.transform.position = point.position;
                    currentNumEnemies++;
                }
                else
                {
                    Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECT" + "</color>");
                }
            }
        }
    }

#endregion

    public bool IsLastWave()
    {
        return currentWave >= waves.Count ? true : false;
    }

    public bool IsAllEnemyClear()
    {
        return currentNumEnemies == 0 ? true : false;
    }
}



[System.Serializable]
public class Wave
{

    #region const parameters

    const int spawnMin = 1;
    const int spawnMax = 15;
    const float timeBetweenMin = 0.25f;
    const float timeBetweenMax = 2.0f;

    const int spawnOffset = 1;
    const float spawnTimeOffset = 0.3f;

    const float TimeToNextWave_Max = 60.0f;

    #endregion

    public List<int> spawnedHashCodes = new List<int>();

    [HideInInspector] public int remainingSpawns;
    [SerializeField, Range(0, 1)] float intensity;
    [SerializeField, Range(0, TimeToNextWave_Max)] float TimeToNextWave;

    public float Intensity
    {
        get { return intensity; }
        set
        {
            value = value < 0 ? 0 : value;
            value = value > 1 ? 1 : value;
            intensity = value;
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
        SetTotalSpawns(intensity);
        SetTimeBetweenSpawns(intensity);
    }

    [IntRange(spawnMin, spawnMax)] public IntRange totalNumSpawns;
    [FloatRange(timeBetweenMin, timeBetweenMax)] public FloatRange SpawningTime;

    void SetTotalSpawns(float intensity)
    {
        float spawnBase = intensity * spawnMax;
        totalNumSpawns.Min = Mathf.RoundToInt(Mathf.Clamp(spawnBase - spawnOffset, spawnMin, spawnMax));
        totalNumSpawns.Max = Mathf.RoundToInt(Mathf.Clamp(spawnBase + spawnOffset, spawnMin, spawnMax));
    }
    void SetTimeBetweenSpawns(float intensity)
    {
        float timeBase = Mathf.Clamp(timeBetweenMax * (1 - intensity), timeBetweenMin, timeBetweenMax);
        SpawningTime.Min = Mathf.Clamp(timeBase - spawnTimeOffset, timeBetweenMin, timeBetweenMax);
        SpawningTime.Max = Mathf.Clamp(timeBase + spawnTimeOffset, timeBetweenMin, timeBetweenMax);
    }

    public void Reset()
    {
        remainingSpawns = totalNumSpawns.GetRandomValue();
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

