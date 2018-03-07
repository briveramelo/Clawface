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
    private static int systemMaxWaveIndex = 19;
    #endregion

    #region Public Fields
    public bool InfiniteWavesEnabled { get; set; }
    public int CurrentWaveIndex { get; set; }
    public int MaxWaveIndex { get; set; }
    public bool AtMaxWaveLimit { get { return MaxWaveIndex == systemMaxWaveIndex; } }
    public bool AtMinWaveLimit { get { return MaxWaveIndex == 0; } }
    public string CurrentWaveText { get { return string.Format("{0}",CurrentWaveIndex + 1); } }
    public string MaxWaveText { get { return string.Format("{0}", MaxWaveIndex + 1); } }
    #endregion

    #region Serialized Unity Fields

    [SerializeField] private LevelEditor editorInstance;

    #endregion


    #region Unity Lifecycle

    void Start()
    {        
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, StartLevelDelayed);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, Reset);
    }

    public void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, StartLevelDelayed);
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
        CurrentWaveIndex = waveIndex;
        editorInstance.EnableCurrentWaveSpawnParents(waveIndex);
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(waveIndex);

        for (int i = 0; i < currentWaveSpawners.Count; i++) {
            PLESpawn spawn = currentWaveSpawners[i];
            spawn.SetOnAllEnemiesDead(OnAllSpawnsInSpawnerDead);
            spawn.StartSpawning();
        }
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, waveIndex);
    }

    private void StartLevelDelayed(params object[] i_params)
    {
        StartCoroutine(WaitForDataToLoad());        
    }

    IEnumerator WaitForDataToLoad() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        RegisterAllSpawns();
        Reset();
        editorInstance.ToggleCameraGameObject(false);
        CallWave(0);
    }

    private void OnAllSpawnsInSpawnerDead()
    {
        //check if all spawners in given wave are marked as completed
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(CurrentWaveIndex);
        bool waveDead = true;

        for (int i = 0; i < currentWaveSpawners.Count; i++)
        {
            PLESpawn currentSpawn = currentWaveSpawners[i];
            if (!currentSpawn.allEnemiesDead && currentSpawn.spawnType!=SpawnType.Keira)
            {
                
                waveDead = false;
                break;
            }
        }

        if(waveDead)
        {
            if (CurrentWaveIndex >= ActiveLevelData.WaveCount - 1 && !InfiniteWavesEnabled)
            {
                if (editorInstance.GetIsTesting())
                {
                    EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_COMPLETED);
                }
                
            }
            else if (CurrentWaveIndex >= ActiveLevelData.WaveCount - 1 && InfiniteWavesEnabled)
            {
                CallWave(0);
            }
            else
            {
                CallNextWave();
            }
        }

    }
    private void Reset(params object[] parameters)
    {
        List<PLESpawn> currentWaveSpawners = ActiveLevelData.GetPLESpawnsFromWave(CurrentWaveIndex);
        for (int i = 0; i < currentWaveSpawners.Count; i++)
        {
            PLESpawn spawn = currentWaveSpawners[i];
            spawn.SetOnAllEnemiesDead(null);
         
        }
        FindObjectsOfType<EnemyBase>().ToList().ForEach(enemy => { enemy.OnDeath(); });
        CurrentWaveIndex = 0;
    }



    #endregion

    #region Public Interface

    public void RegisterAllSpawns() {
        editorInstance.levelDataManager.SaveSpawns();
    }

    public int SetToWave(int i_wave)
    {
        CurrentWaveIndex = Mathf.Clamp(i_wave, 0, MaxWaveIndex);
        editorInstance.EnableCurrentWaveSpawnParents(CurrentWaveIndex);
        return CurrentWaveIndex;
    }

    public int GoToPreviousWave()
    {
        CurrentWaveIndex--;
        if(CurrentWaveIndex < 0) {
            CurrentWaveIndex = MaxWaveIndex;
        }
        return SetToWave(CurrentWaveIndex);
    }

    public int GoToNextWave()
    {
        CurrentWaveIndex++;
        if (CurrentWaveIndex > MaxWaveIndex) {
            CurrentWaveIndex = 0;                
        }
        return SetToWave(CurrentWaveIndex);
    }

    public void TryAddWave()
    {
        if (AtMaxWaveLimit)
            return;
        MaxWaveIndex++;
    }

    public int TryDeleteWave(int i_wave)
    {
        if (MaxWaveIndex == 0) {
            return CurrentWaveIndex;
        }

        MaxWaveIndex--;
        if(CurrentWaveIndex > MaxWaveIndex) {
            CurrentWaveIndex = MaxWaveIndex;
        }
        return CurrentWaveIndex;
    }

    #endregion

}
