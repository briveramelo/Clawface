using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Steamworks;

public class DataPersister : Singleton<DataPersister> {

    #region Public Fields

    public static DataSave ActiveDataSave = new DataSave();
    public static string SavesPathDirectory { get { return Application.dataPath + "/Saves"; } }
    public DataSave dataSave;

    #endregion

    #region Private Fields
    private string DataSaveFilePath { get { return SavesPathDirectory + "/savefile.dat"; } }

    private LevelData WorkingLevelData;
    //TODO: No, delete these
    private string currentLevelDataFolder;
    private string currentImagePath;
    #endregion

    #region Event Subscriptions (Protected)

    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                {Strings.Events.SCENE_LOADED, WipeWorkingData},
            };
        }
    }

    #endregion

    #region Unity Lifecyle
    protected override void Awake() {
        base.Awake();
        Load();
    }    
    #endregion

    #region Public Interface
    public void Load() {
        ActiveDataSave = null;
        if (!Directory.Exists(SavesPathDirectory)) {
            Directory.CreateDirectory(SavesPathDirectory);
        }
        if (File.Exists(DataSaveFilePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(DataSaveFilePath, FileMode.Open);
            try {
                ActiveDataSave = new DataSave((DataSave)bf.Deserialize(fileStream));
                fileStream.Close();
            }
            catch {
                fileStream.Close();
                File.Delete(DataSaveFilePath);
                ActiveDataSave = new DataSave();
            }
        }
        else {
            ActiveDataSave = new DataSave();
        }
        ClearEmptyLevels();
        dataSave = ActiveDataSave;
        if (true) { }
    }

    public void TryDeleteLevel(string uniqueName) {
        ActiveDataSave.TryDeleteLevel(uniqueName);
    }

    public void TrySaveWorkingLevel() {
        ActiveDataSave.SaveWorkingLevelData();
        TrySaveDataFile();
    }
    public void TrySaveDataFile() {
        ClearEmptyLevels();
        SaveDataFile();
    }

    public void SaveSnapshotToFile(string i_fileName, byte[] i_imgData)
    {
        //currentImagePath = currentLevelDataFolder + WorkingLevelData.name + ".png";
        File.WriteAllBytes(i_fileName, i_imgData);
    }

    public void TrySaveLevelDataFile(string i_dir, LevelData i_Data)
    {
        if(!Directory.Exists(i_dir))
        {
            Directory.CreateDirectory(i_dir);
        }
        string completeFilePath = i_dir + "/" + i_Data.name + ".dat";
        SaveLevelDataFile(completeFilePath, i_Data);
    }

    public bool TrySendToSteam(LevelData i_Data)
    {
        WorkingLevelData = i_Data;
        GenerateSteamFileID();
        return true;
    }

    private void OnItemSubmitted(bool result)
    {
        if(result == true)
        {
            print("Item Submitted");
        }

    }
    #endregion

    #region Private Interface
    private void GenerateSteamFileID()
    {
        SteamWorkshop.Instance.CreateNewItem(OnCreateItem);
    }

    private void OnCreateItem(PublishedFileId_t fileId)
    {
        if (fileId.m_PublishedFileId != 0)
        {
            //success
            WorkingLevelData.fileID = fileId;
           SteamWorkshop.Instance.UpdateItem(
           WorkingLevelData.FileID,
           WorkingLevelData.name,
           WorkingLevelData.description,
           currentLevelDataFolder,
           currentImagePath,
           "sup",
           OnItemSubmitted
           );

        }
    }

    void SaveDataFile() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(DataSaveFilePath);
        bf.Serialize(fileStream, new DataSave(ActiveDataSave));
        fileStream.Close();
    }

    void SaveLevelDataFile(string i_filePath, LevelData i_Data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Create(i_filePath);
        bf.Serialize(fs, i_Data);
        fs.Close();
    }

    void ClearEmptyLevels() {
        Predicate<LevelData> containsLevel = level => !level.IsEmpty;
        Predicate<LevelData> containsEmptyLevel = level => level.IsEmpty;
        if (ActiveDataSave.levelDatas.Exists(containsLevel)) {
            while (ActiveDataSave.levelDatas.Exists(containsEmptyLevel)) {
                ActiveDataSave.levelDatas.Remove(ActiveDataSave.levelDatas.Find(containsEmptyLevel));
            }
        }
    }
    void WipeWorkingData(params object[] parameters) {
        if (SceneTracker.IsCurrentSceneEditor) {
            ActiveDataSave.workingLevelData = new LevelData();
        }
    }
    #endregion
}

