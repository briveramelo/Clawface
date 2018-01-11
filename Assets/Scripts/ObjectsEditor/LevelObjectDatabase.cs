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

        const string THUMBNAILS_FOLDER_NAME = "Thumbnails";
        const int THUMBNAIL_WIDTH = 1024;
        const int THUMBNAIL_HEIGHT = 1024;

        #endregion
        #region Private Fields

        List<KeyValuePair<string, LevelEditorObject>>[] objects;

        string[] categoryNames;
        string[] fancyCategoryNames;

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

        public string[] GetFancyCategories { get { return fancyCategoryNames; } }

        public List<KeyValuePair<string, LevelEditorObject>> GetObjects (int category)
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
            fancyCategoryNames = new string[categoryNames.Length];

            // Init array of categories (lists of objects)
            objects = new List<KeyValuePair<string, LevelEditorObject>>[categoryNames.Length];

            // For each category
            for (int i = 0; i < categoryNames.Length; i++)
            {
                // Trim down to just the folder name
                string folderPath = categoryNames[i];
                string folderName = new DirectoryInfo(folderPath).Name;
                categoryNames[i] = folderName;
                fancyCategoryNames[i] = StringExtension.AddSpacesBetweenUpperCase(folderName);

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
                    if (asset == null) continue;
                    string assetName = Path.GetFileNameWithoutExtension (assetPath);

                    if (objects[i] == null)
                        objects[i] = new List<KeyValuePair<string, LevelEditorObject>>();

                    objects[i].Add(new KeyValuePair<string, LevelEditorObject>(assetName, asset));
                }
            }

            GenerateThumbnails();

            //Debug.Log("Finished loading assets.");
        }

        public LevelEditorObject GetObject (string path)
        {
            string[] split = path.Split ('/');
            if (split.Length < 2)
            {
                Debug.LogError (string.Format("Failed to get object at {0}!", path));
                return null;
            }
            return GetObject (split[0], split[1].Replace(" ", ""));
        }

        public LevelEditorObject GetObject (int categoryIndex, string name)
        {
            foreach (KeyValuePair<string, LevelEditorObject> obj in objects[categoryIndex])
            {
                if (obj.Key == name) return obj.Value;
            }

            Debug.LogError (string.Format("Failed to get object {0} in category {1}!", name, categoryIndex));

            return null;
        }

        public LevelEditorObject GetObject (string category, string name)
        {
            return GetObject (categoryNames.IndexOf(category), name);
        }

        public LevelEditorObject GetObject (int categoryIndex, int objectIndex)
        {
            return objects[categoryIndex][objectIndex].Value;
        }

        #endregion
        #region Private Methods

        LevelEditorObject LoadAssetPath(string assetPath, string relFolderPath)
        {
            string assetName = Path.GetFileName(assetPath);
            string relAssetPath = default(string);
            string trimmedPath = default(string);

            LevelEditorObject asset = default(LevelEditorObject);

            if (Application.isPlaying)
            {
                assetName = Path.GetFileNameWithoutExtension (assetName);
                relFolderPath = relFolderPath.Remove (0, "/Resources/".Length);
                relAssetPath = string.Format ("{0}{1}", relFolderPath, assetName);
                //Debug.Log (relAssetPath);
                asset = Resources.Load<LevelEditorObject> (relAssetPath);
                //asset.SetPath(relAssetPath);
            }

            #if UNITY_EDITOR
            else
            {
                relAssetPath = string.Format("Assets{0}{1}", relFolderPath, assetName);
                asset = UnityEditor.AssetDatabase.
                    LoadAssetAtPath<LevelEditorObject>(relAssetPath);
                trimmedPath = relAssetPath.Remove(0, string.Format("Assets{0}", LEVELOBJECT_PATH).Length);
                trimmedPath = trimmedPath.Remove (trimmedPath.IndexOf (".asset"), ".asset".Length);
            }
            #endif

            if (asset == default(LevelEditorObject))
            {
                Debug.LogError(string.Format("Failed to load asset at {0}!", assetPath));
                return null;
            }

            asset.SetPath(trimmedPath);

            // Check prefab
            GameObject prefab = asset.Prefab;
            if (prefab == null)
            {
                Debug.LogWarning (string.Format("Asset {0} does not have a prefab!", asset));
                return null;
            }

            // Check splattable
            if (asset.NeedsSplattable)
            {
                Splattable splattable = asset.Prefab.
                    GetComponentInChildren<Splattable>();
                if (splattable == null)
                {
                    Debug.LogWarning (string.Format(@"Asset {0} does not have 
                        a Splattable! If it doesn't need one, you can disable
                        this check in the LevelEditorObject asset.", 
                        asset.Prefab.name), asset);
                }
            }

            return asset;
        }

        void GenerateThumbnails ()
        {
            RenderTexture renderTexture = new RenderTexture(THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT, 
                0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            Camera renderCamera = new GameObject("~RenderCamera").AddComponent<Camera>();
            renderCamera.targetTexture = renderTexture;
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.clear;

            for (int categoryIndex = 0; categoryIndex < objects.Length; categoryIndex++)
            {
                List<KeyValuePair<string, LevelEditorObject>> category = objects[categoryIndex];
                for (int objectPairIndex = 0; objectPairIndex < category.Count; objectPairIndex++)
                {
                    KeyValuePair<string, LevelEditorObject> objectPair = category[objectPairIndex];
                    LevelEditorObject leObject = objectPair.Value;
                    GenerateThumbnail(renderCamera, leObject);
                }
            }

            renderCamera.targetTexture = null;
            MonoBehaviour.DestroyImmediate (renderTexture);
            MonoBehaviour.DestroyImmediate (renderCamera.gameObject);
        }

        void GenerateThumbnail (Camera camera, LevelEditorObject leObject)
        {
            GameObject instance = InstantiateObjectHelper.InstantiateObject (leObject.Prefab);
            instance.transform.position = camera.transform.position + camera.transform.forward * 5.0f;
            camera.Render();

            RenderTexture.active = camera.targetTexture;
            Texture2D thumbmail = new Texture2D (THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT);
            thumbmail.ReadPixels(new Rect (0, 0, THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT), 0, 0);
            RenderTexture.active = null;

            leObject.SetThumbnail (thumbmail);

            byte[] encodedThumbnail = thumbmail.EncodeToPNG();
            string folderPath = string.Format("{0}/{1}", Application.dataPath, THUMBNAILS_FOLDER_NAME);
            if (!System.IO.Directory.Exists (folderPath))
                System.IO.Directory.CreateDirectory (folderPath);
            string path = string.Format("{0}/{1}.png", folderPath, leObject.Name);
            System.IO.File.WriteAllBytes (path, encodedThumbnail);

            MonoBehaviour.DestroyImmediate (instance);
        }

        #endregion
    }
}