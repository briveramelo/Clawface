using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLESpawnManager : Singleton<PLESpawnManager> {

    #region Private Fields

    List<List<PLESpawn>> spawners = new List<List<PLESpawn>>();
    private int currentWave = 0;
    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextWave);
        }
    }

    private void OnDisable()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextWave);
        }
    }

    #endregion

    #region Private Interface
    private void CallNextWave(object[] parameters)
    {
        List<PLESpawn> currentWaveSpawners = spawners[currentWave];

        //activate spawners
        for(int i = 0; i < currentWaveSpawners.Count; i++)
        {
            PLESpawn currentSpawn = currentWaveSpawners[i];

            //start spawning
            currentSpawn.StartSpawning();
        }
        currentWave++;
    }

    private void CreateWave()
    {

    }

    #endregion



    public void RegisterSpawner(int i_wave, PLESpawn i_spawn)
    {
        if(i_wave >= spawners.Count)
        {
            spawners.Add(new List<PLESpawn>());
        }
        List<PLESpawn> spawnersInWave = spawners[i_wave];

        spawnersInWave.Add(i_spawn);
    }


}