#region Memory Structures
[Serializable]
public class DataSave {
    public DataSave() { }
    public DataSave(DataSave copy) {
        copy.levelDatas.ForEach(levelData => {
            levelDatas.Add(new LevelData(levelData));
        });
    }
    #region Serialized Data
    public List<LevelData> levelDatas = new List<LevelData>();
    #endregion


    int selectedIndex = 0;
    public LevelData workingLevelData = new LevelData();
    public LevelData SelectedLevelData {
        get { if (levelDatas.Count == 0) { levelDatas.Add(new LevelData()); } return levelDatas[SelectedLevelIndex]; }
        set { if (levelDatas.Count == 0) { levelDatas.Add(new LevelData()); } levelDatas[SelectedLevelIndex] = value; }
    }
    public int SelectedLevelIndex {
        get {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(levelDatas.Count - 1, 0));
            return selectedIndex;
        }
        set {
            selectedIndex = value;
        }
    }
    public void FillWorkingLevelDataWithExistingLevelData(string existingLevelUniqueName) {
        SelectedLevelIndex = levelDatas.FindIndex(levelData => levelData.UniqueSteamName==existingLevelUniqueName);
        workingLevelData = new LevelData(SelectedLevelData);
    }

    public void AddAndSelectNewLevel() {
        levelDatas.Add(new LevelData());
        SelectedLevelIndex = levelDatas.Count-1;
    }
    public void TryDeleteLevel(string uniqueName) {
        int levelIndex = levelDatas.FindIndex(levelData => levelData.UniqueSteamName == uniqueName);
        if (levelIndex <= levelDatas.Count-1 && levelIndex >= 0) {
            levelDatas.RemoveAt(levelIndex);
        }
    }
    public void SaveWorkingLevelData() {        
        workingLevelData.SaveTimestamp();
        SelectedLevelData = new LevelData(workingLevelData);
    }
}

[Serializable]
public class LevelData {
    public LevelData() { }
    public LevelData(LevelData copy) {
        this.name = copy.name;
        this.description = copy.description;
        this.isFavorite = copy.isFavorite;
        this.isDownloaded = copy.isDownloaded;
        this.isMadeByThisUser = copy.isMadeByThisUser;
        this.isInfinite = copy.isInfinite;
        this.isHathosLevel = copy.isHathosLevel;
        this.dateString = copy.dateString;
        this.fileID = copy.fileID;
        if (copy.imageData!=null) {
            this.imageData = copy.imageData.ToArray();
        }
        copy.waveData.ForEach(wave => { this.waveData.Add(new WaveData(wave)); });
        copy.tileData.ForEach(tile => { this.tileData.Add(new TileData(tile)); });
        copy.propData.ForEach(prop => { this.propData.Add(new PropData(prop)); });
    }

    private readonly DateTime epochStart = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public long TimeSavedInMillisecondsSinceEpoch { get { return (long)(TimeSaved - epochStart).TotalMilliseconds; } }

    #region SerializedFields
    public string name, description;
    public bool isFavorite = false;
    public bool isDownloaded = false;
    public bool isMadeByThisUser = true;
    public bool isInfinite;
    public bool isHathosLevel;
    public PublishedFileId_t fileID;
    [SerializeField] string dateString;
    [HideInInspector] public byte[] imageData;
    public List<WaveData> waveData = new List<WaveData>();
    public List<TileData> tileData = new List<TileData>();
    public List<PropData> propData = new List<PropData>();
    #endregion

    #region Readonly
    public static readonly Vector2 fixedSize = new Vector2(656, 369);
    public Sprite MySprite {
        get {
            if (snapShot == null) {
                CreateSprite();
            }
            return snapShot;
        }
        set {
            snapShot = value;
        }
    }
    public DateTime TimeSaved {
        get {
            if(timeSaved==null){
                if (!isHathosLevel)
                {
                    if (string.IsNullOrEmpty(dateString))
                    {
                        timeSaved = DateTime.UtcNow;
                        dateString = timeSaved.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        timeSaved = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
                    }
                }
                else
                {
                    timeSaved = epochStart;
                }
            }
            return timeSaved.Value; 
        }
    }
    #endregion

