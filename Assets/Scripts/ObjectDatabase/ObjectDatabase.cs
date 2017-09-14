// ObjectDatabase.cs
// Author: Aaron

using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Turing.LevelEditor
{
    /// <summary>
    /// Serializable class to hold object data.
    /// </summary>
    [Serializable]
    public sealed class ObjectDatabase
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// All persistent object data.
        /// </summary>
        [SerializeField]
        ObjectData[] data = new ObjectData[(int)byte.MaxValue];

        /// <summary>
        /// Mapping of categories to object data.
        /// </summary>
        [SerializeField]
        Dictionary<Category, List<ObjectData>> categories;

        #endregion
        #region Public Methods

        public ObjectDatabase()
        {
            for (int i = 0; i < (int)byte.MaxValue; i++)
                data[i] = new ObjectData(i);
        }

        /// <summary>
        /// Gets/sets the object data at an index.
        /// </summary>
        public ObjectData this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        /// <summary>
        /// Returns a list of all objects in a category.
        /// </summary>
        public List<ObjectData> AllObjectsInCategory(Category cat)
        {
            if (categories == null) BuildCategories();
            return categories[cat];
        }

        /// <summary>
        /// Reloads all prefabs.
        /// </summary>
        public void ReloadPrefabs()
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].path == ObjectData.DEFAULT_PATH) continue;

                GameObject loaded = null;
                if (Application.isPlaying)
                    loaded = Resources.Load<GameObject>(
                        data[i].path.Substring("Assets/Resources/".Length));

                #if UNITY_EDITOR
                else
                    loaded = AssetDatabase.LoadAssetAtPath<GameObject>(
                        data[i].path);
                #endif

                if (loaded == null)
                {
                    if (Application.isEditor || Debug.isDebugBuild)
                        Debug.LogError("Failed to load prefab at " + 
                            data[i].path);
                    continue;
                }

                data[i].prefab = loaded;
            }
        }

        /// <summary>
        /// Builds all object categories.
        /// </summary>
        public void BuildCategories()
        {
            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log("OBJDB: Building object categories...");

            int uncategorizedObjects = 0;

            // Init categories
            categories = new Dictionary<Category, List<ObjectData>>();
            for (int category = 1; category < (int)Category.COUNT; category++)
                categories.Add((Category)category, new List<ObjectData>());

            // Populate categories
            foreach (var obj in data)
            {
                if (obj.prefab == null) continue;

                if (obj.category == Category.None)
                {
                    if (Application.isEditor || Debug.isDebugBuild)
                        Debug.LogWarning("OBJDB: Asset \"" + obj.prefab.name + 
                            "\" is uncategorized!");
                    uncategorizedObjects++;
                    continue;
                }

                categories[obj.category].Add(obj);
            }

            // Print results
            if (Application.isEditor || Debug.isDebugBuild)
            {
                string result = "OBJDB: Successfully built object categories:";
                for (int i = 1; i < (int)Category.COUNT; i++)
                {
                    Category cat = (Category)i;
                    result += string.Format("\n{0}: {1} objects", cat.ToString(), categories[cat].Count);
                }
                result += string.Format("\n{0} uncategorized objects", uncategorizedObjects);
                Debug.Log(result);
            }
        }

        #endregion
        #region Public Structures

        /// <summary>
        /// Enum for the object filter category.
        /// </summary>
        public enum Category
        {
            None       = 0,
            Block      = 1,
            Trap       = 2,
            Mod        = 3,
            Decoration = 4,
            Light      = 5,
            Special    = 6,
            Effect     = 7,
            Dev        = 8,
            Player     = 9,
            Enemy      = 10,
            Whitebox   = 11,
            COUNT      = 12
        }

        /// <summary>
        /// Enum for object snapping mode in the editor.
        /// </summary>
        public enum SnapMode
        {
            Center,
            Edge,
            Corner
        }

        #endregion
    }
}