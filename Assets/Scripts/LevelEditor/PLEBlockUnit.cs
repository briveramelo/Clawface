using ModMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLEBlockUnit : MonoBehaviour
{
    #region Public Fields

    #endregion

    #region Private Fields

    private bool occupied;
    [SerializeField] private int blockID = 0;
    [SerializeField] public Transform spawnTrans;
    [SerializeField] private LevelUnit levelUnit;

    public List<LevelUnitStates> levelStates = new List<LevelUnitStates>();
    [HideInInspector] public GameObject prop;
    [HideInInspector] public List<GameObject> spawns;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        occupied = false;
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (levelStates.Count == 0) {
            for (int i = 0; i <= PLESpawnManager.Instance.MaxWaveIndex; i++) {
                levelStates.Add(LevelUnitStates.Floor);
            }
        }
    }

    private void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_ADD_WAVE, AddNewWave);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_DELETE_CURRENTWAVE, DeleteCurrentWave);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES, SyncTileHeightStates);
    }

    private void OnDisable()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_ADD_WAVE, AddNewWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_DELETE_CURRENTWAVE, DeleteCurrentWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES, SyncTileHeightStates);
        }
    }

    #endregion

    #region Private Interface    


    #endregion



    #region Public Interface

    public void SetOccupation(bool i_state)
    {
        occupied = i_state;
    }
    public void SetProp(GameObject prop) {
        this.prop = prop;
    }
    public void AddSpawn(GameObject spawn) {
        spawns.Add(spawn);
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
        SyncTileHeightStates();


        for (int i = spawns.Count - 1; i >= 0; i--) {
            Helpers.DestroyProper(spawns[i]);
        }
        spawns.Clear();

        if (prop!=null) {
            Helpers.DestroyProper(prop);
        }
        SetOccupation(false);
        SyncTileHeightStates();
    }

    public bool IsOccupied()
    {
        return occupied;
    }

    public void SetBlockID(int i_id)
    {
        blockID = i_id;
    }

    public int GetBlockID()
    {
        return blockID;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnTrans.position;
    }

    public List<LevelUnitStates> GetLevelStates()
    {
        return levelStates;
    }
    public LevelUnitStates GetStateAtWave(int waveIndex) {
        return levelStates[waveIndex];
    }
    public bool IsFlatAtWave(int waveIndex) {
        return GetStateAtWave(waveIndex) == LevelUnitStates.Floor;
    }

    public void SetLevelStates(List<LevelUnitStates> newLevelStates) {
        levelStates.Clear();
        newLevelStates.ForEach(state => {
            levelStates.Add(state);
        });
        SyncTileHeightStates();
    }


    public void AddNewWave(params object[] parameters)
    {
        LevelUnitStates newState = LevelUnitStates.Floor;
        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        if (levelStates.Count > currentWaveIndex) {
            newState = levelStates[currentWaveIndex];
        }
        levelStates.Insert(currentWaveIndex, newState);
    }

    public void DeleteCurrentWave(params object[] parameters)
    {
        levelStates.RemoveAt(PLESpawnManager.Instance.CurrentWaveIndex);
    }

    public void SyncTileHeightStates(params object[] parameters)
    {
        levelUnit.SetLevelUnitStates(levelStates);
    }

    #endregion
}
