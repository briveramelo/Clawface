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
    public List<Wave> waves = new List<Wave>();

    public int currentWaveNumber = 0;
    public int currentNumEnemies = 0;

    #region Serialized Unity Fields
    [SerializeField] SpawnType spawnType;
    #endregion


    #region private variables

    private int currentWave = 0;
    private float NextWaveTime = 10.0f;
    private float currentWaveTime = 0.0f;

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
            return PoolObjectType.MallCopSwinger;
        }
    }

    #endregion

    #region Unity LifeCycle
    void Start()
    {
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        CheckToSpawnEnemyCluster();
    }

    private void Update()
    {
        /*
        currentWaveTime += Time.deltaTime;

        if(currentWaveTime > NextWaveTime)
        {
            GoToNextWave();
            currentWaveTime = 0.0f;
        }
        */
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
            yield return Timing.WaitForSeconds(Random.Range(1f, 2f));

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
    const float timeBetweenMin = .25f;
    const float timeBetweenMax = 5;

    const int spawnOffset = 1;
    const float spawnTimeOffset = .3f;

    const float TimeToNextWaveMin = 30.0f;
    const float TimeToNextWaveMax = 60.0f;

    #endregion

    public List<int> spawnedHashCodes = new List<int>();

    [HideInInspector] public int remainingSpawns;
    [SerializeField, Range(0, 1)] float intensity;

    public float Intensity
    {
        get { return intensity; }
        set
        {
            intensity = value;
            ApplyIntensityValue();
        }
    }
    public void ApplyIntensityValue()
    {
        SetTotalSpawns(intensity);
        SetTimeBetweenSpawns(intensity);
        SetNextWaveTime(intensity);
    }

    [IntRange(spawnMin, spawnMax)] public IntRange totalNumSpawns;
    [FloatRange(timeBetweenMin, timeBetweenMax)] public FloatRange timeBetweenSpawns_sec;
    [FloatRange(TimeToNextWaveMin, TimeToNextWaveMax)] public FloatRange NextWaveTime;

    void SetTotalSpawns(float intensity)
    {
        float spawnBase = intensity * spawnMax;
        totalNumSpawns.Min = Mathf.RoundToInt(Mathf.Clamp(spawnBase - spawnOffset, spawnMin, spawnMax));
        totalNumSpawns.Max = Mathf.RoundToInt(Mathf.Clamp(spawnBase + spawnOffset, spawnMin, spawnMax));
    }
    void SetTimeBetweenSpawns(float intensity)
    {
        float timeBase = Mathf.Clamp(timeBetweenMax * (1 - intensity), timeBetweenMin, timeBetweenMax);
        timeBetweenSpawns_sec.Min = Mathf.Clamp(timeBase - spawnTimeOffset, timeBetweenMin, timeBetweenMax);
        timeBetweenSpawns_sec.Max = Mathf.Clamp(timeBase + spawnTimeOffset, timeBetweenMin, timeBetweenMax);
    }

    void SetNextWaveTime(float intensity)
    {
        float timeBase = intensity * TimeToNextWaveMax;
        NextWaveTime.Min = Mathf.Clamp(timeBase - 10, TimeToNextWaveMin, TimeToNextWaveMax);
        NextWaveTime.Max = Mathf.Clamp(timeBase + 10, TimeToNextWaveMin, TimeToNextWaveMax);
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
            yield return Timing.WaitForSeconds(timeBetweenSpawns_sec.GetRandomValue());
        }
    }
}

