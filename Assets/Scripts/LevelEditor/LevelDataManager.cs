//Brandon
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ModMan;
using System.Linq;
using UnityEngine.UI;
using PLE;
using System;

public class LevelDataManager : MonoBehaviour {

    #region Serialized Unity Fields

    [Header("Required for all scenes")]
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private Transform tileParent, propsParent, spawnParent;

    [Header("Player Editor Scene-specific")]
    [SerializeField] private MainPLEMenu mainPLEMenu;
    [SerializeField] private InputField levelName;
    [SerializeField] private InputField levelDescription;

    #endregion

    #region Public Fields

    public DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    public LevelData WorkingLevelData { get { return ActiveDataSave.workingLevelData; } }
    public List<WaveData> WorkingWaveData { get { return WorkingLevelData.waveData; } }
    public List<TileData> WorkingTileData { get { return WorkingLevelData.tileData; } }
    public List<PropData> WorkingPropData { get { return WorkingLevelData.propData; } }

    #endregion

    #region Private Fields
    private int spawnLayerMask;
    private DataPersister dataPersister { get { return DataPersister.Instance; } }
    #endregion

    #region Unity Lifecycle

    private void Awake() {
        spawnLayerMask = LayerMask.GetMask(Strings.Layers.SPAWN);
    }
    private void Start() {
        if (!SceneTracker.IsCurrentSceneEditor) {
            LoadSelectedLevel();
        }
    }
    #endregion

    #region Load
    public void LoadSelectedLevel() {
        StartCoroutine(DelayLoadOneFrame());        
    }

    private IEnumerator DelayLoadOneFrame() {
        propsParent.DestroyAllChildren();
        spawnParent.DestroyAllChildren();
        yield return null;
        LoadEntireLevel();
    }

    private void LoadEntireLevel() {
        PLESpawnManager.Instance.MaxWaveIndex = WorkingLevelData.MaxWaveIndex;
        PLESpawnManager.Instance.InfiniteWavesEnabled = WorkingLevelData.isInfinite;

        LoadProps();
        LoadSpawnsAllOn();
        LoadTiles();
        LoadSpawnsToggledState();
        LoadImages();


        int waveIndex = 0;
        waveIndex = PLESpawnManager.Instance.SetToWave(waveIndex);//sets spawn parent enabling in here
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ON_LEVEL_DATA_LOADED);

