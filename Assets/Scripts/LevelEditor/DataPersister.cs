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

    private void Awake() {
        Load(0);
    }
    public void Load(int levelIndex) {
        ActiveDataSave = null;
        if (!Directory.Exists(PathDirectory)) {
            Directory.CreateDirectory(PathDirectory);
        }
        if (File.Exists(FilePath)) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fileStream = File.Open(FilePath, FileMode.Open);
                ActiveDataSave = new DataSave((DataSave)bf.Deserialize(fileStream));
                fileStream.Close();
            }
            catch {
                File.Delete(FilePath);
                ActiveDataSave = new DataSave();
            }
        }
        else {
            ActiveDataSave = new DataSave();
        }
        ActiveDataSave.SelectedIndex = levelIndex;
    }

    public void TrySave() {
        Save();
    }

    void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(FilePath);
        bf.Serialize(fileStream, new DataSave(ActiveDataSave));
        fileStream.Close();
    }
}

#region Memory Structures

[Serializable]
public class DataSave {
    public DataSave() { }
    public DataSave(DataSave dataSave) {
        this.levelDatas = dataSave.levelDatas;
    }

    public LevelData ActiveLevelData { get { if (levelDatas.Count==0) { levelDatas.Add(new LevelData()); } return levelDatas[SelectedIndex]; } }
    public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(1, levelDatas.Count - 1)); } }

    [SerializeField] List<LevelData> levelDatas = new List<LevelData>();
    int selectedIndex = 0;
}

[Serializable]
public class LevelData {
    public string name;
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