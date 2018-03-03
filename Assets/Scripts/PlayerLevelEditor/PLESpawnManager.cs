using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class PLESpawnManager : Singleton<PLESpawnManager> {

    #region Private Fields

    List<List<PLESpawn>> spawners = new List<List<PLESpawn>>();
    private int currentWave = -1;
    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextWave);
            //EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, CallNextWave);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, Reset);

        }
    }


    private void OnDisable()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextWave);
            //EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, CallNextWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, Reset);
        }
    }

    #endregion

    #region Private Interface
    private void CallNextWave(object[] parameters)
    {
        RemoveInvalidWaveData();

        currentWave++;
        List<PLESpawn> currentWaveSpawners = spawners[currentWave];

        //activate spawners
        for(int i = 0; i < currentWaveSpawners.Count; i++)
        {
            PLESpawn currentSpawn = currentWaveSpawners[i];

            //start spawning
            currentSpawn.StartSpawning();
        }
    }

    private void RemoveInvalidWaveData()
    {
        for(int currentWaveIndex = 0; currentWaveIndex < spawners.Count; currentWaveIndex++)
        {
            List<PLESpawn> currentWave = spawners[currentWaveIndex];

            for(int currentSpawnIndex = currentWave.Count - 1; currentSpawnIndex >= 0; currentSpawnIndex--)
            {
                PLESpawn currentSpawnUnit = currentWave[currentSpawnIndex];

                if(currentSpawnUnit == null)
                {
                    currentWave.RemoveAt(currentSpawnIndex);
                }
            }
        }
    }

    private void ProcessDeath(object[] parameters)
    {
        //check if all spawners in given wave are marked as completed
        List<PLESpawn> currentSpawnersInWave = spawners[currentWave];
        bool waveDead = true;

        for (int i = 0; i < currentSpawnersInWave.Count; i++)
        {
            PLESpawn currentSpawn = currentSpawnersInWave[i];
            if (!currentSpawn.allEnemiesDead)
            {
                waveDead = false;
                break;
            }
        }

        if(waveDead)
        {
            Debug.Log("All enemies are dead in wave " + currentWave);
            EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXT_WAVE);
        }

    }
    private void Reset(object[] parameters)
    {
        //Timing.KillCoroutines(coroutineName);
        FindObjectsOfType<EnemyBase>().ToList().ForEach(enemy => { enemy.OnDeath(); });
    }

    #endregion

    #region Public Interface

    public void RegisterSpawner(int i_wave, PLESpawn i_spawn)
    {
        List<PLESpawn> currentSpawnersInWave = new List<PLESpawn>();
        
        //new wave?
        if(i_wave >= spawners.Count)
        {
            spawners.Add(currentSpawnersInWave);
        }
        else
        {
            currentSpawnersInWave = spawners[i_wave];
        }

        currentSpawnersInWave.Add(i_spawn);
    }

    #endregion

}
