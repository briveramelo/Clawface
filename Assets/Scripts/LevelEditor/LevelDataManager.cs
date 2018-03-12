//Brandon
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ModMan;
using System.Linq;
using UnityEngine.UI;
using PlayerLevelEditor;

public class LevelDataManager : MonoBehaviour {

    [Header("Required for all scenes")]
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private Transform tileParent, propsParent, spawnParent;

    [Header("Player Editor Scene-specific")]
    [SerializeField] private MainPLEMenu mainPLEMenu;
    [SerializeField] private InputField levelName;
    [SerializeField] private InputField levelDescription;

    private DataPersister dataPersister { get { return DataPersister.Instance; } }
    public DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    public LevelData WorkingLevelData { get { return ActiveDataSave.workingLevelData; } }
    public List<WaveData> WorkingWaveData { get { return WorkingLevelData.waveData; } }
    public List<TileData> WorkingTileData { get { return WorkingLevelData.tileData; } }
    public List<PropData> WorkingPropData { get { return WorkingLevelData.propData; } }
    private int spawnLayerMask;

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
        yield return new WaitForEndOfFrame();
        LoadEntireLevel();
    }

    private void LoadEntireLevel() {
        LoadProps();
        LoadSpawnsAllOn();
        LoadTiles();
        LoadSpawnsToggledState();
        LoadImages();

        PLESpawnManager.Instance.MaxWaveIndex = WorkingLevelData.MaxWaveIndex;
        PLESpawnManager.Instance.InfiniteWavesEnabled = WorkingLevelData.isInfinite;

        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ON_LEVEL_DATA_LOADED);

        int waveIndex = 0;
        waveIndex = PLESpawnManager.Instance.SetToWave(waveIndex);//sets spawn parent enabling in here
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, waveIndex);

        if (SceneTracker.IsCurrentSceneEditor) {
            mainPLEMenu.SetMenuButtonInteractabilityByState();
            mainPLEMenu.SwitchToMenu(PLEMenu.FLOOR);
        }
    }    

    private void LoadProps() {
        List<string> propNames = new List<string>();
        propsParent.DestroyAllChildren();
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
            (mainPLEMenu.GetMenu(PLEMenu.PROPS) as PropsMenu).ResetMenu(propNames);
        }
    }

    private void LoadSpawnsAllOn() {
        List<string> spawnNames = new List<string>();
        List<GameObject> spawnObjects = Resources.LoadAll<GameObject>(Strings.Editor.SPAWN_OBJECTS_PATH).ToList();
        List<PLESpawn> pleSpawns = spawnObjects.Select(spawn => spawn.GetComponent<PLESpawn>()).ToList();
        List<PLESpawn> spawnedPLEs = new List<PLESpawn>();
        spawnParent.DestroyAllChildren();
        for (int i = 0; i < WorkingWaveData.Count; i++) {
            GameObject waveParent = new GameObject(GetWaveName(i));
            waveParent.transform.SetParent(spawnParent);
            for (int j = 0; j < WorkingWaveData[i].spawnData.Count; j++) {
                SpawnData spawnData = WorkingWaveData[i].spawnData[j];
                GameObject enemyUIPrefabToSpawn = pleSpawns.Find(spawn=>(int)(spawn.spawnType)==spawnData.spawnType).gameObject;
                Transform newSpawnTransform = Instantiate(enemyUIPrefabToSpawn, waveParent.transform, false).transform;
                newSpawnTransform.name = newSpawnTransform.name.TryCleanName(Strings.CLONE);
                spawnNames.Add(newSpawnTransform.name);
                PLESpawn pleSpawn = newSpawnTransform.GetComponent<PLESpawn>();
                spawnData.pleSpawn = pleSpawn;
                spawnedPLEs.Add(pleSpawn);
                pleSpawn.totalSpawnAmount = spawnData.count;
                pleSpawn.spawnType = (SpawnType)spawnData.spawnType;
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
            (mainPLEMenu.GetMenu(PLEMenu.SPAWN) as SpawnMenu).ResetMenu(spawnNames);
        }
    }

    private void LoadTiles() {
        PlayerLevelEditorGrid gridController = levelEditor.gridController;
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
            blockUnit.SetLevelStates(levelStates);
            List<GameObject> props = Physics.OverlapBox(tile.realTile.transform.position, new Vector3(1, 10, 1)).ToList().Where(prop => prop.GetComponent<PLEProp>()).Select(item => item.gameObject).ToList();

            if (props.Count > 0) {
                blockUnit.SetOccupation(true);
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
    public void SaveNewLevel() {
        ActiveDataSave.AddAndSelectNewLevel();
        SaveLevel();
    }

    public void SaveLevel() {
        SaveTiles();
        SaveProps();
        SaveSpawns();
        SaveLevelText();
        SaveWaveState();
        StartCoroutine(TakePictureAndSave());
    }
    private void SaveTiles() {
        WorkingTileData.Clear();
        for (int i = 0; i < tileParent.childCount; i++) {
            Transform tile = tileParent.GetChild(i);
            PLEBlockUnit blockUnit = tile.GetComponent<PLEBlockUnit>();
            if (blockUnit) {
                List<LevelUnitStates> levelStates = blockUnit.GetLevelStates();
                int tileType = 0; //HOW DO I GET THIS FOR REAL
                TileData tileData = new TileData(tileType, tile.position, levelStates);
                WorkingTileData.Add(tileData);
            }
        }
    }

    private void SaveProps() {
        //List<string> propNames = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).Select(item=>item.name).ToList();
        WorkingPropData.Clear();
        for (int i = 0; i < propsParent.childCount; i++) {
            Transform prop = propsParent.GetChild(i);
            string propType = prop.name;
            PropData propData = new PropData(propType, prop);
            WorkingPropData.Add(propData);
        }
    }
    public void SaveSpawns() {
        WorkingWaveData.Clear();
        spawnParent.SortChildrenByName();

        for (int i = 1; i < spawnParent.childCount; i++) {
            Transform waveParent = spawnParent.GetChild(i);
            int waveChildCount = waveParent.childCount;
            if (waveChildCount>0) {
                for (int j = 0; j < waveParent.childCount; j++) {
                    Transform spawnUI = waveParent.GetChild(j);
                    AddSpawnData(spawnUI, i-1);
                }
            }
        }

        if (SpawnMenu.playerSpawnInstance) {
            Transform keira = spawnParent.GetChild(0);
            AddSpawnData(keira, 0);
        }
    }

    void AddSpawnData(Transform spawnUI, int waveIndex) {
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
    }

    private void SaveLevelText() {
        WorkingLevelData.name = levelName.text;
        WorkingLevelData.description = levelDescription.text;
    }

    private void SaveWaveState() {
        WorkingLevelData.isInfinite = PLESpawnManager.Instance.InfiniteWavesEnabled;
    }

    IEnumerator TakePictureAndSave() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        levelEditor.gridController.SetGridVisiblity(false);
        mainPLEMenu.CanvasGroup.alpha = 0f;
        mainPLEMenu.GetMenu(PLEMenu.FLOOR).CanvasGroup.alpha = 0f;
        yield return new WaitForEndOfFrame();
        SavePicture();
        dataPersister.TrySave();
        yield return new WaitForEndOfFrame();
        mainPLEMenu.CanvasGroup.alpha = 1f;
        mainPLEMenu.GetMenu(PLEMenu.FLOOR).CanvasGroup.alpha = 1f;
        mainPLEMenu.SwitchToMenu(PLEMenu.FLOOR);
        levelEditor.gridController.SetGridVisiblity(true);
    }

    private void SavePicture() {
        Texture2D snapshot = new Texture2D((int)Camera.main.pixelRect.width, (int)Camera.main.pixelRect.height);
        Rect snapRect = Camera.main.pixelRect;
        snapshot.ReadPixels(snapRect, 0, 0);
        TextureScale.Bilinear(snapshot, (int)LevelData.fixedSize.x, (int)LevelData.fixedSize.y);
        snapshot.Apply();
        byte[] imageBytes = snapshot.EncodeToPNG();
        WorkingLevelData.SetPicture(imageBytes);
    }

    private string GetWaveName(int i) { return Strings.Editor.Wave + i; }
    #endregion

    #region Delete
    public void TryDeleteLevel(string uniqueName) {
        dataPersister.TryDeleteLevel(uniqueName);
        dataPersister.TrySave();
    }
    #endregion

}
