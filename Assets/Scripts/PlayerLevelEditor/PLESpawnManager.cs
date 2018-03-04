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
    public int CurrentWave { get; set; }
    public int MaxWave { get; set; }
    public bool AtMaxWaveLimit { get { return MaxWave == systemMaxWaveLimit; } private set { } }
    public bool OneWaveRemaining { get { return MaxWave == 1; } private set { } }
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
        CurrentWave++;
        editorInstance.EnableSpawnsOnWaveChange(CurrentWave);
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(CurrentWave);
        
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
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(CurrentWave);
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
        CurrentWave = -1;
    }



    #endregion

    #region Public Interface

    public void RegisterAllSpawns() {

        editorInstance.levelDataManager.SaveSpawns();
        
    }

    public void SetToWave(int i_wave)
    {
        CurrentWave = Mathf.Clamp(i_wave, 0, MaxWave - 1);
        CurrentWave = i_wave;
        editorInstance.EnableSpawnsOnWaveChange(CurrentWave);
    }

    public void GoToPreviousWave()
    {
        CurrentWave--;
        if(CurrentWave < 0)
        {
            CurrentWave = MaxWave - 1;
        }
        SetToWave(CurrentWave);
    }

    public void GoToNextWave()
    {
        CurrentWave++;
        if(CurrentWave >= MaxWave)
            CurrentWave = 0;
        
        SetToWave(CurrentWave);

    }

    public void AddWave()
    {
        if (AtMaxWaveLimit)
            return;
        MaxWave++;
    }

    public void DeleteWave(int i_wave)
    {
        if (MaxWave == 1)
            return;
        MaxWave--;
        if(CurrentWave >= MaxWave)
        {
            CurrentWave = MaxWave - 1;
        }
        
    }

    #endregion

}
