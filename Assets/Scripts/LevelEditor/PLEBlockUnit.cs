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
            for (int i = 0; i < WaveSystem.maxWave; i++) {
                levelStates.Add(LevelUnitStates.floor);
            }
            RegisterDefaultState();
        }
    }

    private void OnEnable() {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_ADD_WAVE, AddWave);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, UpdateDynamicLevelState);
    }

    private void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_ADD_WAVE, AddWave);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_UPDATE_LEVELSTATE, UpdateDynamicLevelState);
        }
    }

    #endregion

    #region Private Interface

    void RegisterDefaultState()
    {
        LevelUnit levelUnit = GetComponent<LevelUnit>();
        if (levelUnit == null) return;

        string event_name = Strings.Events.PLE_RESET_LEVELSTATE;
        levelUnit.AddFloorStateEvent(event_name);
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
        for (int i = 0; i < WaveSystem.maxWave; i++) {
            levelStates.Add(LevelUnitStates.floor);
        }
        UpdateDynamicLevelState();


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
        return GetStateAtWave(waveIndex) == LevelUnitStates.floor;
    }

    public void SetLevelStates(List<LevelUnitStates> newLevelStates) {
        levelStates.Clear();
        newLevelStates.ForEach(state => {
            levelStates.Add(state);
        });
        UpdateDynamicLevelState();
    }


    public void AddWave(params object[] parameters)
    {
        levelStates.Add(LevelUnitStates.floor);
    }

    public void UpdateDynamicLevelState(params object[] parameters)
    {
        LevelUnit levelUnit = GetComponent<LevelUnit>();

        if (levelUnit == null) return;

        levelUnit.DeRegisterFromEvents();
        for (int i = 0; i < levelStates.Count; i++) {
            string event_name = Strings.Events.PLE_TEST_WAVE_ + i.ToString();
            
            LevelUnitStates state = levelStates[i];

            switch (state) {
                case LevelUnitStates.cover:
                    levelUnit.AddCoverStateEvent(event_name);
                    break;
                case LevelUnitStates.floor:
                    levelUnit.AddFloorStateEvent(event_name);
                    break;
                case LevelUnitStates.pit:
                    levelUnit.AddPitStateEvent(event_name);
                    break;
            }
        }

        levelUnit.RegisterToEvents();
    }

    #endregion
}
