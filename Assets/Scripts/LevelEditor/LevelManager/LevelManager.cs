// LevelManager.cs
// Author: Aaron

using ModMan;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Class to handle all level editing functionality.
    /// Both the editor window and in-game editor use this manager
    /// for consistency.
    /// </summary>
    [ExecuteInEditMode]
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Number of objects of each type (to enforce maximums).
        /// </summary>
        [SerializeField] List<int> objectCounts;

        #endregion
        #region Private Fields

        /// <summary>
        /// The interface that is currently using LM.
        /// </summary>
        ILevelEditor editor;

        /// <summary>
        /// GameObject representing the loaded level.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        GameObject loadedLevelObject;

        /// <summary>
        /// All currently loaded objects in the editor.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        List<ObjectSpawner> loadedSpawners =
            new List<ObjectSpawner>();

        /// <summary>
        /// Plane representing current editing y-level.
        /// </summary>
        Plane editingPlane = new Plane(Vector3.up, Vector3.zero);

        /// <summary>
        /// Show the placement grid?
        /// </summary>
        bool drawGrid = true;

        /// <summary>
        /// Snap objects to grid?
        /// </summary>
        bool snapToGrid = true;

        /// <summary>
        /// 3D asset preview object.
        /// </summary>
        AssetPreview assetPreview;

        /// <summary>
        /// Ghost material for 3D asset preview.
        /// </summary>
        Material previewMaterial;

        /// <summary>
        /// Currently applied object filter.
        /// </summary>
        ObjectDatabase.Category filter = ObjectDatabase.Category.Block;

        /// <summary>
        /// List of objects that pass the current filter.
        /// </summary>
        List<ObjectData> filteredObjects;

        #endregion
        #region Unity Lifecycle

        new void Awake()
        {
            base.Awake();

            // Try to reacquire the loaded level on application play
            if (LevelObject.Instance != null)
                loadedLevel = LevelObject.Instance.Level;

            else if (loadedLevelObject != null)
                loadedLevel = loadedLevelObject.
                    GetComponent<LevelObject>().Level;

            selectedObjects.Clear();
            hoveredObjects.Clear();

            if (Application.isPlaying) AwakePlayer();
            else AwakeEditor();
        }

        void OnApplicationQuit()
        {
            if (Application.isPlaying && playingLevel)
                StopPlayingLevel();
        }

        new void OnDestroy()
        {
            base.OnDestroy();

            selectedObjects.Clear();
            hoveredObjects.Clear();
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns true if the LevelManager is connected to an editor 
        /// (read-only).
        /// </summary>
        public bool IsConnectedToEditor { get { return editor != null; } }

        /// <summary>
        /// Returns true if the grid is currently enabled (read-only).
        /// </summary>
        public bool GridEnabled { get { return drawGrid; } }

        /// <summary>
        /// Gets/sets whether or not objects snap to the grid.
        /// </summary>
        public bool SnapToGrid
        {
            get { return snapToGrid; }
            set { snapToGrid = value; }
        }

        /// <summary>
        /// Returns the currently selected object filter (read-only).
        /// </summary>
        public ObjectDatabase.Category CurrentObjectFilter
        { get { return filter; } }

        /// <summary>
        /// Returns the objects that pass the current object filter 
        /// (read-only).
        /// </summary>
        public List<ObjectData> FilteredObjects
        { get { return filteredObjects; } }

        /// <summary>
        /// Returns true if the 3D preview is active (read-only).
        /// </summary>
        public bool PreviewActive { get { return assetPreview != null; } }

        /// <summary>
        /// Returns the preview material (read-only).
        /// </summary>
        public Material PreviewMaterial { get { return previewMaterial; } }

        /// <summary>
        /// Returns the editing plane (read-only).
        /// </summary>
        public Plane EditingPlane { get { return editingPlane; } }

        /// <summary>
        /// Sets the editor that is currently connected to LevelManager.
        /// </summary>
        public void SetEditor(ILevelEditor editor)
        {
            this.editor = editor;
        }

        /// <summary>
        /// Prepares the LM for editing.
        /// </summary>
        public void StartEditing()
        {
            if (assetPreview != null)
            {
                if (Application.isEditor)
                    DestroyImmediate(assetPreview.gameObject);

                else Destroy(assetPreview.gameObject);
            }

            // OBJDB instance may not be initialized yet, so wait for event
            ObjectDatabaseManager.OnSingletonInitializedEditor.
                AddListener(() =>
            {
                filteredObjects = ObjectDatabaseManager.Instance.
                AllObjectsInCategory(filter);
            });

            filter = ObjectDatabase.Category.Block;
            CreateAssetPreview();
            selectedObjectIndexForPlacement = -1;
            currentYPosition = 0;
            currentPlacementYRotation = 0f;
            editingPlane = new Plane(Vector3.up, Vector3.zero);
        }

        /// <summary>
        /// Stops the LM from editing.
        /// </summary>
        public void StopEditing()
        {
            if (assetPreview != null)
                DestroyImmediate(assetPreview.gameObject);
        }

        /// <summary>
        /// Sets the object browser filter.
        /// </summary>
        public void SetObjectFilter(ObjectDatabase.Category filterIndex)
        {
            filter = filterIndex;
            filteredObjects = ObjectDatabaseManager.Instance.
                AllObjectsInCategory(filter);
        }

        /// <summary>
        /// Gets the attributes of the given object.
        /// </summary>
        public ObjectAttributes AttributesOfObject(GameObject obj)
        {
            var index = LevelIndexOfObject(obj);
            return loadedLevel.Objects[index];
        }

        /// <summary>
        /// Gets the index in the level of the given object.
        /// </summary>
        public int LevelIndexOfObject(GameObject obj)
        {
            return loadedSpawners.IndexOf(obj.GetComponent<ObjectSpawner>());
        }

        /// <summary>
        /// Gets the database index of the given object.
        /// </summary>
        public byte ObjectIndexOfObject(GameObject obj)
        {
            var index = LevelIndexOfObject(obj);
            var data = loadedLevel.Objects[index];
            if (data == null)
            {
                Debug.LogError("Object " + obj.name + 
                    " not found in the level file!");
                return byte.MaxValue;
            }
            return data.Index;
        }

        /// <summary>
        /// Snaps the preview asset to the grid.
        /// </summary>
        public void SnapCursor()
        {
            Vector3 pos = cursorPosition;
            Vector3 snapped = pos;
            if (selectedObjectIndexForPlacement != -1)
            {
                float closestX;
                float closestZ;

                switch (selectedObjectSnapMode)
                {
                    // Center snapping
                    case ObjectDatabase.SnapMode.Center:
                        snapped = new Vector3(
                            Mathf.Round(pos.x / TILE_UNIT_WIDTH),
                            Mathf.Round(pos.y / TILE_UNIT_WIDTH),
                            Mathf.Round(pos.z / TILE_UNIT_WIDTH)
                        ) * TILE_UNIT_WIDTH;
                        currentPlacementYRotation = 0f;
                        break;

                    // Corner snapping
                    case ObjectDatabase.SnapMode.Corner:
                        closestX = Mathf.Round(pos.x / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH;
                        closestZ = Mathf.Round(pos.z / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH;
                        bool left = closestX >= pos.x;
                        bool down = closestZ >= pos.z;
                        snapped = new Vector3(
                            closestX,
                            currentYPosition,
                            closestZ
                            );
                        currentPlacementYRotation = left ? (down ? 0f : 90f) 
                            : (down ? 270f : 180f);
                        float half = TILE_UNIT_WIDTH / 2f;
                        snapped.x += left ? -half : half;
                        snapped.z += down ? -half : half;
                        break;

                    // Edge snapping
                    case ObjectDatabase.SnapMode.Edge:
                        closestX = Mathf.Round(pos.x / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH;
                        closestZ = Mathf.Round(pos.z / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH;
                        float distToClosestX = Mathf.Abs(pos.x - closestX);
                        float distToClosestZ = Mathf.Abs(pos.z - closestZ);
                        bool snapToX = distToClosestX <= distToClosestZ;
                        snapped = new Vector3(
                            snapToX ? closestX : 
                            Mathf.Floor(pos.x / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH + TILE_UNIT_WIDTH / 2f,

                            Mathf.Round(pos.y / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH,

                            snapToX ? Mathf.Floor(pos.z / TILE_UNIT_WIDTH) 
                            * TILE_UNIT_WIDTH + TILE_UNIT_WIDTH / 2f : 
                            closestZ
                            );
                        currentPlacementYRotation = snapToX ? 0f : 90f;
                        break;
                }

                cursorPosition = snapped;
            }
        }

        /// <summary>
        /// Checks the placement of the preview asset to see if it is valid.
        /// </summary>
        public bool CheckPlacement()
        {
            // Check x in range
            float x = cursorPosition.x;
            if (x < -0.05f || x >= (float)Level.LEVEL_WIDTH * 
                TILE_UNIT_WIDTH + 0.05f) return false;

            // Check y in range
            float y = cursorPosition.y;
            float minY = -0.05f;
            float maxY = Level.LEVEL_HEIGHT * TILE_UNIT_WIDTH + 0.05f;
            if (y < minY || y >= maxY) return false;

            // Check z in range
            float z = cursorPosition.z;
            if (z < -0.05f || z >= (float)Level.LEVEL_DEPTH * 
                TILE_UNIT_WIDTH + 0.05f) return false;

            return true;
        }

        /// <summary>
        /// Returns true if allowed to place more of the object with the given 
        /// index.
        /// </summary>
        public bool CanPlaceAnotherObject(int index)
        {
            var limit = ObjectDatabaseManager.Instance.GetObjectLimit(index);
            if (limit < 0) return true;

            else return objectCounts[(byte)index] < limit;
        }

        /// <summary>
        /// Returns true if allowed to place more of the currently selected object.
        /// </summary>
        public bool CanPlaceAnotherCurrentObject()
        {
            return CanPlaceAnotherObject(selectedObjectIndexForPlacement);
        }

        /// <summary>
        /// Gets the object attributes of the given object.
        /// </summary>
        public ObjectAttributes GetAttributesOfObject(GameObject obj)
        {
            var index = loadedSpawners.IndexOf(obj.GetComponent<ObjectSpawner>());
            var data = loadedLevel.Objects[index];
            return data;
        }

        /// <summary>
        /// Enables the grid.
        /// </summary>
        public void EnableGrid()
        {
            drawGrid = true;
        }

        /// <summary>
        /// Disables the grid.
        /// </summary>
        public void DisableGrid()
        {
            drawGrid = false;
        }

        /// <summary>
        /// Selects an asset from the object browser.
        /// </summary>
        /// <param name="index">Index of the object in the browser.</param>
        public void SelectObjectInCategory(int index,
            ObjectDatabase.Category category)
        {
            var data = ObjectDatabaseManager.Instance.AllObjectsInCategory
                (category)[index];

            selectedObjectIndexForPlacement = data.index;
            selectedObjectSnapMode = data.snapMode;
            var obj = ObjectDatabaseManager.Instance.GetObject(data.index);

            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log(string.Format(
                    "Object {0} ({1}) selected from category {2}.",
                    obj.name, index.ToString(), category.ToString()));

            ShowAssetPreview(obj);
        }
        
        /// <summary>
        /// Uses the proper function to destroy the given object.
        /// </summary>
        public void DestroyLoadedObject(GameObject obj)
        {
            if (Application.isEditor) DestroyImmediate(obj);
            else Destroy(obj);
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Called on awake in the player.
        /// </summary>
        void AwakePlayer()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Called on awake in the editor.
        /// </summary>
        void AwakeEditor()
        {
        }

        /// <summary>
        /// Initializes the 3D asset preview.
        /// </summary>
        void CreateAssetPreview()
        {
            assetPreview = new GameObject(PREVIEW_NAME,
                typeof(AssetPreview)).GetComponent<AssetPreview>();
            //_preview.hideFlags = HideFlags.HideAndDontSave;

            // Load preview material
            if (previewMaterial == null)
            {
                previewMaterial = editor.PreviewMaterial;

                if (previewMaterial == null)
                    Debug.LogError("Failed to load preview material!");
            }
        }

        /// <summary>
        /// Shows a 3D preview of the currently selected asset.
        /// </summary>
        void ShowAssetPreview(GameObject obj)
        {
            if (obj == null)
                throw new System.NullReferenceException("No asset given!");

            assetPreview.GetComponent<AssetPreview>().SetPreviewObject(obj);
            assetPreview.Show();
        }

        /// <summary>
        /// Hides the 3D asset preview.
        /// </summary>
        void DisablePreview()
        {
            if (assetPreview == null) return;
            assetPreview.Hide();
        }

        /// <summary>
        /// Spawns objects from the currently loaded level.
        /// </summary>
        void ReconstructFloor()
        {
            var objects = loadedLevel.Objects;
            for (int i = 0; i < objects.Length; i++)
            {
                var attribs = objects[i];
                byte index;

                try
                {
                    index = attribs.Index;
                } catch (NullReferenceException)
                {
                    continue;
                }

                // Skip empty objects
                if (index == byte.MaxValue) continue;

                // Create spawner
                var spawner = CreateSpawner(index);
                spawner.transform.parent = loadedLevelObject.transform;
                spawner.transform.position = attribs.Position;
                spawner.transform.rotation = 
                    Quaternion.Euler(attribs.EulerRotation);
                spawner.transform.localScale = attribs.Scale;
                loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
            }
        }

        /// <summary>
        /// Creates base level objects.
        /// </summary>
        void SetupLevelObjects()
        {
            loadedLevelObject = new GameObject(loadedLevel.Name +
                " (Loaded Level)", typeof(LevelObject));
        }

        /// <summary>
        /// Counts the number of each object in the scene.
        /// </summary>
        void GetObjectCounts()
        {
            foreach (var obj in loadedSpawners)
            {
                var index = ObjectIndexOfObject(obj.Template);
                objectCounts[index]++;
            }
        }

        /// <summary>
        /// Resets the object counter.
        /// </summary>
        void InitObjectCounts()
        {
            if (objectCounts == null)
                objectCounts = new List<int>();

            for (int i = 0; i < (int)byte.MaxValue; i++)
            {
                if (i >= objectCounts.Count) objectCounts.Add(0);
                else objectCounts[i] = 0;
            }
        }

        /// <summary>
        /// Removes all spawners from the current floor.
        /// </summary>
        void CleanupFloor()
        {
            while (loadedSpawners.Count > 0)
            {
                if (loadedSpawners[0] == null ||
                    loadedSpawners[0].gameObject == null)
                {
                    loadedSpawners.RemoveAt(0);
                    continue;
                }

                DestroyLoadedObject(loadedSpawners.PopFront().gameObject);
            }
        }

        /// <summary>
        /// Cleans up the floor and level.
        /// </summary>
        void CleanupLevel()
        {
            CleanupFloor();

            if (loadedLevelObject != null)
            {
                DestroyLoadedObject(loadedLevelObject);
                loadedLevelObject = null;
            }
        }

        #endregion
        #region Public Structures

        /// <summary>
        /// Enum for the current tool mode of the editor.
        /// </summary>
        public enum Tool
        {
            Select = 0,
            Place = 1,
            Erase = 2,
            Move = 3
        }

        #endregion
    }
}