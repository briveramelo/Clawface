using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class DataPersister : MonoBehaviour {
    
    public static DataSave ActiveDataSave = new DataSave();
    string PathDirectory { get { return Application.dataPath + "/Saves";} }
    string FilePath { get { return PathDirectory + "/savefile.dat"; } }

    public DataSave dataSave;

    #region Unity Lifecyle
    private void Awake() {
        Load();
    }
    #endregion

    #region Public Interface
    public void Load() {
        ActiveDataSave = null;
        if (!Directory.Exists(PathDirectory)) {
            Directory.CreateDirectory(PathDirectory);
        }
        if (File.Exists(FilePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(FilePath, FileMode.Open);
            try {
                ActiveDataSave = new DataSave((DataSave)bf.Deserialize(fileStream));
                fileStream.Close();
            }
            catch {
                fileStream.Close();
                File.Delete(FilePath);
                ActiveDataSave = new DataSave();
            }
        }
        else {
            ActiveDataSave = new DataSave();
        }
        ClearEmptyLevels();
        dataSave = ActiveDataSave;
    }

    public void DeleteSelectedLevel() {
        ActiveDataSave.DeleteSelectedLevel();
        ActiveDataSave.AddAndSelectNewLevel();
    }

    public void TrySave() {
        ClearEmptyLevels();
        ActiveDataSave.SaveTimestamp();
        Save();
    }
    #endregion

    #region Private Interface
    void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(FilePath);
        bf.Serialize(fileStream, new DataSave(ActiveDataSave));
        fileStream.Close();
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
    #endregion
}

#region Memory Structures
[Serializable]
public class DataSave {
    public DataSave() { }
    public DataSave(DataSave dataSave) {
        this.levelDatas = dataSave.levelDatas;
    }
    #region Serialized Data
    public List<LevelData> levelDatas = new List<LevelData>();
    #endregion

    int selectedIndex = 0;

    public LevelData ActiveLevelData { get { if (levelDatas.Count==0) { levelDatas.Add(new LevelData()); } return levelDatas[SelectedLevelIndex]; } }
    public int SelectedLevelIndex {
        get {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(levelDatas.Count - 1, 0));
            return selectedIndex;
        }
        set {
            selectedIndex = value;
        }
    }

    public void AddAndSelectNewLevel() {
        levelDatas.Add(new LevelData());
        SelectedLevelIndex = levelDatas.Count-1;
    }
    public void DeleteSelectedLevel() {
        if (levelDatas.Count-1 >= SelectedLevelIndex){
            levelDatas.RemoveAt(SelectedLevelIndex);
        }
    }
    public void SaveTimestamp() {
        ActiveLevelData.SaveTimestamp();
    }
}

[Serializable]
public class LevelData {

    private readonly DateTime epochStart = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public long TimeSavedInMillisecondsSinceEpoch { get { return (long)(TimeSaved - epochStart).TotalMilliseconds; } }


    public string name, description;
    [HideInInspector] public byte[] imageData;
    public bool isFavorite = false;
    public bool isDownloaded = false;
    public bool isMadeByThisUser = true;
    public bool isInfinite;
    public bool isHathosLevel;
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

    [NonSerialized] Texture2D imageTexture;
    [NonSerialized] Sprite snapShot;
    [NonSerialized] DateTime? timeSaved;
    [SerializeField] string dateString;


    public void SetPicture(byte[] imageData) {
        this.imageData = imageData;
        CreateSprite();
    }
    public string UniqueSteamName { get { return name + TimeSavedInMillisecondsSinceEpoch; } }

    public bool IsEmpty { get { return string.IsNullOrEmpty(name); } }
    public List<WaveData> waveData = new List<WaveData>();
    public List<TileData> tileData = new List<TileData>();
    public List<PropData> propData = new List<PropData>();    

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

    void CreateSprite() {
        imageTexture = new Texture2D((int)fixedSize.x, (int)fixedSize.y);
        if (imageData != null) {
            imageTexture.LoadImage(imageData);
        }
        snapShot = Sprite.Create(imageTexture, new Rect(Vector2.zero, fixedSize), Vector2.one * .5f);
    }

}

[Serializable]
public class WaveData {
    public List<SpawnData> spawnData = new List<SpawnData>();
    public List<PLESpawn> GetPleSpawnsFromWave()
    {
        return spawnData.Select(item => { return item.pleSpawn; }).ToList();
    }
}

[Serializable]
public class SpawnData {
    public SpawnData(int spawnType, int count, Vector3 position) {
        this.spawnType = spawnType;
        this.count = count;
        this.position = new Vector3_S(position);
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

    public int tileType;
    public Vector3_S position;
    public List<LevelUnitStates> levelStates = new List<LevelUnitStates>();
}

[Serializable] //No enum for prop types?
public class PropData {
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