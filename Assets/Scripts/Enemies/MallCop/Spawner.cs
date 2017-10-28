﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ModMan;
using MovementEffects;
using System.Linq;

[System.Serializable]
public class Spawner : RoutineRunner
{
    public bool useIntensityCurve, manualEdits;
    public AnimationCurve intensityCurve;

    public List<Wave> waves = new List<Wave>();

    public int currentWaveNumber = 0;
    public int currentNumEnemies = 0;
    public int totalNumEnemies = 0;


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
        foreach(Wave w in waves)
        {
            foreach(WaveType type in w.monsterList)
            {
                totalNumEnemies += type.Count;
            }
        }
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
        totalNumEnemies--;

        if (currentWave < waves.Count - 1 && currentNumEnemies <= waves[currentWave].NumToNextWave)
        {
            GoToNextWave();
        }

        if(totalNumEnemies == 0)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXTWAVEENEMIES);
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
        if (eventNames != null)
        {
            for (int i = 0; i < eventNames.Count; i++)
            {
                EventSystem.Instance.TriggerEvent(eventNames[i]);
            }
        }
    }


}