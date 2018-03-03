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
    [SerializeField] private DataPersister dataPersister;
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private Transform tileParent, propsParent, spawnParent;

    [Header("Player Editor Scene-specific")]
    [SerializeField] private InputField levelName;
    [SerializeField] private InputField levelDescription;

    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private LevelData ActiveLevelData { get { return ActiveDataSave.ActiveLevelData; } }
    private List<WaveData> ActiveWaveData { get { return ActiveLevelData.waveData; } }
    private List<TileData> ActiveTileData { get { return ActiveLevelData.tileData; } }
    private List<PropData> ActivePropData { get { return ActiveLevelData.propData; } }


    #region Unity Lifecycle
    private void Start() {
        LoadSelectedLevel();
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
        if (SceneTracker.IsCurrentSceneEditor) {
            WaveSystem.maxWave = ActiveLevelData.MaxWave;
            levelEditor.waveSystem.ResetToWave0();
            levelEditor.CheckToSetMenuInteractability();
        }
        else {
            //Trigger level start event?
            levelEditor.PlayLevel();
        }
    }

    private void LoadProps() {
        List<string> propNames = new List<string>();
        propsParent.DestroyAllChildren();
        List<GameObject> propPrefabs = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).ToList();
        for (int i = 0; i < ActivePropData.Count; i++) {
            PropData propData = ActivePropData[i];
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
            (levelEditor.GetMenu(PLEMenu.PROPS) as PropsMenu).ResetMenu(propNames);
        }
    }

    private void LoadSpawnsAllOn() {
        List<string> spawnNames = new List<string>();
        List<GameObject> spawnObjects = Resources.LoadAll<GameObject>(Strings.Editor.SPAWN_OBJECTS_PATH).ToList();
        List<PLESpawn> pleSpawns = spawnObjects.Select(spawn => spawn.GetComponent<PLESpawn>()).ToList();
        List<PLESpawn> spawnedPLEs = new List<PLESpawn>();
        spawnParent.DestroyAllChildren();
        for (int i = 0; i < ActiveWaveData.Count; i++) {
            GameObject waveParent = new GameObject(GetWaveName(i));
            waveParent.transform.SetParent(spawnParent);
            for (int j = 0; j < ActiveWaveData[i].spawnData.Count; j++) {
                SpawnData spawnData = ActiveWaveData[i].spawnData[j];
                GameObject enemyUIPrefabToSpawn = pleSpawns.Find(spawn=>(int)(spawn.spawnType)==spawnData.spawnType).gameObject;
                Transform child = Instantiate(enemyUIPrefabToSpawn, waveParent.transform, false).transform;
                child.name = child.name.TryCleanClone();
                spawnNames.Add(child.name);
                PLESpawn pleSpawn = child.GetComponent<PLESpawn>();
                spawnedPLEs.Add(pleSpawn);
                pleSpawn.totalSpawnAmount = spawnData.count;
                pleSpawn.spawnType = (SpawnType)spawnData.spawnType;
                child.position = spawnData.position.AsVector;

                //Hook in when it's ready
                //Might also need to wipe out the list
                //PLESpawnManager.Instance.RegisterSpawner(i, pleSpawn);
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
            (levelEditor.GetMenu(PLEMenu.SPAWN) as SpawnMenu).ResetMenu(spawnNames);
        }
    }

    private void LoadTiles() {
        levelEditor.gridController.ResetGrid();
        for (int i = 0; i < ActiveTileData.Count; i++) {
            TileData tileData = ActiveTileData[i];
            List<LevelUnitStates> levelStates = new List<LevelUnitStates>();
            foreach (LevelUnitStates levelUnitState in tileData.levelStates) {
                levelStates.Add(levelUnitState);
            }
            GridTile tile = levelEditor.gridController.GetTileAtPoint(tileData.position.AsVector);
            tile.IsActive = true;
            LevelUnit levelUnit = tile.realTile.GetComponent<LevelUnit>();
            PLEBlockUnit blockUnit = tile.realTile.GetComponent<PLEBlockUnit>();
            blockUnit.SetLevelStates(levelStates);
            List<GameObject> props = Physics.OverlapBox(tile.realTile.transform.position, new Vector3(1, 10, 1)).ToList().Where(prop => prop.GetComponent<PLEProp>()).Select(item => item.gameObject).ToList();

            if (props.Count > 0) {
                blockUnit.SetOccupation(true);
                blockUnit.SetProp(props[0]);
            }

            List<PLESpawn> spawns = Physics.OverlapBox(tile.realTile.transform.position, new Vector3(1, 10, 1)).ToList().Where(prop => prop.GetComponent<PLESpawn>()).Select(prop => prop.GetComponent<PLESpawn>()).ToList();
            spawns.ForEach(spawn => {
                blockUnit.AddSpawn(spawn.gameObject);
            });

            tile.realTile.transform.SetParent(tileParent);
        }
        levelEditor.gridController.ShowWalls();
        if (!SceneTracker.IsCurrentSceneEditor) {
            levelEditor.gridController.SetGridVisiblity(false);
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
        StartCoroutine(TakePictureAndSave());
    }
    private void SaveTiles() {
        ActiveTileData.Clear();
        for (int i = 0; i < tileParent.childCount; i++) {
            Transform tile = tileParent.GetChild(i);
            PLEBlockUnit blockUnit = tile.GetComponent<PLEBlockUnit>();
            if (blockUnit) {
                List<LevelUnitStates> levelStates = blockUnit.GetLevelStates();
                int tileType = 0; //HOW DO I GET THIS FOR REAL
                TileData tileData = new TileData(tileType, tile.position, levelStates);
                ActiveTileData.Add(tileData);
            }
        }
    }

    private void SaveProps() {
        //List<string> propNames = Resources.LoadAll<GameObject>(Strings.Editor.ENV_OBJECTS_PATH).Select(item=>item.name).ToList();
        ActivePropData.Clear();
        for (int i = 0; i < propsParent.childCount; i++) {
            Transform prop = propsParent.GetChild(i);
            string propType = prop.name;
            PropData propData = new PropData(propType, prop);
            ActivePropData.Add(propData);
        }
    }
    private void SaveSpawns() {
        ActiveWaveData.Clear();
        spawnParent.SortChildrenByName();
        int startIndex = 0;
        if (SpawnMenu.playerSpawnInstance) {
            startIndex = 1;            
        }

        for (int i = startIndex; i < spawnParent.childCount; i++) {
            int currentIndex = SpawnMenu.playerSpawnInstance ? i-1 : i;
            ActiveWaveData.Add(new WaveData());
            Transform waveParent = spawnParent.GetChild(i);
            for (int j = 0; j < waveParent.childCount; j++) {
                Transform spawnUI = waveParent.GetChild(j);
                PLESpawn spawn = spawnUI.GetComponent<PLESpawn>();
                int spawnType = (int)spawn.spawnType;
                int spawnCount = spawn.totalSpawnAmount;
                SpawnData spawnData = new SpawnData(spawnType, spawnCount, spawnUI.position);
                ActiveWaveData[currentIndex].spawnData.Add(spawnData);
            }
        }

        if (SpawnMenu.playerSpawnInstance) {
            Transform keira = spawnParent.GetChild(0);
            SpawnData keiraSpawnData = new SpawnData((int)SpawnType.Keira, 1, keira.position);
            ActiveWaveData[0].spawnData.Add(keiraSpawnData);
        }
    }

    private void SaveLevelText() {
        ActiveLevelData.name = levelName.text;
        ActiveLevelData.description = levelDescription.text;
    }

    IEnumerator TakePictureAndSave() {
        levelEditor.GetMenu(PLEMenu.MAIN).CanvasGroup.alpha = 0f;
        yield return new WaitForEndOfFrame();
        SavePicture();
        dataPersister.TrySave();
        yield return new WaitForEndOfFrame();
        levelEditor.GetMenu(PLEMenu.MAIN).CanvasGroup.alpha = 1f;
    }

    private void SavePicture() {
        Texture2D snapshot = new Texture2D((int)Camera.main.pixelRect.width, (int)Camera.main.pixelRect.height);
        Rect snapRect = Camera.main.pixelRect;
        snapshot.ReadPixels(snapRect, 0, 0);
        TextureScale.Bilinear(snapshot, (int)LevelData.fixedSize.x, (int)LevelData.fixedSize.y);
        snapshot.Apply();
        byte[] imageBytes = snapshot.EncodeToPNG();
        ActiveLevelData.SetPicture(imageBytes);
    }

    private string GetWaveName(int i) { return Strings.Editor.Wave + i; }
    #endregion

    #region Delete
    public void DeleteSelectedLevel() {
        dataPersister.DeleteSelectedLevel();
        dataPersister.TrySave();
    }
    #endregion

}
