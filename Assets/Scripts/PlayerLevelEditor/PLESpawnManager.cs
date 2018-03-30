using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;
using PlayerLevelEditor;

public class PLESpawnManager : Singleton<PLESpawnManager> {

    #region Private Fields
    
    private LevelData WorkingLevelData { get { return DataPersister.ActiveDataSave.workingLevelData; } }
    private List<WaveData> ActiveWaveData { get { return WorkingLevelData.waveData; } }
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

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.PLE_ON_LEVEL_READY, TryStartLevel },
                { Strings.Events.PLE_TEST_END, Reset},
            };
        }
    }
    #endregion

    private bool hasCycledLevel=false;

    #region Unity Lifecycle
    #endregion

    #region Private Interface
    private void TryStartLevel(params object[] parameters) {
        if (SceneTracker.IsCurrentScenePlayerLevels || editorInstance.IsTesting) {
            SyncLevelData();
            Reset();
            editorInstance.ToggleCameraGameObject(false);//should match up in same frame where keira's camera comes on
            CallWave(0);
        }
    }

    private void CallNextWave(params object[] i_params)
    {
        CurrentWaveIndex++;
        CallWave(CurrentWaveIndex);
    }

    private void CallWave(int waveIndex) {
        CurrentWaveIndex = waveIndex;
        editorInstance.EnableCurrentWaveSpawnParents(waveIndex);
        List<PLESpawn> currentWaveSpawners = WorkingLevelData.GetPLESpawnsFromWave(waveIndex);

        for (int i = 0; i < currentWaveSpawners.Count; i++) {
            PLESpawn spawn = currentWaveSpawners[i];
            spawn.SetOnCriticalEnemiesDead(OnCriticalSpawnsInSpawnerDead);
            spawn.StartSpawning();
        }
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, waveIndex);
    }    

    private void OnCriticalSpawnsInSpawnerDead()
    {
        //check if all spawners in given wave are marked as completed
        List<PLESpawn> currentWaveSpawners = WorkingLevelData.GetPLESpawnsFromWave(CurrentWaveIndex);
        bool lastWaveIsComplete = true;
        if (CurrentWaveIndex>0 || (CurrentWaveIndex == 0 && InfiniteWavesEnabled && hasCycledLevel)) {
            int lastWaveIndex = CurrentWaveIndex-1;
            if (lastWaveIndex<0) {
                lastWaveIndex = MaxWaveIndex;
            }
            List<PLESpawn> lastWaveSpawners = WorkingLevelData.GetPLESpawnsFromWave(lastWaveIndex);
            lastWaveIsComplete= lastWaveSpawners.All(spawner => { return spawner.AllEnemiesDead; });
        }
        bool currentWaveMinEnemiesAreDead = currentWaveSpawners.All(spawner => { return spawner.MinEnemiesDead; });

        if (lastWaveIsComplete && currentWaveMinEnemiesAreDead) {
            if (CurrentWaveIndex >= WorkingLevelData.WaveCount - 1 && !InfiniteWavesEnabled) {
                EventSystem.Instance.TriggerEvent(
                    Strings.Events.LEVEL_COMPLETED, SceneTracker.CurrentSceneName, 
                    ScoreManager.Instance.GetScore(), ModManager.leftArmOnLoad.ToString(),
                    ModManager.rightArmOnLoad.ToString());
            }
            else if (CurrentWaveIndex >= WorkingLevelData.WaveCount - 1 && InfiniteWavesEnabled) {
                hasCycledLevel = true;
                CallWave(0);
            }
            else {
                CallNextWave();
            }
        }
    }

    private void Reset(params object[] parameters)
    {
        List<PLESpawn> currentWaveSpawners = WorkingLevelData.GetPLESpawnsFromWave(CurrentWaveIndex);
        for (int i = 0; i < currentWaveSpawners.Count; i++)
        {
            PLESpawn spawn = currentWaveSpawners[i];
            spawn.SetOnCriticalEnemiesDead(null);
        }
        FindObjectsOfType<EnemyBase>().ToList().ForEach(enemy => { enemy.OnDeath(); });
        CurrentWaveIndex = 0;
        hasCycledLevel = false;
    }
    


    #endregion

    #region Public Interface

    public void SyncLevelData() {
        editorInstance.levelDataManager.SyncWorkingSpawnData();
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
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
