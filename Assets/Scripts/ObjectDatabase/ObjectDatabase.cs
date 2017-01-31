﻿// ObjectDatabase.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Serializable class to hold object information.
/// </summary>
[Serializable]
public class ObjectData {

    /// <summary>
    /// Default path to show when object is not used.
    /// </summary>
    public const string DEFAULT_PATH = "UNUSED";

    /// <summary>
    /// Database index of the object.
    /// </summary>
    [SerializeField]
    public int index = -1;

    /// <summary>
    /// Resource path of the object.
    /// </summary>
    [SerializeField]
    public string path = DEFAULT_PATH;

    /// <summary>
    /// Prefab of the object.
    /// </summary>
    [SerializeField]
    public GameObject prefab = null;

    /// <summary>
    /// Category of the object.
    /// </summary>
    [SerializeField]
    public ObjectDatabase.Category category = ObjectDatabase.Category.None;

    /// <summary>
    /// Index constructor.
    /// </summary>
    public ObjectData (int index) {
        this.index = index;
    }
}

/// <summary>
/// Serializable class to hold object data.
/// </summary>
[Serializable]
public class ObjectDatabase {

    #region Enums

    public enum Category {
        None       = 0,
        Block      = 1,
        Trap       = 2,
        Mod        = 3,
        Decoration = 4,
        Light      = 5,
        Special    = 6,
        Effect     = 7,
        Dev        = 8,
        COUNT      = 9
    }

    #endregion
    #region Vars

    [SerializeField]
    ObjectData[] _data = new ObjectData[(int)byte.MaxValue];

    Dictionary<Category, List<ObjectData>> _categories;

    //Texture2D[] _thumbnails = new Texture2D[(int)byte.MaxValue];

    #endregion
    #region Constructors

    public ObjectDatabase() {
        for (int i = 0; i < (int)byte.MaxValue; i++) {
            _data[i] = new ObjectData(i);
        }
    }

    #endregion
    #region Methods

    public ObjectData this[int index] {
        get { return _data[index]; }
        set { _data[index] = value; }
    }

    public List<ObjectData> AllObjectsInCategory (Category cat) {
        if (_categories == null) BuildCategories();
        return _categories[cat];
    }
    
    public void ReloadPrefabs() {
        for (int i = 0; i < _data.Length; i++) {
            if (_data[i].path == ObjectData.DEFAULT_PATH) continue;

            GameObject loaded;
            if (Application.isPlaying)
                loaded = Resources.Load<GameObject>(_data[i].path.Substring("Assets/Resources/".Length));
            else
                loaded = AssetDatabase.LoadAssetAtPath<GameObject>(_data[i].path);

            if (loaded == null) {
                Debug.LogError("Failed to load prefab at " + _data[i].path);
                continue;
            }

            _data[i].prefab = loaded;
        }
    }

    public void BuildCategories () {
        Debug.Log("OBJDB: Building object categories...");

        int uncategorizedObjects = 0;

        // Init categories
        _categories = new Dictionary<Category, List<ObjectData>>();
        for (int category = 1; category < (int)Category.COUNT; category++)
            _categories.Add ((Category)category, new List<ObjectData>());

        // Populate categories
        foreach (var obj in _data) {
            if (obj.prefab == null) continue;

            if (obj.category == Category.None) {
                Debug.LogWarning ("OBJDB: Asset \"" + obj.prefab.name + "\" is uncategorized!");
                uncategorizedObjects++;
                continue;
            }

            _categories[obj.category].Add (obj);
        }

        string result = "OBJDB: Successfully built object categories:";
        for (int i = 1; i < (int)Category.COUNT; i++) {
            Category cat = (Category)i;
            result += string.Format("\n{0}: {1} objects", cat.ToString(), _categories[cat].Count);
        }
        result += string.Format ("\n{0} uncategorized objects", uncategorizedObjects);
        Debug.Log(result);
    }

    /*public void RenderThumbnails () {
        var assetRenderCamera = GameObject.Find ("~AssetRenderCamera").GetComponent<Camera>();
        var renderTexture = new RenderTexture(256, 256, 16);
        renderTexture.Create();
        assetRenderCamera.targetTexture = renderTexture;
        for (int i = 0; i < (int)byte.MaxValue; i++) {
            if (_data[i].prefab != null) {
                var instance = PrefabUtility.InstantiatePrefab(_data[i].prefab);
                _thumbnails[i] = new Texture2D (256, 256);
                //_thumbnails[i].s
                //_thumbnails[i].ReadPixels (new Rect (0,0,256,256),
            }
        }
        renderTexture.Release();
    }*/

    #endregion
}
