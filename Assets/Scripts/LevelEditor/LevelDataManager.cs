//Brandon
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;
using System.Linq;

public class LevelDataManager : RoutineRunner {

	[SerializeField] private DataPersister dataPersister;
    [SerializeField] private Transform tileParent, propsParent, spawnParent;
    [SerializeField] private PlayerLevelEditorGrid playerLevelEditorGrid;

    private DataSave DataSave { get { return DataPersister.ActiveDataSave; } }
    private LevelData ActiveLevelData { get { return DataSave.ActiveLevelData; } }
    private List<WaveData> ActiveWaveData { get { return ActiveLevelData.waveData; } set { ActiveLevelData.waveData = value; } }
    private List<TileData> ActiveTileData { get { return ActiveLevelData.tileData; } set { ActiveLevelData.tileData = value; } }
    private List<PropData> ActivePropData { get { return ActiveLevelData.propData; } set { ActiveLevelData.propData = value; } }

    private string GetWaveName(int i) { return Strings.Editor.Wave + i; }


    #region Unity Lifecycle

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            SaveLevel();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            LoadLevel(0);
        }
    }
    #endregion

    #region Load
    public void LoadLevel(int levelIndex) {
        dataPersister.Load(levelIndex);
        playerLevelEditorGrid.ResetGrid();
        Timing.RunCoroutine(DelayAction(()=> {
            LoadTiles();
            LoadProps();
            SpawnSpawns();
        }), coroutineName);        
    }

    void LoadTiles() {
        for (int i = 0; i < ActiveTileData.Count; i++) {
            TileData tileData = ActiveTileData[i];
            List<LevelUnitStates> levelStates = tileData.levelStates;
            GridTile tile = playerLevelEditorGrid.GetTileAtPoint(tileData.position.AsVector);
            tile.IsActive = true;
            LevelUnit levelUnit = tile.realTile.GetComponent<LevelUnit>();
            tile.realTile.transform.SetParent(tileParent);
            levelUnit.DeRegisterFromEvents();
            for (int j = 0; j < levelStates.Count; j++) {
                string eventName = Strings.Events.PLE_TEST_WAVE_ + j;
                LevelUnitStates state = levelStates[j];
                switch (state) {
                    case LevelUnitStates.cover: levelUnit.AddCoverStateEvent(eventName); break;
                    case LevelUnitStates.floor: levelUnit.AddFloorStateEvent(eventName); break;
                    case LevelUnitStates.pit: levelUnit.AddPitStateEvent(eventName); break;
                }
            }
        }
    }

    void LoadProps() {
        propsParent.DestroyAllChildren();
        List<GameObject> propPrefabs = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).ToList();
        for (int i = 0; i < ActivePropData.Count; i++) {
            PropData propData = ActivePropData[i];
            GameObject propToSpawn = propPrefabs.Find(item=>item.name==propData.propType);
            Transform child = Instantiate(propToSpawn, propsParent, false).transform;
            child.name = propData.propType;
            child.position = propData.position.AsVector;
            child.rotation = Quaternion.Euler(propData.rotation.AsVector);
        }
    }

    void SpawnSpawns() {
        List<GameObject> spawnObjects = Resources.LoadAll<GameObject>(Strings.Editor.SPAWN_OBJECTS_PATH).ToList();
        spawnParent.DestroyAllChildren();
        for (int i = 0; i < ActiveWaveData.Count; i++) {
            GameObject waveParent = new GameObject(GetWaveName(i));
            waveParent.transform.SetParent(spawnParent);
            for (int j = 0; j < ActiveWaveData[i].spawnData.Count; j++) {
                SpawnData spawnData = ActiveWaveData[i].spawnData[j];
                GameObject enemyUIPrefabToSpawn = spawnObjects[spawnData.spawnType];
                Transform child = Instantiate(enemyUIPrefabToSpawn, waveParent.transform, false).transform;
                PLESpawn pleSpawn = child.GetComponent<PLESpawn>();
                pleSpawn.spawnCount = spawnData.count;
                pleSpawn.spawnType = spawnData.SpawnType;
                child.position = spawnData.position.AsVector;
            }
        }
        spawnParent.ToggleAllChildren(false);
        Transform currentWaveChild = spawnParent.GetChild(WaveSystem.currentWave);
        if (currentWaveChild) {
            currentWaveChild.gameObject.SetActive(true);
        }
    }
    #endregion

    #region Save
    public void SaveLevel() {
        SaveTiles();
        SaveProps();
        SaveSpawns();

        dataPersister.TrySave();
    }
    void SaveTiles() {
        ActiveTileData.Clear();
        for (int i = 0; i < tileParent.childCount; i++) {
            Transform tile = tileParent.GetChild(i);
            PLEBlockUnit blockUnit = tile.GetComponent<PLEBlockUnit>();
            List<LevelUnitStates> levelStates = blockUnit.GetLevelStates();
            int tileType = 0; //HOW DO I GET THIS FOR REAL
            TileData tileData = new TileData(tileType, tile.position, levelStates);
            ActiveTileData.Add(tileData);
        }
    }

    void SaveProps() {
        //List<string> propNames = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).Select(item=>item.name).ToList();
        ActivePropData.Clear();
        for (int i = 0; i < propsParent.childCount; i++) {
            Transform prop = propsParent.GetChild(i);
            string propType = prop.name;
            PropData propData = new PropData(propType, prop);
            ActivePropData.Add(propData);
        }
    }
    void SaveSpawns() {
        ActiveWaveData.Clear();
        spawnParent.SortChildrenByName();
        for (int i = 0; i < spawnParent.childCount; i++) {
            ActiveWaveData.Add(new WaveData());
            Transform waveParent = spawnParent.GetChild(i);
            for (int j = 0; j < waveParent.childCount; j++) {
                Transform spawnUI = waveParent.GetChild(j);
                PLESpawn spawn = spawnUI.GetComponent<PLESpawn>();
                int spawnType = (int)spawn.spawnType;
                int spawnCount = spawn.spawnCount;
                SpawnData spawnData = new SpawnData(spawnType, spawnCount, spawnUI.position);
                ActiveWaveData[i].spawnData.Add(spawnData);
            }
        }
    }

    #endregion

}
