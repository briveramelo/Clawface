using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

    public LevelData ActiveLevelData { get { if (levelDatas.Count==0) { levelDatas.Add(new LevelData()); } return levelDatas[SelectedIndex]; } }
    public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = Mathf.Clamp(value, 0, Mathf.Max(levelDatas.Count - 1, 0)); } }

    public void AddAndSelectNewLevel() {
        levelDatas.Add(new LevelData());
        SelectedIndex = levelDatas.Count-1;
    }
    public void DeleteSelectedLevel() {
        if (levelDatas.Count-1 >= SelectedIndex){
            levelDatas.RemoveAt(SelectedIndex);
        }
    }
}

[Serializable]
public class LevelData {    

    public string name, description;
    public byte[] imageData;
    public Vector2_S size;
    public Texture2D Snapshot {
        get {
            if (snapShot==null) {
                snapShot = new Texture2D((int)size.x, (int)size.y);
                if (imageData!=null) {
                    snapShot.LoadImage(imageData);
                }
            }
            return snapShot;
        }
    }
    public void SetPicture(byte[] imageData, Vector2 dimensions) {
        this.imageData = imageData;
        this.size = new Vector2_S(dimensions);
        snapShot = new Texture2D((int)dimensions.x, (int)dimensions.y);
        snapShot.LoadImage(imageData);
    }
    public bool IsEmpty { get { return string.IsNullOrEmpty(name); } }

    [NonSerialized] Texture2D snapShot;
    public List<WaveData> waveData = new List<WaveData>();
    public List<TileData> tileData = new List<TileData>();
    public List<PropData> propData = new List<PropData>();

    public int WaveCount { get { return waveData.Count; } }
    public int TileCount { get { return tileData.Count; } }
    public int PropCount { get { return propData.Count; } }
}

[Serializable]
public class WaveData {
    public List<SpawnData> spawnData = new List<SpawnData>();
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