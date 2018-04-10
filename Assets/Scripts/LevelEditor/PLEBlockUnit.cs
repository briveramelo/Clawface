using ModMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLEBlockUnit : EventSubscriber
{
    #region Public Fields

    #endregion

    #region Private Fields

    [SerializeField] public Transform spawnTrans;
    [SerializeField] private LevelUnit levelUnit;

    public List<LevelUnitStates> levelStates = new List<LevelUnitStates>();
    public Color riseColor;
    public int TileType { get { return TileColors.GetType(riseColor); } }
    [HideInInspector] public GameObject prop;
    [HideInInspector] public List<GameObject> spawns;
    #endregion
    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.EnableDisable; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.PLE_ADD_WAVE, AddNewWave},
                { Strings.Events.PLE_DELETE_CURRENTWAVE, DeleteCurrentWave},
                { Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES, SyncTileStatesAndColors},
            };
        }
    }
    #endregion
    #region Unity Lifecycle

    new IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (levelStates.Count == 0) {
            for (int i = 0; i <= PLESpawnManager.Instance.MaxWaveIndex; i++) {
                levelStates.Add(LevelUnitStates.Floor);
            }
        }        
    }

    #endregion

    #region Private Interface    


    #endregion



    #region Public Interface
    public void SetProp(GameObject prop) {
        this.prop = prop;
    }
    public void AddSpawn(GameObject spawn) {
        if (!spawns.Contains(spawn)) {
            spawns.Add(spawn);
        }
    }
    public void RemoveSpawn(GameObject spawn) {
        spawns.Remove(spawn);
    }
    public bool HasActiveSpawn {
        get {
            CleanNullSpawns();
            return spawns.Exists(spawn => { return spawn.activeInHierarchy; });
        }
    }
    void CleanNullSpawns() {
        for (int i = spawns.Count - 1; i >= 0; i--) {
            if (spawns[i] == null) {
                spawns.RemoveAt(i);
            }
        }
    }
    public void ClearItems() {
        transform.position = transform.position.NoY();
        levelStates.Clear();
        
        for (int i = 0; i <= PLESpawnManager.Instance.MaxWaveIndex; i++) {
            levelStates.Add(LevelUnitStates.Floor);
        }
        riseColor = TileColors.Green;

        for (int i = spawns.Count - 1; i >= 0; i--) {
            Helpers.DestroyProper(spawns[i]);
        }
        spawns.Clear();

        if (prop!=null) {
            Helpers.DestroyProper(prop);
        }
        SyncTileStatesAndColors();
    }

    public bool IsOccupied { get { return HasActiveSpawn || prop!=null; } }

    public List<LevelUnitStates> GetLevelStates()
    {
        return levelStates;
    }
    public LevelUnitStates GetStateAtWave(int waveIndex) {
        return levelStates[waveIndex];
    }
    public bool IsFlatAtWave(int waveIndex) {
        return waveIndex < levelStates.Count && GetStateAtWave(waveIndex) == LevelUnitStates.Floor;
    }

    public void SetLevelStates(List<LevelUnitStates> newLevelStates, Color riseColor) {
        levelStates.Clear();
        newLevelStates.ForEach(state => {
            levelStates.Add(state);
        });
        this.riseColor = riseColor;
        SyncTileStatesAndColors();
    }


    public void AddNewWave(params object[] parameters)
    {
        LevelUnitStates newState = LevelUnitStates.Floor;
        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        if (levelStates.Count > currentWaveIndex) {
            newState = levelStates[currentWaveIndex];
        }
        levelStates.Insert(currentWaveIndex, newState);
        SyncTileStatesAndColors();
    }

    public void DeleteCurrentWave(params object[] parameters)
    {
        levelStates.RemoveAt(PLESpawnManager.Instance.CurrentWaveIndex);
        SyncTileStatesAndColors();
    }

    public void SyncTileStatesAndColors(params object[] parameters)
    {
        levelUnit.SetLevelUnitStates(levelStates, riseColor);
    }

    #endregion
}
