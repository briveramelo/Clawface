// LevelManagerIO.cs
// Author: Aaron

using UnityEngine;
using UnityEngine.Events;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Holds all I/O functionality for the LM.
    /// </summary>
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Public Fields

        /// <summary>
        /// Invoked when a new level is created.
        /// </summary>
        [HideInInspector]
        public UnityEvent onCreateLevel = new UnityEvent();

        /// <summary>
        /// Invoked when a level is loaded.
        /// </summary>
        [HideInInspector]
        public UnityEvent onLoadLevel = new UnityEvent();

        /// <summary>
        /// Invoked when a level is saved.
        /// </summary>
        [HideInInspector]
        public UnityEvent onSaveLevel = new UnityEvent();

        /// <summary>
        /// Invoked when a level is closed.
        /// </summary>
        [HideInInspector]
        public UnityEvent onCloseLevel = new UnityEvent();

        #endregion
        #region Private Fields

        /// <summary>
        /// The currently loaded level in LM.
        /// Because it's serialized, a blank level is automatically created in 
        /// the editor OnEnable. Use _levelLoaded to see if a level is actually
        /// loaded.
        /// </summary>
        [HideInInspector][SerializeField]
        Level loadedLevel = null;

        /// <summary>
        /// Is a level currently loaded?
        /// </summary>
        [HideInInspector][SerializeField]
        bool levelLoaded = false;

        /// <summary>
        /// Resource path of current loaded level.
        /// </summary>
        [HideInInspector][SerializeField]
        string loadedLevelPath = "";

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns true if a level is currently loaded (read-only).
        /// </summary>
        public bool LevelLoaded { get { return levelLoaded; } }

        /// <summary>
        /// Returns the currently loaded level (read-only).
        /// </summary>
        public Level LoadedLevel { get { return loadedLevel; } }

        /// <summary>
        /// Returns the path of the currently loaded level.
        /// </summary>
        public string LoadedLevelPath { get { return loadedLevelPath; } }

        /// <summary>
        /// Sets the name of the loaded level.
        /// </summary>
        public void SetLoadedLevelName(string newName)
        {
            loadedLevel.Name = newName;
            loadedLevelObject.name = newName + " (Loaded Level)";
        }

        /// <summary>
        /// Creates a new level.
        /// </summary>
        public void CreateNewLevel()
        {
            loadedLevel = new Level();
            dirty = false;
            levelLoaded = true;
            SetupLevelObjects();
            InitObjectCounts();
            StartEditing();
            onCreateLevel.Invoke();
        }

        /// <summary>
        /// Saves the current level.
        /// </summary>
        public void SaveCurrentLevelToJSON(bool doUseFilePanel = false)
        {
            if (doUseFilePanel || loadedLevelPath == "")
            {
                string savePath = editor.GetSaveLevelPath();
                if (savePath == default(string) || savePath == "") return;
                JSONFileUtility.SaveToJSONFile(loadedLevel, savePath);
                loadedLevelPath = savePath;
            } 
            
            else JSONFileUtility.SaveToJSONFile(loadedLevel, loadedLevelPath);

            dirty = false;
            onSaveLevel.Invoke();
        }

        /// <summary>
        /// Closes the currently loaded level.
        /// </summary>
        public void CloseLevel()
        {
            StopEditing();
            hoveredObjects.Clear();
            selectedObjects.Clear();

            loadedLevel = null;
            loadedLevelPath = "";
            SelectTool(Tool.Select);

            undoStack.Clear();
            redoStack.Clear();

            CleanupLevel();
            objectCounts.Clear();
            levelLoaded = false;
            onCloseLevel.Invoke();
        }

        /// <summary>
        /// Loads a JSON level file from the given path.
        /// </summary>
        public void LoadLevelFromJSON(string path)
        {
            var asset = JSONFileUtility.LoadFromJSONFile<Level>(path);

            // If asset failed to load
            if (asset == null)
            {
                if (Application.isEditor || Debug.isDebugBuild)
                    Debug.LogError("Failed to load level!");

                return;
            }

            if (loadedLevel != null) CloseLevel();

            editor.LastEditedLevelPath = path;

            loadedLevel = asset;
            levelLoaded = true;
            dirty = false;
            SetupLevelObjects();
            InitObjectCounts();
            GetObjectCounts();
            ReconstructFloor();
            StartEditing();
            onLoadLevel.Invoke();
        }

        /// <summary>
        /// Prompts the user for a level asset to load.
        /// </summary>
        public void LoadLevelFromJSON()
        {
            var assetPath = editor.GetOpenLevelPath();

            // If user cancelled loading
            if (assetPath == default(string) || assetPath == "") return;

            LoadLevelFromJSON(assetPath);
        }

        #endregion
    }
}