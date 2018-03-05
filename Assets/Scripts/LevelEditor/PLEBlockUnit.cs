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
            RegisterDefaultState();
        }
    }

    private void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_ADD_WAVE, AddNewWave);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_DELETE_CURRENTWAVE, DeleteCurrentWave);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, UpdateTileHeightStates);
    }

    private void OnDisable()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_ADD_WAVE, AddNewWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_DELETE_CURRENTWAVE, DeleteCurrentWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, UpdateTileHeightStates);
        }
    }

    #endregion

    #region Private Interface

    void RegisterDefaultState()
    {
        LevelUnit levelUnit = GetComponent<LevelUnit>();
        if (levelUnit == null) return;

        string eventName = Strings.Events.PLE_RESET_LEVELSTATE;
        levelUnit.AddStateEvent(LevelUnitStates.Floor, eventName);
        levelUnit.RegisterToEvents();
    }


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
            return spawns.Exists(spawn => { return spawn.activeSelf; });
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
        UpdateTileHeightStates();


        for (int i = spawns.Count - 1; i >= 0; i--) {
            Helpers.DestroyProper(spawns[i]);
        }
        spawns.Clear();

        if (prop!=null) {
            Helpers.DestroyProper(prop);
        }
        SetOccupation(false);
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
        UpdateTileHeightStates();
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

    public void UpdateTileHeightStates(params object[] parameters)
    {
        levelUnit.DeRegisterFromEvents();
        string eventName = null;
        for (int i = 0; i < levelStates.Count; i++) {
            eventName = string.Format("{0}{1}", Strings.Events.PLE_TEST_WAVE_, i.ToString());
            LevelUnitStates state = levelStates[i];
            levelUnit.AddStateEvent(state, eventName);
        }
        levelUnit.RegisterToEvents();
    }

    #endregion
}
