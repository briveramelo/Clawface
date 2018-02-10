using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using MovementEffects;


//Questions for Garin/Lai
//How can I reference tile prefabs / prop prefabs?
public class LevelDataManager : RoutineRunner {

	[SerializeField] DataPersister dataPersister;
    [SerializeField] Transform tileParent, propsParent, spawnParent;
    [SerializeField] PlayerLevelEditorGrid playerLevelEditorGrid;

    DataSave DataSave { get { return DataPersister.ActiveDataSave; } }
    LevelData ActiveLevelData { get { return DataSave.ActiveLevelData; } }
    List<WaveData> ActiveWaveData { get { return ActiveLevelData.waveData; } set { ActiveLevelData.waveData = value; } }
    List<TileData> ActiveTileData { get { return ActiveLevelData.tileData; } set { ActiveLevelData.tileData = value; } }
    List<PropData> ActivePropData { get { return ActiveLevelData.propData; } set { ActiveLevelData.propData = value; } }
    
    string GetWaveName(int i) { return Strings.Editor.Wave + i; }


    #region UnityLifecycle
    private void Awake() {
        
    }

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
            SpawnProps();
        }), coroutineName);
        
        //SpawnSpawns();
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

    void SpawnProps() {
        GameObject[] propPrefabs = Resources.LoadAll<GameObject>(Strings.Editor.RESOURCE_PATH + Strings.Editor.ENV_OBJECTS_PATH);
        for (int i = 0; i < ActivePropData.Count; i++) {
            PropData propData = ActivePropData[i];
            GameObject propToSpawn = propPrefabs[propData.propType];
            Transform child = Instantiate(propToSpawn, spawnParent, false).transform;
            child.position = propData.position.AsVector;
            child.rotation = Quaternion.Euler(propData.rotation.AsVector);
        }
    }

    void SpawnSpawns() {
        //FIX
        GameObject[] enemyUIPrefabs = Resources.LoadAll<GameObject>(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK);
        for (int i = 0; i < ActiveWaveData.Count; i++) {
            GameObject waveParent = new GameObject(GetWaveName(i));
            spawnParent.transform.SetParent(spawnParent);
            for (int j = 0; j < ActiveWaveData[i].spawnData.Count; j++) {
                SpawnData spawnData = ActiveWaveData[i].spawnData[j];
                int spawnType = spawnData.spawnType;
                GameObject enemyUIPrefabToSpawn = enemyUIPrefabs[spawnType];
                Transform child = Instantiate(enemyUIPrefabToSpawn, waveParent.transform, false).transform;
                //child.GetComponent<SPAWNUI>.SetCount(spawnData.count);
                child.position = spawnData.position.AsVector;
            }
        }
    }
    #endregion

    #region Save
    public void SaveLevel() {
        SaveTiles();
        SaveProps();
        //SaveSpawns();

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
        ActivePropData.Clear();
        for (int i = 0; i < propsParent.childCount; i++) {
            Transform prop = propsParent.GetChild(i);
            int propType = 0; //HOW DO I GET THIS FOR REAL
            PropData propData = new PropData(propType, prop);
            ActivePropData.Add(propData);
        }
    }
    void SaveSpawns() {
        ActiveWaveData.Clear();
        for (int i = 0; i < spawnParent.childCount; i++) {
            ActiveWaveData.Add(new WaveData());
            Transform waveParent = spawnParent.GetChild(i);
            for (int j = 0; j < waveParent.childCount; j++) {
                Transform spawnUI = waveParent.GetChild(j);
                int spawnType = 0; //HOW DO I GET THIS FOR REAL
                int spawnCount = 1;//prop.GetComponent<SPAWNUI>.GetCount();
                SpawnData spawnData = new SpawnData(spawnType, spawnCount, spawnUI.position);
                ActiveWaveData[i].spawnData.Add(spawnData);
            }
        }
    }

    #endregion

}