    #region Non Serialized
    [NonSerialized] Texture2D imageTexture;
    [NonSerialized] Sprite snapShot;
    [NonSerialized] DateTime? timeSaved;

    #endregion

    #region Public Interface
    public void SetPicture(byte[] imageData) {
        this.imageData = imageData;
        CreateSprite();
    }
    public string UniqueSteamName { get { return name + TimeSavedInMillisecondsSinceEpoch; } }

    public bool IsEmpty { get { return string.IsNullOrEmpty(name); } }

    public int NumSpawns(SpawnType spawnType, int waveIndex) {
        int totalSpawnsOfType = 0;
        List<PLESpawn> spawns = GetPLESpawnsFromWave(waveIndex);
        for (int i = 0; i < spawns.Count; i++) {
            if (spawns[i].spawnType==spawnType) {
                totalSpawnsOfType += spawns[i].totalSpawnAmount;
            }
        }
        return totalSpawnsOfType;
    }
    public int MinNumSpawns(SpawnType spawnType, int waveIndex) {
        List<PLESpawn> spawns = GetPLESpawnsFromWave(waveIndex);
        PLESpawn spawn = spawns.Find(item=>item.spawnType==spawnType);
        if (spawn) {
            return spawn.MinSpawns;
        }
        return 0;
    }

    public List<PLESpawn> GetPLESpawnsFromWave(int i_wave)
    {
        while (waveData.Count<=i_wave) {
            waveData.Add(new WaveData());
        }
        return waveData[i_wave].GetPleSpawnsFromWave();
    }
    public SpawnData KeiraSpawnData {
        get {
            Predicate<SpawnData> isKeira = item => item.SpawnType == SpawnType.Keira;           
            Predicate<WaveData> waveHasKeira = waveElement => waveElement.spawnData.Exists(isKeira);
            WaveData keiraWave = waveData.Find(waveHasKeira);
            SpawnData keiraSpawnData = null;
            if (keiraWave!=null) {
                keiraSpawnData = keiraWave.spawnData.Find(isKeira);
            }
            return keiraSpawnData;
        }
    }
    public bool AllWavesHaveEnemies(int maxWaveIndex){
        if (waveData.Count == 0) return false;

        List<PLESpawn> waveOne = GetPLESpawnsFromWave(0);
        if (waveOne.Count==0 || (waveOne.Count==1 && waveOne.Exists(spawn=>spawn.spawnType==SpawnType.Keira))) {
            return false;
        }
        for (int i = 1; i <= maxWaveIndex; i++) {
            List<PLESpawn> spawns = GetPLESpawnsFromWave(i);
            if (spawns.Count==0) {
                return false;
            }
        }
        return true;
    }
    public int WaveCount { get { return waveData.Count; } }
    public int TileCount { get { return tileData.Count; } }
    public int PropCount { get { return propData.Count; } }
    public PublishedFileId_t FileID { get { return fileID; } }


    public void SaveTimestamp() {
        this.timeSaved = DateTime.UtcNow;
        dateString = timeSaved.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
    public int MaxWaveIndex {
        get {
            int tileWaveCount = 0;
            if (tileData.Count>0) {
                tileWaveCount = tileData[0].levelStates.Count;
            }
            int longestListLength = Mathf.Max(WaveCount, tileWaveCount);
            return Mathf.Clamp(longestListLength - 1, 0, longestListLength);
        }
    }
    #endregion

    #region Private Interface
    void CreateSprite() {
        imageTexture = new Texture2D((int)fixedSize.x, (int)fixedSize.y);
        if (imageData != null) {
            imageTexture.LoadImage(imageData);
        }
        snapShot = Sprite.Create(imageTexture, new Rect(Vector2.zero, fixedSize), Vector2.one * .5f);
    }
    #endregion
}

[Serializable]
public class WaveData {
    public WaveData() { }
    public WaveData(WaveData copy) {
        copy.spawnData.ForEach(spawn => {
            this.spawnData.Add(new SpawnData(spawn));
        });
        copy.minSpawnsData.ForEach(minSpawns => {
            this.minSpawnsData.Add(new MinSpawnsData(minSpawns));
        });
    }


