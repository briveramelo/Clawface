using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

namespace Turing.LevelEditor
{

    public class LevelObjectDatabase
    {
        #region Public Fields

        public const string LEVELOBJECT_PATH = "/Resources/LevelEditorObjects/";

        #endregion
        #region Private Fields

        List<LevelEditorObject>[] objects;

        string[] categoryNames;

        #endregion
        #region Constructors

        public LevelObjectDatabase(bool doPopulate = true)
        {
            if (doPopulate)
            {
                LoadLevelObjects();
            }
        }

        #endregion
        #region Public Methods

        public string[] GetCategories { get { return categoryNames; } }

        public List<LevelEditorObject> GetObjects (int category)
        {
            return objects[category];
        }

        public void LoadLevelObjects()
        {
            // Get absolute path of level objects (for IO functions)
            string absolutePath = string.Format("{0}{1}", Application.dataPath, 
                LEVELOBJECT_PATH);

            // Get all category names, which are just subfolders
            categoryNames = Directory.GetDirectories(absolutePath);

            // Init array of categories (lists of objects)
            objects = new List<LevelEditorObject>[categoryNames.Length];

            // For each category
            for (int i = 0; i < categoryNames.Length; i++)
            {
                // Trim down to just the folder name
                string folderPath = categoryNames[i];
                string folderName = new DirectoryInfo(folderPath).Name;
                categoryNames[i] = folderName;

                //Debug.Log(string.Format("Folder Name: {0}", folderName));

                // Get absolute and Resources-relative folder paths
                folderName += "/";
                var absFolderPath = string.Format("{0}{1}{2}", 
                    Application.dataPath, LEVELOBJECT_PATH, folderName);
                var relFolderPath = string.Format("{0}{1}", LEVELOBJECT_PATH, 
                    folderName);

                //Debug.Log(string.Format("Absolute Folder Path: {0}", absFolderPath));
                //Debug.Log(string.Format("Relative Folder Path: {0}", relFolderPath));

                // Get all .asset files within the folder
                var files = Directory.GetFiles(absFolderPath, "*.asset");

                //Debug.Log(string.Format("Found {0} asset files in folder.", files.Length));

                // Attempt to load all asset files
                foreach (string assetPath in files)
                {
                    LevelEditorObject asset = LoadAssetPath(assetPath, relFolderPath);

                    if (objects[i] == null)
                        objects[i] = new List<LevelEditorObject>();

                    objects[i].Add(asset);
                }
            }

            //Debug.Log("Finished loading assets.");
        }

        public LevelEditorObject GetObject (string path)
        {
            string[] split = path.Split ('/');
            return GetObject (split[0], split[2]);
        }

        public LevelEditorObject GetObject (int categoryIndex, string name)
        {
            foreach (LevelEditorObject obj in objects[categoryIndex])
            {
                if (obj.Name == name) return obj;
            }

            return null;
        }

        public LevelEditorObject GetObject (string category, string name)
        {
            return GetObject (categoryNames.IndexOf(category), name);
        }

        public LevelEditorObject GetObject (int categoryIndex, int objectIndex)
        {
            return objects[categoryIndex][objectIndex];
        }

        #endregion
        #region Private Methods

        LevelEditorObject LoadAssetPath(string assetPath, string relFolderPath)
        {
            string assetName = Path.GetFileName(assetPath);
            string relAssetPath = default(string);

            LevelEditorObject asset = default(LevelEditorObject);

            if (Application.isPlaying)
            {
                assetName = Path.GetFileNameWithoutExtension (assetName);
                relFolderPath = relFolderPath.Remove (0, "/Resources/".Length);
                relAssetPath = string.Format ("{0}{1}", relFolderPath, assetName);
                //Debug.Log (relAssetPath);
                asset = Resources.Load<LevelEditorObject> (relAssetPath);
            }

            #if UNITY_EDITOR
            else
            {
                relAssetPath = string.Format("Assets{0}{1}", relFolderPath, assetName);
                asset = UnityEditor.AssetDatabase.
                    LoadAssetAtPath<LevelEditorObject>(relAssetPath);
            }
            #endif

            if (asset == default(LevelEditorObject))
            {
                Debug.LogError(string.Format("Failed to load asset at {0}!", assetPath));
                return null;
            }

             //Debug.Log(string.Format("Loaded asset at {0}.", assetPath));

            return asset;
        }

        #endregion
    }
}