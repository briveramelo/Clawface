using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;
using PlayerLevelEditor;

public class PLESpawnManager : Singleton<PLESpawnManager> {

    #region Private Fields
    
    private LevelData ActiveLevelData { get { return DataPersister.ActiveDataSave.ActiveLevelData; } }
    private List<WaveData> ActiveWaveData { get { return ActiveLevelData.waveData; } }
    private int currentWave = -1;

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private LevelEditor editorInstance;

    #endregion


    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        if (EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextWave);
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, StartLevel);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, Reset);

        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, StartLevel);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, Reset);
        }
    }


    #endregion

    #region Private Interface
    private void CallNextWave(params object[] i_params)
    {
        currentWave++;
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(currentWave);
        
        for(int i = 0; i < currentWaveSpawners.Count; i++)
        {
            //for keira
            if(currentWaveSpawners[i] != null)
            {
                currentWaveSpawners[i].StartSpawning();
            }
            
        }
    }

    private void StartLevel(params object[] i_params)
    {
        Reset();
        RegisterAllSpawns();
        CallNextWave();
    }

    private void ProcessDeath(params object[] parameters)
    {
        //check if all spawners in given wave are marked as completed
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(currentWave);
        bool waveDead = true;

        for (int i = 0; i < currentWaveSpawners.Count; i++)
        {
            PLESpawn currentSpawn = currentWaveSpawners[i];
            if (!currentSpawn.allEnemiesDead)
            {
                waveDead = false;
                break;
            }
        }

        if(waveDead)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.CALL_NEXT_WAVE);
        }

    }
    private void Reset(params object[] parameters)
    {
        //Timing.KillCoroutines(coroutineName);
        FindObjectsOfType<EnemyBase>().ToList().ForEach(enemy => { enemy.OnDeath(); });
        currentWave = -1;
    }



    #endregion

    #region Public Interface

    public void RegisterAllSpawns() {

        editorInstance.levelDataManager.SaveSpawns();
        
    }

    #endregion

}
