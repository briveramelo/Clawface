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
    public List<TestWave> testwaves = new List<TestWave>();

    #region Serialized Unity Fields
    [SerializeField] SpawnType spawnType;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<Wave> waves;
    [SerializeField] int currentWaveNumber=0;
    [SerializeField] int currentNumEnemies=0;

    private int currentWave = 0;

    #endregion




    #region private variables
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
    void Start() {
        CheckToSpawnEnemyCluster();
    }
    #endregion

    #region Private Methods
    private void ReportDeath()
    {
        currentNumEnemies--;
        CheckToSpawnEnemyCluster();
    }

    private void CheckToSpawnEnemyCluster()
    { 
        if (Application.isPlaying)
        {
            if (currentWave < waves.Count && currentNumEnemies < waves[currentWave].min)
            {
                Timing.RunCoroutine(SpawnEnemyCluster());
            }
        }
    }

    private IEnumerator<float> SpawnEnemyCluster()
    {
        int enemiesToSpawn = currentWave < waves.Count ? waves[currentWave].max - currentNumEnemies : 0;
        currentNumEnemies += enemiesToSpawn;

        for (int i = 0;  i < enemiesToSpawn; i++)
        {
            yield return Timing.WaitForSeconds(Random.Range(1f, 2f));
            GameObject spawnedObject = ObjectPool.Instance.GetObject(objectToSpawn);

            if (spawnedObject)
            {
                ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                if (!spawnable.HasWillBeenWritten())
                {
                    spawnable.RegisterDeathEvent(ReportDeath);
                }

                if (!spawnPoints.Any(sp=>sp==null))
                {
                    spawnedObject.transform.position = spawnPoints.GetRandom().position;                
                }
            }
        }

        currentWave++;
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

    #region Internal Structures
    [System.Serializable]
    struct Wave
    {
        public int max;
        public int min;
        public Wave(int max, int min)
        {
            this.max = max;
            this.min = min;
        }
    }
    #endregion
}



[System.Serializable]
public class TestWave
{
    const int spawnMin = 1;
    const int spawnMax = 15;
    const float timeBetweenMin = .25f;
    const float timeBetweenMax = 5;

    const int spawnOffset = 1;
    const float spawnTimeOffset = .3f;

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
    }

    [IntRange(spawnMin, spawnMax)] public IntRange totalNumSpawns;
    [FloatRange(timeBetweenMin, timeBetweenMax)] public FloatRange timeBetweenSpawns_sec;

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

