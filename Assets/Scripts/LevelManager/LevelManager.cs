// LevelManager.cs
// Author: Aaron

using ModMan;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

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
        #region Vars

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
        /// Current state of the editor tool.
        /// </summary>
        Tool currentTool = Tool.Select;

        /// <summary>
        /// Are there unsaved changes?
        /// </summary>
        bool dirty = false;

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
        /// ID of object being moved.
        /// </summary>
        GameObject movingObject;

        /// <summary>
        /// Original coordinates of object being moved;
        /// </summary>
        Vector3 movingObjectOriginalCoords;

        /// <summary>
        /// 3D asset preview object.
        /// </summary>
        AssetPreview assetPreview;

        /// <summary>
        /// Ghost material for 3D asset preview.
        /// </summary>
        Material previewMaterial;

        /// <summary>
        /// Is object placement allowed at the current location?
        /// </summary>
        bool placementAllowed = true;

        /// <summary>
        /// Index of the currently selected object.
        /// </summary>
        int selectedObjectIndexForPlacement;

        ObjectDatabase.SnapMode selectedObjectSnapMode;

        float currentPlacementYRotation = 0f;

        List<GameObject> hoveredObjects = new List<GameObject>();

        /// <summary>
        /// Currently selected object in editor;
        /// </summary>
        List<GameObject> selectedObjects = new List<GameObject>();

        /// <summary>
        /// Currently applied object filter.
        /// </summary>
        ObjectDatabase.Category filter = ObjectDatabase.Category.Block;

        /// <summary>
        /// List of objects that pass the current filter.
        /// </summary>
        List<ObjectData> filteredObjects;

        /// <summary>
        /// Current editing y position.
        /// </summary>
        int currentYPosition = 0;

        /// <summary>
        /// Is the level currently being played?
        /// </summary>
        bool playingLevel = false;

        /// <summary>
        /// Stack of undo actions.
        /// </summary>
        Stack<LevelEditorAction> undoStack = new Stack<LevelEditorAction>();

        /// <summary>
        /// Stack of redo actions.
        /// </summary>
        Stack<LevelEditorAction> redoStack = new Stack<LevelEditorAction>();

        /// <summary>
        /// Number of objects of each type (to enforce maximums).
        /// </summary>
        [SerializeField]
        List<int> objectCounts;

        public UnityEvent onCreateLevel = new UnityEvent();
        public UnityEvent onLoadLevel = new UnityEvent();
        public UnityEvent onSaveLevel = new UnityEvent();
        public UnityEvent onCloseLevel = new UnityEvent();
        public UnityEvent onSelectObject = new UnityEvent();
        public UnityEvent onDeselectObject = new UnityEvent();

        #endregion
        #region Properties

        /// <summary>
        /// Returns true if the LevelManager is connected to an editor (read-only).
        /// </summary>
        public bool IsConnectedToEditor { get { return editor != null; } }

        /// <summary>
        /// Returns true if unsaved changes exist (read-only).
        /// </summary>
        public bool Dirty { get { return dirty; } }

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
        public ObjectDatabase.Category CurrentObjectFilter { get { return filter; } }

        /// <summary>
        /// Returns the objects that pass the current object filter (read-only).
        /// </summary>
        public List<ObjectData> FilteredObjects { get { return filteredObjects; } }

        /// <summary>
        /// Returns the list of objects that are currently hovered over by the
        /// pointer or selection rect (read-only).
        /// </summary>
        public List<GameObject> HoveredObjects { get { return hoveredObjects; } }

        /// <summary>
        /// Returns the objects that are currently selected (read-only).
        /// </summary>
        public List<GameObject> SelectedObjects { get { return selectedObjects; } }

        /// <summary>
        /// Returns true if an object is currently selected for placement (read-only).
        /// </summary>
        public bool HasSelectedObjectForPlacement
        {
            get { return selectedObjectIndexForPlacement != -1; }
        }

        /// <summary>
        /// Returns the current editing y-value (read-only).
        /// </summary>
        public int CurrentYValue { get { return currentYPosition; } }

        /// <summary>
        /// Returns true if the 3D preview is active (read-only).
        /// </summary>
        public bool PreviewActive { get { return assetPreview != null; } }

        /// <summary>
        /// Returns the preview material (read-only).
        /// </summary>
        public Material PreviewMaterial { get { return previewMaterial; } }

        /// <summary>
        /// Returns the current tool (read-only).
        /// </summary>
        public Tool CurrentTool { get { return currentTool; } }

        /// <summary>
        /// Returns the undo stack (read-only).
        /// </summary>
        public Stack<LevelEditorAction> UndoStack { get { return undoStack; } }

        /// <summary>
        /// Returns the redo stack (read-only).
        /// </summary>
        public Stack<LevelEditorAction> RedoStack { get { return redoStack; } }

        /// <summary>
        /// Returns the editing plane (read-only).
        /// </summary>
        public Plane EditingPlane { get { return editingPlane; } }

        /// <summary>
        /// Returns true if an object is selected (read-only).
        /// </summary>
        public bool HasSelectedObjects { get { return selectedObjects.Count > 0; } }

        /// <summary>
        /// Returns true if an object is being moved (read-only).
        /// </summary>
        public bool IsMovingObject { get { return movingObject != null; } }

        #endregion
        #region Unity Callbacks

        new void Awake()
        {
            base.Awake();

            // Try to reacquire the loaded level on application play
            if (LevelObject.Instance != null)
                loadedLevel = LevelObject.Instance.Level;
            else if (loadedLevelObject != null)
                loadedLevel = loadedLevelObject.GetComponent<LevelObject>().Level;

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
        #region Methods

        /// <summary>
        /// Sets the editor that is currently connected to LevelManager.
        /// </summary>
        public void SetEditor(ILevelEditor editor)
        {
            this.editor = editor;
        }

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
        /// Prepares the LM for editing.
        /// </summary>
        public void StartEditing()
        {
            if (assetPreview != null)
            {
                if (Application.isEditor) DestroyImmediate(assetPreview.gameObject);
                else Destroy(assetPreview.gameObject);
            }

            // OBJDB instance may not be initialized yet, so wait for event
            ObjectDatabaseManager.OnSingletonInitializedEditor.AddListener(() =>
            {
                filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(filter);
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

                if (previewMaterial == null) Debug.LogError("Failed to load preview material!");
            }
        }

        /// <summary>
        /// Sets the dirty status of the LM.
        /// </summary>
        public void SetDirty(bool dirty) { this.dirty = dirty; }

        /// <summary>
        /// Selects the given tool.
        /// </summary>
        public void SelectTool(Tool tool)
        {
            currentTool = tool;
            if (tool != Tool.Place) DisablePreview();
        }

        /// <summary>
        /// Sets the object browser filter.
        /// </summary>
        public void SetObjectFilter(ObjectDatabase.Category filterIndex)
        {
            filter = filterIndex;
            filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(filter);
        }

        public void HoverObject(GameObject obj)
        {
            hoveredObjects.Clear();
            hoveredObjects.Add(obj);
        }

        public List<GameObject> ObjectsInSelection()
        {
            Camera camera = editor.ActiveCamera;
            Rect selectionRect = editor.SelectionRect;
            List<GameObject> result = new List<GameObject>();
            foreach (var spawner in loadedSpawners)
            {
                Vector3 rawScreenPos = camera.WorldToScreenPoint(spawner.transform.position);
                Vector2 screenPos = new Vector2(rawScreenPos.x, rawScreenPos.y);
                if (selectionRect.Contains(screenPos, true)) result.Add(spawner.gameObject);
            }
            return result;
        }


        /// <summary>
        /// Selects an object in the level.
        /// </summary>
        public void SelectObjects(List<GameObject> objects, bool clearFirst = true)
        {
            if (clearFirst) selectedObjects.Clear();
            foreach (var obj in objects)
            {
                // If selected object is not spawner, find through parents
                var spawner = obj.GetComponent<ObjectSpawner>();
                if (spawner == null) spawner = obj.GetComponentInAncestors<ObjectSpawner>();

                if (spawner != null)
                {
                    if (!selectedObjects.Contains(spawner.gameObject))
                    {
                        selectedObjects.Add(spawner.gameObject);
                        onSelectObject.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Deselects an object.
        /// </summary>
        public void DeselectObjects()
        {
            if (selectedObjects != null)
            {
                onDeselectObject.Invoke();
                selectedObjects.Clear();
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
                Debug.LogError("Object " + obj.name + " not found in the level file!");
                return byte.MaxValue;
            }
            return data.Index;
        }

        /// <summary>
        /// Moves the editing plane up by one tile.
        /// </summary>
        public void IncrementY()
        {
            if (currentYPosition < Level.LEVEL_HEIGHT - TILE_UNIT_WIDTH)
            {
                currentYPosition++;
                UpdateHeight();
            }
        }

        /// <summary>
        /// Moves the editing plane down by one tile.
        /// </summary>
        public void DecrementY()
        {
            if (currentYPosition > 0)
            {
                currentYPosition--;
                UpdateHeight();
            }
        }

        /// <summary>
        /// Updates the editing plane according to the current floor and y-value.
        /// </summary>
        public void UpdateHeight()
        {
            editingPlane.SetNormalAndPosition(Vector3.up,
                new Vector3(0f, currentYPosition, 0f));
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
                    case ObjectDatabase.SnapMode.Center:
                        snapped = new Vector3(
                            Mathf.Round(pos.x / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH,
                            Mathf.Round(pos.y / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH,
                            Mathf.Round(pos.z / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH
                        );
                        currentPlacementYRotation = 0f;
                        break;

                    case ObjectDatabase.SnapMode.Corner:
                        closestX = Mathf.Round(pos.x / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH;
                        closestZ = Mathf.Round(pos.z / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH;
                        bool left = closestX >= pos.x;
                        bool down = closestZ >= pos.z;
                        snapped = new Vector3(
                            closestX,
                            currentYPosition,
                            closestZ
                            );
                        currentPlacementYRotation = left ? (down ? 0f : 90f) : (down ? 270f : 180f);
                        float half = TILE_UNIT_WIDTH / 2f;
                        snapped.x += left ? -half : half;
                        snapped.z += down ? -half : half;
                        break;

                    case ObjectDatabase.SnapMode.Edge:
                        closestX = Mathf.Round(pos.x / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH;
                        closestZ = Mathf.Round(pos.z / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH;
                        float distToClosestX = Mathf.Abs(pos.x - closestX);
                        float distToClosestZ = Mathf.Abs(pos.z - closestZ);
                        bool snapToX = distToClosestX <= distToClosestZ;
                        snapped = new Vector3(
                            snapToX ? closestX : Mathf.Floor(pos.x / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH + TILE_UNIT_WIDTH / 2f,
                            Mathf.Round(pos.y / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH,
                            snapToX ? Mathf.Floor(pos.z / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH + TILE_UNIT_WIDTH / 2f : closestZ
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
            if (x < -0.05f || x >= (float)Level.LEVEL_WIDTH * TILE_UNIT_WIDTH + 0.05f) return false;

            // Check y in range
            float y = cursorPosition.y;
            float minY = -0.05f;
            float maxY = Level.LEVEL_HEIGHT * TILE_UNIT_WIDTH + 0.05f;
            if (y < minY || y >= maxY) return false;

            // Check z in range
            float z = cursorPosition.z;
            if (z < -0.05f || z >= (float)Level.LEVEL_DEPTH * TILE_UNIT_WIDTH + 0.05f) return false;

            return true;
        }

        /// <summary>
        /// Resets the currently selected tool (to placement).
        /// </summary>
        public void ResetTool()
        {
            selectedObjectIndexForPlacement = -1;
            DisablePreview();
            currentTool = Tool.Select;
        }

        /// <summary>
        /// Returns true if allowed to place more of the object with the given index.
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
        /// Creates one of the currently selected objects at the cursor.
        /// </summary>
        public void CreateCurrentSelectedObjectAtCursor()
        {
            CreateObject((byte)selectedObjectIndexForPlacement,
                cursorPosition, LevelEditorAction.ActionType.Normal);
        }

        /// <summary>
        /// Crates an object with the given index.
        /// </summary>
        public void CreateObject(byte index, Vector3 position,
            LevelEditorAction.ActionType actionType)
        {
            // Warn if invalid index
            if (index < 0 || index >= byte.MaxValue)
                throw new IndexOutOfRangeException("Invalid index! " + index);

            // Add object to level file
            loadedLevel.AddObject((int)index, position,
                currentPlacementYRotation);

            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log(currentPlacementYRotation);

            // Create spawner
            GameObject spawner = CreateSpawner(index);
            spawner.transform.SetParent(loadedLevelObject.transform);
            spawner.transform.position = position;
            spawner.transform.localRotation = Quaternion.Euler(0f, currentPlacementYRotation, 0f);
            loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
            objectCounts[(int)index]++;
            dirty = true;

            // Update undo/redo stack
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(new CreateObjectAction(index, spawner));
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(new DeleteObjectAction(spawner, index));
                    break;
            }
        }

        /// <summary>
        /// Deletes the given object.
        /// </summary>
        public void DeleteObject(GameObject obj, LevelEditorAction.ActionType actionType)
        {
            // Get database index of object
            byte deletedObjectIndex = (byte)ObjectIndexOfObject(obj);
            int levelIndex = LevelIndexOfObject(obj);

            // Update undo/redo stack
            var deleteAction = new DeleteObjectAction(obj, deletedObjectIndex);
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(deleteAction);
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(new CreateObjectAction(deletedObjectIndex, obj));
                    break;
            }

            // Delete object from level
            loadedLevel.DeleteObject(levelIndex);

            // Delete spawner
            objectCounts[deletedObjectIndex]--;
            loadedSpawners.Remove(obj.GetComponent<ObjectSpawner>());
            DestroyLoadedObject(obj);
            dirty = true;
        }

        /// <summary>
        /// Moves an object.
        /// </summary>
        public void MoveObject(GameObject obj, Vector3 oldPos, Vector3 newPos,
            LevelEditorAction.ActionType actionType)
        {
            var moveAction = new MoveObjectAction(obj, oldPos, newPos);
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(moveAction);
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(new MoveObjectAction(obj, newPos, oldPos));
                    break;
            }

            //var attribs = AttributesOfObject(obj);
            SetObjectPosition(obj, newPos, actionType);
        }

        public void SetObjectPosition(GameObject obj, Vector3 pos,
            LevelEditorAction.ActionType actionType)
        {
            var attribs = AttributesOfObject(obj);
            attribs.SetPosition(pos);
            obj.transform.position = pos;
        }

        public void SetObjectEulerRotation(GameObject obj, Vector3 rot,
            LevelEditorAction.ActionType actionType)
        {
            var attribs = AttributesOfObject(obj);
            attribs.SetEulerRotation(rot);
            obj.transform.localRotation = Quaternion.Euler(rot);
        }

        public void SetObject3DScale(GameObject obj, Vector3 scale,
            LevelEditorAction.ActionType actionType)
        {
            var attribs = AttributesOfObject(obj);
            attribs.Set3DScale(scale);
            obj.transform.localScale = scale;
        }

        public void RecordAttributeChange(GameObject obj,
            ChangeObjectNormalAttributeAction.AttributeChanged attrib,
            Vector3 oldValue, Vector3 newValue, LevelEditorAction.ActionType actionType)
        {
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(new ChangeObjectNormalAttributeAction(obj, attrib, newValue));
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(new ChangeObjectNormalAttributeAction(obj, attrib, oldValue));
                    break;
            }
        }

        /// <summary>
        /// Undoes the most recent action.
        /// </summary>
        public void Undo()
        {
            if (undoStack.Count <= 0) return;

            var undoAction = undoStack.Pop();
            undoAction.Undo();
        }

        /// <summary>
        /// Redoes the most recent action.
        /// </summary>
        public void Redo()
        {
            if (redoStack.Count <= 0) return;

            var redoAction = redoStack.Pop();
            redoAction.Redo();
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
        /// Starts moving an object.
        /// </summary>
        public void StartMovingObject(GameObject obj)
        {
            ShowAssetPreview(obj);
            movingObjectOriginalCoords = obj.transform.position;
            movingObject = obj;
            //DeleteObject(obj, ActionType.None);
        }

        /// <summary>
        /// Stops moving an object.
        /// </summary>
        public void StopMovingObject()
        {
            if (placementAllowed)
            {
                //CreateObject((byte)_movingObjectID, _cursorPosition, ActionType.None);
                //_movingObject.transform.position = _cursorPosition;
                MoveObject(movingObject, movingObjectOriginalCoords,
                    cursorPosition, LevelEditorAction.ActionType.Normal);
                ResetMovingObject();
            }
        }

        /// <summary>
        /// Resets the object being moved.
        /// </summary>
        public void ResetMovingObject()
        {
            //CreateObject((byte)_movingObjectID, _movingObjectOriginalCoords, ActionType.None);
            //_movingObjectID = -1;
            movingObject = null;
            DisablePreview();
        }

        /// <summary>
        /// Selects an asset from the object browser.
        /// </summary>
        /// <param name="index">Index of the object in the browser.</param>
        public void SelectObjectInCategory(int index,
            ObjectDatabase.Category category)
        {
            var data = ObjectDatabaseManager.Instance.AllObjectsInCategory(category)[index];

            selectedObjectIndexForPlacement = data.index;
            selectedObjectSnapMode = data.snapMode;
            var obj = ObjectDatabaseManager.Instance.GetObject(data.index);

            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log(string.Format("Object {0} ({1}) selected from category {2}.",
                    obj.name, index.ToString(), category.ToString()));

            ShowAssetPreview(obj);
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
                spawner.transform.rotation = Quaternion.Euler(attribs.EulerRotation);
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
        /// Creates a spawner for the given template.
        /// </summary>
        public GameObject CreateSpawner(GameObject template)
        {
            GameObject spawner = new GameObject(template.name +
                " Spawner", typeof(ObjectSpawner));
            spawner.GetComponent<ObjectSpawner>().SetTemplate(template);
            return spawner;
        }

        /// <summary>
        /// Returns a new instance of the object with the given index.
        /// </summary>
        public GameObject CreateSpawner(byte index)
        {
            var template = ObjectDatabaseManager.Instance.GetObject(index);
            return CreateSpawner(template);
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

        /// <summary>
        /// Uses the proper function to destroy the given object.
        /// </summary>
        public void DestroyLoadedObject(GameObject obj)
        {
            if (Application.isEditor) DestroyImmediate(obj);
            else Destroy(obj);
        }

        /// <summary>
        /// Starts playing the currently loaded level.
        /// </summary>
        public void PlayCurrentLevel()
        {
            //Debug.Log("Play");

            // Activate all spawners
            foreach (ObjectSpawner spawner in loadedSpawners)
                spawner.Play();

            playingLevel = true;
        }

        /// <summary>
        /// Stops playing the currently loaded level.
        /// </summary>
        public void StopPlayingLevel()
        {
            //Debug.Log("Stop");

            // Reset all spawners
            foreach (var spawner in loadedSpawners)
                spawner.ResetSpawner();

            playingLevel = false;
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

        [Serializable]
        public class ByteToIntDict : Dictionary<byte, int> { }

        #endregion
    }
}