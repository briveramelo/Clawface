using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;
using PLE;

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
    private int infiniteWaveIndex;
    private bool hasCycledLevel=false;
    private List<PLESpawn> allSpawnTypes = new List<PLESpawn>();

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

    private void CallNextWave()
    {
        CurrentWaveIndex++;
        infiniteWaveIndex++;
        CallWave(CurrentWaveIndex);

        GameObject fireworks = ObjectPool.Instance.GetObject(PoolObjectType.VFXFireworks);
        if (fireworks) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                fireworks.transform.position = player.transform.position;
        }
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
        EventSystem.Instance.TriggerEvent(Strings.Events.WAVE_COMPLETE, infiniteWaveIndex);
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
                infiniteWaveIndex++;
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
        infiniteWaveIndex = 0;
        hasCycledLevel = false;
    }



    #endregion

    #region Public Interface
    public void SetSpawnTypes(List<PLESpawn> spawnTypes) {
        allSpawnTypes.Clear();
        spawnTypes.ForEach(type => { allSpawnTypes.Add(type); });
    }
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

    public int NumberSpawnsInCurrentWave(SpawnType type) {
        return NumberSpawnsInWave(type, CurrentWaveIndex);
    }
    public int NumberSpawnsInWave(SpawnType type, int waveIndex) {
        waveIndex = Mathf.Clamp(waveIndex, 0, MaxWaveIndex);
        return WorkingLevelData.NumSpawns(type, waveIndex);
    }
    public int GetMaxSpawnsAllowedInCurrentWave(PLESpawn spawn) {
        return GetMaxSpawnsAllowedInWave(spawn, CurrentWaveIndex);
    }
    public int GetMaxSpawnsAllowedInWave(PLESpawn spawn, int waveIndex) {
        waveIndex = Mathf.Clamp(waveIndex, 0, MaxWaveIndex);
        int previousWaveIndex = waveIndex - 1;
        int previousWaveMinSpawnCount = 0;
        if (previousWaveIndex < 0 && InfiniteWavesEnabled) {
            previousWaveIndex = MaxWaveIndex;
        }
        if (previousWaveIndex >= 0) {
            previousWaveMinSpawnCount = WorkingLevelData.MinNumSpawns(spawn.spawnType, previousWaveIndex);
        }
        int max = spawn.MaxPerWave - previousWaveMinSpawnCount;
        return Mathf.Clamp(max, 0, spawn.MaxPerWave);
    }
    public int GetNumberSpawnsInNextWave(PLESpawn spawn) {
        int numSpawnsInNextWave = 0;
        int currentIndex = CurrentWaveIndex;
        int nextIndex = currentIndex + 1;
        if (nextIndex > MaxWaveIndex && InfiniteWavesEnabled) {
            nextIndex = 0;
        }
        if (nextIndex <= MaxWaveIndex) {
            numSpawnsInNextWave = WorkingLevelData.NumSpawns(spawn.spawnType, nextIndex);
        }
        return numSpawnsInNextWave;
    }
    public bool SpawnsUnderMaximum(PLESpawn spawn) {
        return NumberSpawnsInCurrentWave(spawn.spawnType) < GetMaxSpawnsAllowedInCurrentWave(spawn);
    }
    public bool Wave0SpawnsAllowForWrapping() {
        return allSpawnTypes.All(spawn => {
            int numSpawnsInWave0 = NumberSpawnsInWave(spawn.spawnType, 0);
            int maxSpawnsAllowedInWave0 = GetMaxSpawnsAllowedInWave(spawn, 0);
            return numSpawnsInWave0 <= maxSpawnsAllowedInWave0;
        });
    }

    public int GetMaxMinSpawnsAllowedCurrentInWave(PLESpawn spawn) {
        return
            Mathf.Min(spawn.MaxPerWave - GetNumberSpawnsInNextWave(spawn),
            WorkingLevelData.NumSpawns(spawn.spawnType, CurrentWaveIndex));
    }
    public int GetRequiredKillCountInCurrentWave(PLESpawn spawn) {
        int maxMinSpawnsAllowed = GetMaxMinSpawnsAllowedCurrentInWave(spawn);
        if (CurrentWaveIndex == MaxWaveIndex && !InfiniteWavesEnabled) {
            return maxMinSpawnsAllowed;
        }
        return Mathf.Clamp(maxMinSpawnsAllowed - spawn.MinSpawns, 0, maxMinSpawnsAllowed);
    }
    public int RemainingSpawnsAllowedInCurrentWave(PLESpawn spawn) {
        return GetMaxSpawnsAllowedInCurrentWave(spawn) - NumberSpawnsInCurrentWave(spawn.spawnType);
    }
    #endregion

}
