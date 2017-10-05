using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MovementEffects;
using System.Linq;
public class Spawner : RoutineRunner
{
    public bool useIntensityCurve, manualEdits;
    public AnimationCurve intensityCurve;
    //    public AnimationCurve timingCurve;

    public IntRangeProperties spawnRange;
    public FloatRangeProperties spawnTimeRange;

    public List<Wave> waves = new List<Wave>();

    public int currentWaveNumber = 0;
    public int currentNumEnemies = 0;
    //    public float TimeToNextWave = 0.0f;

    #region Serialized Unity Fields
    //[SerializeField] SpawnType spawnType;
    #endregion


    #region private variables

    private int currentWave = 0;

    List<Transform> spawnPoints = new List<Transform>();


    private PoolObjectType GetPoolObject(SpawnType spawnType) {
        switch (spawnType) {
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
        //        if(waves.Count > 0) TimeToNextWave = waves[0].Time;

        foreach (Transform child_point in transform)
        {
            spawnPoints.Add(child_point);
        }

        CheckToSpawnEnemyCluster();
    }

    private void Update()
    {

        /*
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
        */
    }


    #endregion

    #region Private Methods
    private void ReportDeath()
    {
        currentNumEnemies--;

        if (currentWave < waves.Count-1 && currentNumEnemies <= waves[currentWave].totalNumSpawns.Min * spawnPoints.Count)
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

    static int waveCount;
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
        int enemiesToSpawn = waves[currentWave].totalNumSpawns.Max;
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(waves[currentWave].RunSpawnSequence(SpawnEnemy), coroutineName));
    }

    void SpawnEnemy(SpawnType spawnType) {        
        GameObject spawnedObject = ObjectPool.Instance.GetObject(GetPoolObject(spawnType));
        
        if (spawnedObject) {
            ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

            if (!spawnable.HasWillBeenWritten()) {
                spawnable.RegisterDeathEvent(ReportDeath);
            }
            Vector3 spawnPosition = spawnPoints.GetRandom().position;
            spawnedObject.transform.position = spawnPosition;
            spawnable.WarpToNavMesh(spawnPosition);

            currentNumEnemies++;            
        }
        else {
            Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECT" + "</color>");
        }        
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
public class Wave
{
    [HideInInspector] public int remainingSpawns;
    [SerializeField, Range(0, 1)] public float intensity;    
    [EditableIntRange] public IntRange totalNumSpawns;
    [EditableFloatRange] public FloatRange spawningTime;
    public EnemySpawnQuantities enemySpawnQuantities;
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

    public void ApplyIntensityValue()
    {
        SetTotalSpawns(intensity);
        SetTimeBetweenSpawns(intensity);
        SetEnemyCounts(intensity);
    }

    

    void SetTotalSpawns(float intensity)
    {
        float spawnBase = totalNumSpawns.minLimit + intensity * totalNumSpawns.Range;
        totalNumSpawns.Min = Mathf.RoundToInt(Mathf.Clamp(spawnBase - spawnOffset, totalNumSpawns.minLimit, totalNumSpawns.maxLimit));
        totalNumSpawns.Max = Mathf.RoundToInt(Mathf.Clamp(spawnBase + spawnOffset, totalNumSpawns.minLimit, totalNumSpawns.maxLimit));
    }
    void SetTimeBetweenSpawns(float intensity)
    {
        float timeBase = Mathf.Clamp(spawningTime.maxLimit * (1 - intensity), spawningTime.Min, spawningTime.Max);
        spawningTime.Min = Mathf.Clamp(timeBase - spawnTimeOffset, spawningTime.minLimit, spawningTime.maxLimit);
        spawningTime.Max = Mathf.Clamp(timeBase + spawnTimeOffset, spawningTime.minLimit, spawningTime.maxLimit);
    }

    //TO DO, set values custom per enemy
    void SetEnemyCounts(float intensity) {
        int maxSpawns = totalNumSpawns.Max;
        enemySpawnQuantities.blaster.spawnCount = maxSpawns;
    }

    public void Reset()
    {
        remainingSpawns = totalNumSpawns.Max;
        enemySpawnQuantities.Reset();        
    }    
    public IEnumerator<float> RunSpawnSequence(System.Action<SpawnType> onSpawn)
    {
        Reset();
        while (true)
        {
            SpawnType type;
            if (enemySpawnQuantities.GetNextAvailableSpawnType(out type)) {
                
                onSpawn(type);
                enemySpawnQuantities.DecrementRemaining(type);
            }
            remainingSpawns--;
            if (remainingSpawns <= 0)
            {
                break;
            }
            yield return Timing.WaitForSeconds(spawningTime.GetRandomValue());
        }
    }
}

[System.Serializable]
public class EnemySpawnQuantities {
    public EnemySpawnQuantity blaster = new EnemySpawnQuantity(SpawnType.Blaster);
    public EnemySpawnQuantity bouncer = new EnemySpawnQuantity(SpawnType.Bouncer);
    public EnemySpawnQuantity kamikaze = new EnemySpawnQuantity(SpawnType.Kamikaze);
    public EnemySpawnQuantity zombie = new EnemySpawnQuantity(SpawnType.Zombie);

    List<EnemySpawnQuantity> quantities = new List<EnemySpawnQuantity>();
    void ResetList() {
        quantities.Clear();
        quantities.Add(blaster);
        quantities.Add(bouncer);
        quantities.Add(kamikaze);
        quantities.Add(zombie);
    }

    public void Reset() {
        ResetList();
        quantities.ForEach(quant=> {
            quant.remainingSpawnCount = quant.spawnCount;
        });
    }

    public void DecrementRemaining(SpawnType type) {
        ResetList();
        quantities.Find(quant => quant.spawnType == type).remainingSpawnCount--;
    }
    public bool GetNextAvailableSpawnType(out SpawnType spawnType) {
        ResetList();
        spawnType = SpawnType.Blaster;
        System.Predicate<EnemySpawnQuantity> anyRemaining = quant => quant.remainingSpawnCount > 0;
        bool exists = quantities.Exists(anyRemaining);
        if (exists) {
            EnemySpawnQuantity availableType = quantities.Find(anyRemaining);
            spawnType = availableType.spawnType;
        }
        return exists;
    }
}

[System.Serializable]
public class EnemySpawnQuantity {
    public SpawnType spawnType;
    public int spawnCount;
    [HideInInspector] public int remainingSpawnCount;
    public EnemySpawnQuantity(SpawnType type) {
        this.spawnType = type;
    }
}

[System.Serializable]
public class IntRangeProperties {
    public int min;
    public int max;
    public int rangeSize;
}

[System.Serializable]
public class FloatRangeProperties {
    public float min;
    public float max;
    public float rangeSize;
}