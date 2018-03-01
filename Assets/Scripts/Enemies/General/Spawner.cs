using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MEC;
using System.Linq;
using Turing.VFX;

[System.Serializable]
public class Spawner : RoutineRunner
{
    public bool useIntensityCurve, manualEdits;
    public AnimationCurve intensityCurve;

    public List<Wave> waves = new List<Wave>();

    public int currentWaveNumber = 0;
    public int currentNumEnemies = 0;
    public int totalNumEnemies = 0;

    private float spawnHeightOffset = 50.0f;


    #region private variables

    private int currentWave = 0;

    List<Transform> spawnPoints = new List<Transform>();

    #endregion

    #region Unity LifeCycle

    void Init()
    {
        totalNumEnemies = 0;
        currentWave = 0;
        foreach (Wave w in waves)
        {
            foreach(WaveType type in w.monsterList)
            {
                totalNumEnemies += type.Count;
            }
        }
    }
    
    #endregion

    public void Activate()
    {
        foreach (Transform child_point in transform)
        {
            spawnPoints.Add(child_point);
        }
        Init();
        CheckToSpawnEnemyCluster();
    }


    #region Private Methods
    private void ReportDeath()
    {
        currentNumEnemies--;
        totalNumEnemies--;

        if (currentWave < waves.Count - 1 && currentNumEnemies <= waves[currentWave].NumToNextWave)
        {
            GoToNextWave();
        }

        if(totalNumEnemies == 0)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXT_WAVE);
        }
    }

    private void GoToNextWave()
    {
        waves[currentWave].FirePostEvents();
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
                waves[currentWave].FirePreEvents();
                StartCoroutine(SpawnEnemyCluster());
            }
        }
    }

    private IEnumerator SpawnEnemyCluster()
    {
        for(int i = 0; i < waves[currentWave].monsterList.Count; i++)
        {
            for(int j = 0; j < waves[currentWave].monsterList[i].Count; j++)
            {
                float spawnDelay = waves[currentWave].spawnEffectTime;
                GameObject spawnEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXEnemySpawn);
                Vector3 spawnPosition = spawnPoints.GetRandom().position;
                if (spawnEffect) {
                    spawnEffect.GetComponent<VFXOneOff>().Play(spawnDelay);
                    spawnEffect.transform.position = spawnPosition;
                }
                PoolObjectType enemy = waves[currentWave].monsterList[i].Type.ToPoolObject();
                SpawnEnemy(spawnPosition, enemy);
                yield return new WaitForSeconds(1.0f);
                //Timing.RunCoroutine(DelayAction(() => SpawnEnemy(spawnPosition, enemy), spawnDelay), coroutineName);
            }
        }        

    }

    void SpawnEnemy(Vector3 spawnPosition, PoolObjectType enemy) {
        spawnPosition.y += spawnHeightOffset;
        GameObject spawnedObject = ObjectPool.Instance.GetObject(enemy);
        spawnedObject.transform.position = spawnPosition;
        //spawnedObject.SetActive(false);

        if (spawnedObject) {
            ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

            if (!spawnable.HasWillBeenWritten()) {
                spawnable.RegisterDeathEvent(ReportDeath);
            }

            EnemyBase enemyBase = spawnedObject.GetComponent<EnemyBase>();
            if (enemyBase)
            {
                enemyBase.SpawnWithRagdoll(spawnPosition);                
            }
            
            EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_SPAWNED, spawnedObject);

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
        return totalNumEnemies == 0 ? true : false;
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

    #endregion

    public List<int> spawnedHashCodes = new List<int>();

    public List<WaveType> monsterList;

    [HideInInspector] public int remainingSpawns;
    public float spawnEffectTime=1.5f;
    [SerializeField, Range(0, 1)] public float intensity;
    [SerializeField, Range(0, 10)] public int NumToNextWave;



    [SerializeField]
    private List<string> preEventNames;
    [SerializeField]
    private List<string> postEventNames;


    public int spawnOffset;
    public float spawnTimeOffset;

    public float Intensity
    {
        get { return intensity; }
        set
        {            
            intensity = Mathf.Clamp01(value);
        }
    }

    public FloatRange SpawningTime;
    

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

    public void AddPreEvent(string eventName)
    {
        AddEvent(preEventNames, eventName);
    }

    public void AddPostEvent(string eventName)
    {
        AddEvent(postEventNames, eventName);
    }

    private void AddEvent(List<string> eventNames, string eventName)
    {
        if (eventNames == null)
        {
            eventNames = new List<string>();
        }
        if (!eventNames.Contains(eventName))
        {
            eventNames.Add(eventName);
        }
    }

    public void ClearEvents()
    {
        if (postEventNames != null)
        {
            postEventNames.Clear();
        }

        if (preEventNames != null)
        {
            preEventNames.Clear();
        }
    }

    public void FirePreEvents()
    {
        FireEvents(preEventNames);
    }

    public void FirePostEvents()
    {
        FireEvents(postEventNames);
    }

    private void FireEvents(List<string> eventNames)
    {
        bool shouldShowColor = true;
        if (eventNames != null)
        {
            for (int i = 0; i < eventNames.Count; i++)
            {
                EventSystem.Instance.TriggerEvent(eventNames[i], shouldShowColor);
            }
        }
    }


}