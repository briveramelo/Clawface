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
    private static int systemMaxWaveLimit = 20;
    #endregion

    #region Public Fields
    public bool InfiniteWavesEnabled { get; set; }
    public int CurrentWaveIndex { get; set; }
    public int MaxWaveIndex { get; set; }
    public bool AtMaxWaveLimit { get { return MaxWaveIndex == systemMaxWaveLimit; } }
    public bool OneWaveMax { get { return MaxWaveIndex == 1; } }
    public string CurrentWaveText { get { return string.Format("{0}",CurrentWaveIndex + 1); } }
    public string MaxWaveText { get { return string.Format("{0}", MaxWaveIndex + 1); } }
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
        CurrentWaveIndex++;
        CallWave(CurrentWaveIndex);
    }

    void CallWave(int waveIndex) {
        editorInstance.EnableCurrentWaveSpawnParents(waveIndex);
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(waveIndex);

        for (int i = 0; i < currentWaveSpawners.Count; i++) {
            //for keira
            if (currentWaveSpawners[i] != null) {
                currentWaveSpawners[i].StartSpawning();
            }

        }
    }

    private void StartLevel(params object[] i_params)
    {
        Reset();
        RegisterAllSpawns();
        CallWave(0);
    }

    private void ProcessDeath(params object[] parameters)
    {
        //check if all spawners in given wave are marked as completed
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(CurrentWaveIndex);
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
        CurrentWaveIndex = 0;
    }



    #endregion

    #region Public Interface

    public void RegisterAllSpawns() {

        editorInstance.levelDataManager.SaveSpawns();
        
    }

    public void SetToWave(int i_wave)
    {
        CurrentWaveIndex = Mathf.Clamp(i_wave, 0, MaxWaveIndex);
        editorInstance.EnableCurrentWaveSpawnParents(CurrentWaveIndex);
    }

    public void GoToPreviousWave()
    {
        CurrentWaveIndex--;
        if(CurrentWaveIndex < 0)
        {
            CurrentWaveIndex = MaxWaveIndex;
        }
        SetToWave(CurrentWaveIndex);
    }

    public void GoToNextWave()
    {
        CurrentWaveIndex++;
        if(CurrentWaveIndex > MaxWaveIndex)
            CurrentWaveIndex = 0;
        
        SetToWave(CurrentWaveIndex);
    }

    public void AddWave()
    {
        if (AtMaxWaveLimit)
            return;
        MaxWaveIndex++;
    }

    public void DeleteWave(int i_wave)
    {
        if (MaxWaveIndex == 1)
            return;

        MaxWaveIndex--;
        if(CurrentWaveIndex > MaxWaveIndex)
        {
            CurrentWaveIndex = MaxWaveIndex;
        }
        
    }

    #endregion

}
