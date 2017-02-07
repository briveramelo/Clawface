using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class ObjectDatabaseManager : SingletonMonoBehaviour<ObjectDatabaseManager> {

    [SerializeField]
	ObjectDatabase _database = new ObjectDatabase();

    new void Awake() {
        base.Awake();
        
        if (Application.isPlaying)
            DontDestroyOnLoad (gameObject);
    }

    public GameObject GetObject (int index) {
        if (index < 0 || index >= (int)byte.MaxValue) {
            Debug.LogError ("Index out of range! " + index.ToString());
            return null;
        }
        return _database[index].prefab;
    }

    public int GetObjectLimit (int index) {
        return _database[index].limit;
    }

    public List<ObjectData> AllObjectsInCategory (ObjectDatabase.Category cat) {
        return _database.AllObjectsInCategory (cat);
    }

    public void RebuildCategories () {
        _database.BuildCategories();
    }

    public void SaveToJSON () {
        var dbAsJSON = JsonUtility.ToJson (_database);
        var path = EditorUtility.SaveFilePanel ("Save database file", Application.dataPath, "ObjectDatabase", "json");
        if (path == default(string) || path == "") return;
        if (!File.Exists(path)) File.CreateText(path).Write (dbAsJSON);
        else File.WriteAllText (path, dbAsJSON);
    }

    public void LoadFromJSON () {
        var path = EditorUtility.OpenFilePanel ("Open database file", Application.dataPath, "json");
        if (path == default(string) || path == "") return;
        var newDB = JsonUtility.FromJson<ObjectDatabase> (File.ReadAllText(path));
        if (newDB == null) {
            Debug.LogError ("Failed to load database.");
            return;
        }

        _database = newDB;
        newDB.ReloadPrefabs();
        newDB.BuildCategories();
    }
}
