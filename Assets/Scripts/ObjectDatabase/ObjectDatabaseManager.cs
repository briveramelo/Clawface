// ObjectDatabaseManager.cs
// Author: Aaron

using System.Collections.Generic;
using System.IO;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Turing.LevelEditor
{
    /// <summary>
    /// Manager to store object database data.
    /// </summary>
    [ExecuteInEditMode]
    public class ObjectDatabaseManager :
        EditorSingleton<ObjectDatabaseManager>
    {
        #region Vars

        /// <summary>
        /// Object info database.
        /// </summary>
        [SerializeField]
        ObjectDatabase database = new ObjectDatabase();

        #endregion
        #region Unity Callbacks

        new void Awake() 
        {
            base.Awake();

            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Returns the object with the given index.
        /// </summary>
        public GameObject GetObject(int index) 
        {
            if (index < 0 || index >= (int)byte.MaxValue) 
            {
                if (Application.isEditor || Debug.isDebugBuild)
                    Debug.LogError("Index out of range! " + index.ToString());
                return null;
            }

            return database[index].prefab;
        }

        /// <summary>
        /// Returns the limit for objects with the given index.
        /// </summary>
        public int GetObjectLimit(int index) 
        {
            return database[index].limit;
        }

        /// <summary>
        /// Returns a list of all objects in a given category.
        /// </summary>
        public List<ObjectData> AllObjectsInCategory
            (ObjectDatabase.Category cat) 
        {
            return database.AllObjectsInCategory(cat);
        }

        /// <summary>
        /// Builds categories.
        /// </summary>
        public void RebuildCategories() 
        {
            database.BuildCategories();
        }

        //public void AddObject () {
        //    _database.AddObject();
        //}

        /// <summary>
        /// Saves the database to a JSON file.
        /// </summary>
        public void SaveToJSON() 
        {
            #if UNITY_EDITOR

            // Serialize database to JSON
            var dbAsJSON = JsonUtility.ToJson(database);

            var path = EditorUtility.SaveFilePanel("Save database file", 
                Application.dataPath, "ObjectDatabase", "json");

            if (path == default(string) || path == "") return;

            if (!File.Exists(path)) File.CreateText(path).Write(dbAsJSON);
            else File.WriteAllText(path, dbAsJSON);

            #endif
        }

        /// <summary>
        /// Loads database info from a JSON file.
        /// </summary>
        public void LoadFromJSON() 
        {
            #if UNITY_EDITOR

            var path = EditorUtility.OpenFilePanel("Open database file", Application.dataPath, "json");
            if (path == default(string) || path == "") return;
            var newDB = JsonUtility.FromJson<ObjectDatabase>(File.ReadAllText(path));
            if (newDB == null) {
                Debug.LogError("Failed to load database.");
                return;
            }

            database = newDB;
            newDB.ReloadPrefabs();
            newDB.BuildCategories();

            #endif
        }

        #endregion
    }
}