        if (SceneTracker.IsCurrentSceneEditor) {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, waveIndex);
            mainPLEMenu.SetMenuButtonInteractabilityByState();
            mainPLEMenu.SelectMenuItem(PLEMenuType.FLOOR);
        }
    }    

    private void LoadProps() {
        List<string> propNames = new List<string>();        
        List<GameObject> propPrefabs = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).ToList();
        for (int i = 0; i < WorkingPropData.Count; i++) {
            PropData propData = WorkingPropData[i];
            GameObject propToSpawn = propPrefabs.Find(item => item.name == propData.propType);
            Transform child = Instantiate(propToSpawn, propsParent, false).transform;
            child.name = propData.propType;
            propNames.Add(child.name);
            child.position = propData.position.AsVector;
            child.rotation = Quaternion.Euler(propData.rotation.AsVector);
            Rigidbody rigbod = child.GetComponent<Rigidbody>();
            rigbod.isKinematic = true;
        }
        if (SceneTracker.IsCurrentSceneEditor) {
            (mainPLEMenu.GetMenu(PLEMenuType.PROPS) as PropsMenu).ResetMenu(propNames);
        }
    }

    private void LoadSpawnsAllOn() {
        List<string> spawnNames = new List<string>();
        List<GameObject> spawnObjects = Resources.LoadAll<GameObject>(Strings.Editor.SPAWN_OBJECTS_PATH).ToList();
        List<PLESpawn> pleSpawns = spawnObjects.Select(spawn => spawn.GetComponent<PLESpawn>()).ToList();
        List<PLESpawn> spawnedPLEs = new List<PLESpawn>();
        for (int i = 0; i < WorkingWaveData.Count; i++) {
            GameObject waveParent = new GameObject(GetWaveName(i));
            waveParent.transform.SetParent(spawnParent);

            for (int j = 0; j < WorkingWaveData[i].spawnData.Count; j++) {
                SpawnData spawnData = WorkingWaveData[i].spawnData[j];
                int minSpawns = WorkingWaveData[i].GetMinSpawns(spawnData.SpawnType);

                GameObject enemyUIPrefabToSpawn = pleSpawns.Find(spawn=>(int)(spawn.spawnType)==spawnData.spawnType).gameObject;
                Transform newSpawnTransform = Instantiate(enemyUIPrefabToSpawn, waveParent.transform, false).transform;
                newSpawnTransform.name = newSpawnTransform.name.TryCleanName(Strings.CLONE);
                spawnNames.Add(newSpawnTransform.name);
                PLESpawn pleSpawn = newSpawnTransform.GetComponent<PLESpawn>();
                spawnData.pleSpawn = pleSpawn;
                
                spawnedPLEs.Add(pleSpawn);
                pleSpawn.minSpawns = minSpawns;
                pleSpawn.totalSpawnAmount = spawnData.count;
                pleSpawn.spawnType = (SpawnType)spawnData.spawnType;
                pleSpawn.tile = levelEditor.gridController.GetTileAtPoint(spawnData.position.AsVector);
                newSpawnTransform.position = spawnData.position.AsVector;
            }
        }
        

        System.Predicate<PLESpawn> keiraSpawn = spawn => spawn.spawnType == SpawnType.Keira;
        if (spawnedPLEs.Exists(keiraSpawn)) {
            Transform keiraSpawnTransform = spawnedPLEs.Find(keiraSpawn).transform;
            keiraSpawnTransform.SetParent(spawnParent);
            keiraSpawnTransform.SetAsFirstSibling();
            SpawnMenu.playerSpawnInstance = keiraSpawnTransform.gameObject;
        }
        if (SceneTracker.IsCurrentSceneEditor) {
            (mainPLEMenu.GetMenu(PLEMenuType.SPAWN) as SpawnMenu).ResetMenu(spawnNames);
        }
    }

    private void LoadTiles() {
        PLEGrid gridController = levelEditor.gridController;
        gridController.ResetGrid();
        for (int i = 0; i < WorkingTileData.Count; i++) {
            TileData tileData = WorkingTileData[i];
            List<LevelUnitStates> levelStates = new List<LevelUnitStates>();
            foreach (LevelUnitStates levelUnitState in tileData.levelStates) {
                levelStates.Add(levelUnitState);
            }
            GridTile tile = gridController.GetTileAtPoint(tileData.position.AsVector);
            tile.IsActive = true;
            LevelUnit levelUnit = tile.levelUnit;
            PLEBlockUnit blockUnit = tile.blockUnit;
            blockUnit.SetLevelStates(levelStates, tileData.RiseColor);
            List<GameObject> props = Physics.OverlapBox(tile.realTile.transform.position, new Vector3(1, 10, 1)).ToList().Where(prop => prop.GetComponent<PLEProp>()).Select(item => item.gameObject).ToList();

            if (props.Count > 0) {
                blockUnit.SetProp(props[0]);
            }

            List<PLESpawn> spawns = Physics.OverlapBox(tile.realTile.transform.position, new Vector3(1, 10, 1), Quaternion.identity, spawnLayerMask).ToList().Select(prop => prop.GetComponent<PLESpawn>()).ToList();
            spawns.ForEach(spawn => {
                blockUnit.AddSpawn(spawn.gameObject);
            });

            tile.realTile.transform.SetParent(tileParent);
        }
        gridController.ShowWalls();
        if (!SceneTracker.IsCurrentSceneEditor) {
            gridController.SetGridVisiblity(false);
        }
    }

    private void LoadSpawnsToggledState() {
        spawnParent.ToggleAllChildren(false);
        if (spawnParent.childCount > 0) {
            Transform firstChild = spawnParent.GetChild(0);
            if (firstChild.name.Contains("Player")) {
                firstChild.gameObject.SetActive(true);
            }
            if (spawnParent.Find(GetWaveName(0))) {
                spawnParent.Find(GetWaveName(0)).gameObject.SetActive(true);
            }
        }
    }

    private void LoadImages() {
        ActiveDataSave.levelDatas.ForEach(levelData => { Sprite dummySprite = levelData.MySprite; });
    }

    #endregion

    #region Save
    public void SaveAndUploadSingleLevel(SteamWorkshop.SubmitItemCallBack onUploadComplete, Action onSaveComplete=null) {
        Action<LevelData> uploadOnFinishSaving = (levelData) => {
            if (onSaveComplete!=null) {
                onSaveComplete();
            }
            string levelDirectory = DataPersister.SavesPathDirectory + "/" + levelData.name + "/";
            string levelImagePath = levelDirectory + levelData.name + ".png";
            dataPersister.TrySaveLevelDataFile(levelDirectory, levelData);
            dataPersister.SaveSnapshotToFile(levelImagePath, levelData.imageData);
            SteamAdapter.GenerateFileIDAndUpload(levelDirectory, levelImagePath, levelData, onUploadComplete);
        };
        SaveLevel(false, uploadOnFinishSaving);
    }

    public void SaveNewLevel() {
        ActiveDataSave.AddAndSelectNewLevel();
        SaveLevel();
    }

    public void SaveLevel(bool isInternal=true, Action<LevelData> onFinishSavingLevel=null) {
        SyncWorkingTileData();
        SyncWorkingPropData();
        SyncWorkingSpawnData();
        SyncWorkingLevelText();
        SyncWorkingWaveState();
        StartCoroutine(TakePicture(isInternal, onFinishSavingLevel));
    }

    private void SyncWorkingTileData() {
        WorkingTileData.Clear();
        for (int i = 0; i < tileParent.childCount; i++) {
            Transform tile = tileParent.GetChild(i);
            PLEBlockUnit blockUnit = tile.GetComponent<PLEBlockUnit>();
            if (blockUnit) {
                List<LevelUnitStates> levelStates = blockUnit.GetLevelStates();
                int tileType = blockUnit.TileType;
                TileData tileData = new TileData(tileType, tile.position, levelStates);
                WorkingTileData.Add(tileData);
            }
        }
    }

    private void SyncWorkingPropData() {
        //List<string> propNames = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).Select(item=>item.name).ToList();
        WorkingPropData.Clear();
        for (int i = 0; i < propsParent.childCount; i++) {
            Transform prop = propsParent.GetChild(i);
            string propType = prop.name;
            PropData propData = new PropData(propType, prop);
            WorkingPropData.Add(propData);
        }
    }
    
    public void SyncWorkingSpawnData() {
        WorkingWaveData.Clear();
        spawnParent.SortChildrenByName();

        for (int i = 0; i < spawnParent.childCount; i++) {
            Transform waveParent = spawnParent.GetChild(i);
            PLESpawn spawn = waveParent.GetComponent<PLESpawn>();
            if (spawn != null) {
                AddSpawnData(waveParent, 0);//keira
            }
            else {//any other baddy
                int waveChildCount = waveParent.childCount;
                for (int j = 0; j < waveParent.childCount; j++) {
                    Transform spawnUI = waveParent.GetChild(j);
                    if (spawnUI) {
                        AddSpawnData(spawnUI, i-1);
                    }
                }
            }
        }
    }

    private void AddSpawnData(Transform spawnUI, int waveIndex) {
        waveIndex = Mathf.Max(waveIndex, 0);
        PLESpawn spawn = spawnUI.GetComponent<PLESpawn>();
        int spawnType = (int)spawn.spawnType;
        int spawnCount = spawn.totalSpawnAmount;
        SpawnData spawnData = new SpawnData(spawnType, spawnCount, spawnUI.position) {
            pleSpawn = spawn
        };
        while (WorkingWaveData.Count <= waveIndex) {
            WorkingWaveData.Add(new WaveData());
        }
        WorkingWaveData[waveIndex].spawnData.Add(spawnData);
        WorkingWaveData[waveIndex].SetMinSpawns(spawnType, spawn.MinSpawns);
    }

    private void SyncWorkingLevelText() {
        WorkingLevelData.name = levelName.text;
        WorkingLevelData.description = levelDescription.text;
    }

    private void SyncWorkingWaveState() {
        WorkingLevelData.isInfinite = PLESpawnManager.Instance.InfiniteWavesEnabled;
    }

    IEnumerator TakePicture(bool isInternal, Action<LevelData> onFinishSaving=null)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        levelEditor.gridController.SetGridVisiblity(false);        
        SetMenusAlpha(0f, PLEMenuType.MAIN, PLEMenuType.FLOOR, PLEMenuType.STEAM);
        yield return new WaitForEndOfFrame();
        SavePicture();
        dataPersister.TrySaveWorkingLevel();
        if (onFinishSaving!=null) {
            onFinishSaving(WorkingLevelData);
        }
        yield return new WaitForEndOfFrame();
        SetMenusAlpha(1f, PLEMenuType.MAIN, PLEMenuType.FLOOR, PLEMenuType.STEAM);
        if (isInternal) {
            mainPLEMenu.SelectMenuItem(PLEMenuType.FLOOR);
            levelEditor.gridController.SetGridVisiblity(true);
        }
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

    private void SetMenusAlpha(float alpha, params PLEMenuType[] menuTypes) {
        foreach (PLEMenuType menu in menuTypes) {
            mainPLEMenu.GetMenu(menu).CanvasGroup.alpha = alpha;
        }
    }


    private void SavePicture() {
        Texture2D snapshot = new Texture2D((int)Camera.main.pixelRect.width, (int)Camera.main.pixelRect.height);
        Rect snapRect = Camera.main.pixelRect;
        snapshot.ReadPixels(snapRect, 0, 0);
        TextureScale.Bilinear(snapshot, (int)LevelData.fixedSize.x, (int)LevelData.fixedSize.y);
        snapshot.Apply();
        WorkingLevelData.SetPicture(snapshot.EncodeToPNG());
    }

    private string GetWaveName(int i) {
        return Support.GetWaveName(i);
    }
    #endregion

    #region Delete
    public void TryDeleteLevel(string uniqueName) {
        dataPersister.TryDeleteLevel(uniqueName);
        dataPersister.TrySaveDataFile();
        if (SceneTracker.IsCurrentSceneEditor) {
            mainPLEMenu.SetMenuButtonInteractabilityByState();
        }
    }
    #endregion


}