    public List<SpawnData> spawnData = new List<SpawnData>();
    public List<MinSpawnsData> minSpawnsData = new List<MinSpawnsData>();

    public List<PLESpawn> GetPleSpawnsFromWave() {
        List<PLESpawn> spawns = spawnData.Select(item => { return item.pleSpawn; }).ToList();
        return spawns;
    }
    public void SetMinSpawns(int spawnType, int minCount) {
        Predicate<MinSpawnsData> typeMatch = data => data.spawnType == spawnType;
        if (!minSpawnsData.Exists(typeMatch)) {
            minSpawnsData.Add(new MinSpawnsData(spawnType, minCount));
        }
        else {
            minSpawnsData.FindAll(typeMatch).ForEach(data=>data.minCount = minCount);
        }
    }
    public int GetMinSpawns(SpawnType type) {
        MinSpawnsData minSpawns = minSpawnsData.Find(data => data.SpawnType == type);
        if (minSpawns != null) {
            return minSpawns.minCount;
        }
        return 0;
    }
    public int WaveCount { get { return spawnData.Count; } }
}

[Serializable]
public class MinSpawnsData {
    public MinSpawnsData(MinSpawnsData copy) {
        this.spawnType = copy.spawnType;
        this.minCount = copy.minCount;
    }
    public MinSpawnsData(int spawnType, int minCount) {
        this.spawnType = spawnType;
        this.minCount = minCount;
    }

    public int minCount;
    public int spawnType;
    public SpawnType SpawnType { get { return (SpawnType)spawnType; } }
}

    [Serializable]
public class SpawnData {
    public SpawnData(int spawnType, int count, Vector3 position) {
        this.spawnType = spawnType;
        this.count = count;
        this.position = new Vector3_S(position);
    }
    public SpawnData(SpawnData copy) {
        this.spawnType = copy.spawnType;
        this.count = copy.count;
        this.position = copy.position;
        this.pleSpawn = copy.pleSpawn;//reference is desire here
    }

    public int spawnType;
    public int count;
    public Vector3_S position = new Vector3_S();
    [NonSerialized] public PLESpawn pleSpawn;

    public SpawnType SpawnType { get { return (SpawnType)spawnType; } }
}


[Serializable]
public class TileData {
    public TileData(int tileType, Vector3 position, List<LevelUnitStates> levelStates) {
        this.tileType = tileType;
        this.position = new Vector3_S(position);
        this.levelStates = levelStates;
    }
    public TileData(TileData copy) {
        this.tileType = copy.tileType;
        this.position = copy.position;
        copy.levelStates.ForEach(state => {
            levelStates.Add(state);
        });
    }

    public int tileType;
    public Vector3_S position;
    public List<LevelUnitStates> levelStates = new List<LevelUnitStates>();
    public Color RiseColor { get { return TileColors.GetColor(tileType); } }
}

[Serializable] //No enum for prop types?
public class PropData {
    public PropData(PropData copy) {
        this.propType = copy.propType;
        this.position = copy.position;
        this.rotation = copy.rotation;
    }

    public PropData(string propType, Transform trans) {
        this.propType = propType;
        this.position = new Vector3_S(trans.position);
        this.rotation = new Vector3_S(trans.rotation.eulerAngles);
    }

    public string propType;
    public Vector3_S position;
    public Vector3_S rotation;
}

[Serializable]
public struct Vector3_S {
    public float x;
    public float y;
    public float z;
    public Vector3_S(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3_S(Vector3 position) {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }
    public Vector3 AsVector { get { return new Vector3(x, y, z); } }
}

[Serializable]
public struct Vector2_S {
    public float x;
    public float y;
    public Vector2_S(float x, float y) {
        this.x = x;
        this.y = y;        
    }
    public Vector2_S(Vector2 position) {
        this.x = position.x;
        this.y = position.y;
    }
    public Vector2 AsVector { get { return new Vector2(x, y); } }
}

[Serializable]
public struct Color_S {
    public float r;
    public float g;
    public float b;
    public float a;
    public Color_S(float r, float g, float b, float a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public Color_S(Color color) {
        this.r = color.r;
        this.g = color.g;
        this.b = color.b;
        this.a = color.a;
    }
    public Color AsColor { get { return new Color(r, g, b, a); } }
}
#endregion