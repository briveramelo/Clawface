// LevelManager.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

/// <summary>
/// Class to handle all level editing functionality.
/// Both the editor window and in-game editor use this manager
/// for consistency.
/// </summary>
[ExecuteInEditMode]
public class LevelManager : SingletonMonoBehaviour<LevelManager> {

    #region Enums

    /// <summary>
    /// Enum for the current tool mode of the editor.
    /// </summary>
    public enum Tool {
        Select = 0,
        Place = 1,
        Erase = 2,
        Move = 3
    }

    #endregion
    #region Constants

    /// <summary>
    /// Default level path.
    /// </summary>
    const string _LEVEL_PATH = "Assets/Resources/Levels/";

    /// <summary>
    /// Editor name of the 3D asset preview object.
    /// </summary>
    const string _PREVIEW_NAME = "~LevelEditorPreview";

    #endregion
    #region Vars

    /// <summary>
    /// The currently loaded level in LM.
    /// !!! KEEP NONSERIALIZED !!!
    /// </summary>
    [NonSerialized]
    Level _loadedLevel;

    /// <summary>
    /// Resource path of current loaded level.
    /// </summary>
    string _loadedLevelPath = "";

    /// <summary>
    /// GameObject representing the loaded level.
    /// </summary>
    GameObject _loadedLevelObject;

    /// <summary>
    /// GameObjects representing the floors of the loaded level.
    /// </summary>
    GameObject[] _floorObjects;

    /// <summary>
    /// All currently loaded objects in the editor.
    /// </summary>
    List<ObjectSpawner> _loadedSpawners = new List<ObjectSpawner>();

    /// <summary>
    /// Current state of the editor tool.
    /// </summary>
    Tool _currentTool = Tool.Select;

    /// <summary>
    /// Are there unsaved changes?
    /// </summary>
    bool _dirty = false;

    /// <summary>
    /// Index of floor that is currently being edited.
    /// </summary>
    int _selectedFloor = 0;

    /// <summary>
    /// Plane representing current editing y-level.
    /// </summary>
    Plane _editingPlane = new Plane(Vector3.up, Vector3.zero);

    /// <summary>
    /// Show the placement grid?
    /// </summary>
    bool _drawGrid = true;

    /// <summary>
    /// Snap objects to grid?
    /// </summary>
    bool _snapToGrid = true;

    /// <summary>
    /// ID of object being moved.
    /// </summary>
    int _movingObjectID = -1;

    /// <summary>
    /// Original coordinates of object being moved;
    /// </summary>
    Vector3 _movingObjectOriginalCoords;

    /// <summary>
    /// 3D asset preview object.
    /// </summary>
    GameObject _preview;

    /// <summary>
    /// Ghost material for 3D asset preview.
    /// </summary>
    Material _previewMaterial;

    /// <summary>
    /// Path of the 3D asset preview material.
    /// </summary>
    const string _PREVIEW_MAT_PATH = "Assets/Resources/Materials/PreviewGhost.mat";

    /// <summary>
    /// Is object placement allowed at the current location?
    /// </summary>
    bool _placementAllowed = true;

    /// <summary>
    /// Index of the currently selected object.
    /// </summary>
    int _selectedObjectIndexForPlacement;

    /// <summary>
    /// Currently selected object in editor;
    /// </summary>
    GameObject _selectedObject;

    /// <summary>
    /// Currently applied object filter.
    /// </summary>
    ObjectDatabase.Category _filter = ObjectDatabase.Category.Block;

    /// <summary>
    /// List of objects that pass the current filter.
    /// </summary>
    List<ObjectData> _filteredObjects;

    int _currentYPosition = 0;

    int _currentYRotation = 0;

    bool _playingLevel = false;

    Vector3 _cursorPosition = Vector3.zero;

    [SerializeField]
    Stack<LevelEditorAction> _undoStack = new Stack<LevelEditorAction>();

    [SerializeField]
    Stack<LevelEditorAction> _redoStack = new Stack<LevelEditorAction>();

    Dictionary<byte, int> _objectCounts = new Dictionary<byte, int>();

    public LevelManagerEvent onCreateLevel = new LevelManagerEvent();
    public LevelManagerEvent onLoadLevel = new LevelManagerEvent();
    public LevelManagerEvent onSaveLevel = new LevelManagerEvent();
    public LevelManagerEvent onCloseLevel = new LevelManagerEvent();
    public LevelManagerEvent onSelectObject = new LevelManagerEvent();
    public LevelManagerEvent onDeselectObject = new LevelManagerEvent();

    bool _mouseOverUI = false;

    #endregion
    #region Properties

    /// <summary>
    /// Returns true if a level is currently loaded (read-only).
    /// </summary>
    public bool LevelLoaded { get { return _loadedLevel != null; } }

    public Level LoadedLevel { get { return _loadedLevel; } }

    /// <summary>
    /// Returns true if unsaved changes exist (read-only).
    /// </summary>
    public bool Dirty { get { return _dirty; } }

    /// <summary>
    /// Returns true if the grid is currently enabled (read-only).
    /// </summary>
    public bool GridEnabled { get { return _drawGrid; } }

    /// <summary>
    /// Gets/sets whether or not objects snap to the grid.
    /// </summary>
    public bool SnapToGrid {
        get { return _snapToGrid; }
        set { _snapToGrid = value; }
    }

    /// <summary>
    /// Returns the currently selected object filter (read-only).
    /// </summary>
    public ObjectDatabase.Category CurrentObjectFilter { get { return _filter; } }

    /// <summary>
    /// Returns the objects that pass the current object filter (read-only).
    /// </summary>
    public List<ObjectData> FilteredObjects { get { return _filteredObjects; } }

    public bool HasSelectedObjectForPlacement { get { return _selectedObjectIndexForPlacement != -1; } }

    /// <summary>
    /// Returns the currently selected floor (read-only).
    /// </summary>
    public int SelectedFloor { get { return _selectedFloor; } }

    /// <summary>
    /// Returns the current editing y-value (read-only).
    /// </summary>
    public int CurrentYValue { get { return _currentYPosition; } }

    /// <summary>
    /// Returns the currently selected object (read-only).
    /// </summary>
    public GameObject SelectedObject { get { return _selectedObject; } }

    /// <summary>
    /// Returns the position of the 3D cursor (read-only).
    /// </summary>
    public Vector3 CursorPosition {
        get { return _cursorPosition; }
        set { _cursorPosition = value; }
    }

    public bool PreviewActive { get { return _preview != null; } }

    public Material PreviewMaterial { get { return _previewMaterial; } }

    public Tool CurrentTool { get { return _currentTool; } }

    public Stack<LevelEditorAction> UndoStack { get { return _undoStack; } }

    public Stack<LevelEditorAction> RedoStack { get { return _redoStack; } }

    public bool MouseOverUI {
        get { return _mouseOverUI; }
        set { _mouseOverUI = value; }
    }

    #endregion
    #region Unity Callbacks

    new void Awake() {
        base.Awake();

        if (Application.isPlaying) {
            DontDestroyOnLoad(gameObject);
            if (_loadedLevel != null) PlayCurrentLevel();
        }
    }

    private void OnDestroy() {
        if (Application.isPlaying)
        StopPlayingLevel();
    }

    #endregion
    #region Methods

    public void StartEditing() {
        if (_preview != null) {
            if (Application.isEditor) DestroyImmediate(_preview);
            else Destroy(_preview);
        }

        _filter = ObjectDatabase.Category.Block;
        _filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(_filter);
        _preview = new GameObject(_PREVIEW_NAME, typeof(MeshFilter), typeof(MeshRenderer));
        //_preview.hideFlags = HideFlags.HideAndDontSave;
        _selectedObjectIndexForPlacement = -1;
        _selectedFloor = 0;
        _currentYPosition = 0;
        _currentYRotation = 0;
        _editingPlane = new Plane(Vector3.up, Vector3.zero);

        // Load preview material
        if (Application.isEditor) _previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(_PREVIEW_MAT_PATH);
        else _previewMaterial = Resources.Load<Material>(_PREVIEW_MAT_PATH);
        if (_previewMaterial == null) Debug.LogError("Failed to load preview material!");
    }

    public void StopEditing() {
        if (_preview != null)
            DestroyImmediate(_preview);
    }

    public void SetDirty(bool dirty) {
        _dirty = dirty;
    }

    public void SelectTool(Tool tool) {
        _currentTool = tool;
        if (tool != Tool.Place) DisablePreview();
    }

    public void SetObjectFilter(ObjectDatabase.Category filterIndex) {
        _filter = filterIndex;
        _filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(_filter);
    }

    public void SetLoadedLevelName(string newName) {
        _loadedLevel.Name = newName;
        _loadedLevelObject.name = newName + " (Loaded Level)";
    }

    public void SelectObject(GameObject obj) {
        if (obj != _selectedObject) onSelectObject.Invoke();
        _selectedObject = obj;
    }

    public void DeselectObject() {
        if (_selectedObject != null) onDeselectObject.Invoke();
        _selectedObject = null;
    }

    /// <summary>
    /// Shows a 3D preview of the currently selected asset.
    /// </summary>
    void Show3DAssetPreview(GameObject obj) {
        var assetMeshFilter = obj.GetComponentInChildren<MeshFilter>();
        if (assetMeshFilter == null) return;

        var assetMesh = assetMeshFilter.sharedMesh;
        if (assetMesh == null) return;

        var previewMeshFilter = _preview.GetComponent<MeshFilter>();
        var previewMeshRenderer = _preview.GetComponent<MeshRenderer>();

        previewMeshFilter.sharedMesh = assetMesh;
        previewMeshRenderer.enabled = true;
        previewMeshRenderer.sharedMaterial = _previewMaterial;
    }

    /// <summary>
    /// Hides the 3D asset preview.
    /// </summary>
    void DisablePreview() {
        if (_preview == null) return;
        _preview.GetComponent<MeshRenderer>().enabled = false;
    }

    public Plane EditingPlane { get { return _editingPlane; } }

    public Level.ObjectAttributes AttributesOfObject(GameObject obj) {
        var index = LevelIndexOfObject(obj);
        return _loadedLevel[_selectedFloor][index];
    }

    public Vector3 PositionOfObject(GameObject obj) {
        var attribs = AttributesOfObject(obj);
        return attribs.Position;
    }

    public int LevelIndexOfObject(GameObject obj) {
        return _loadedSpawners.IndexOf(obj.GetComponent<ObjectSpawner>());
    }

    public byte ObjectIndexOfObject(GameObject obj) {
        var index = LevelIndexOfObject(obj);
        var data = _loadedLevel[_selectedFloor][index];
        if (data == null) {
            Debug.LogError("Object " + obj.name + " not found in the level file!");
            return byte.MaxValue;
        }
        return data.Index;
    }

    public void UpOneFloor() {
        if (_selectedFloor < Level.MAX_FLOORS - 1) {
            CleanupSpawners();
            _selectedFloor++;
            _currentYPosition = 0;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    public void DownOneFloor() {
        if (_selectedFloor > 0) {
            CleanupSpawners();
            _selectedFloor--;
            _currentYPosition = 0;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    public void IncrementY() {
        if (_currentYPosition < Level.FLOOR_HEIGHT - 1) {
            _currentYPosition++;
            UpdateHeight();
        }
    }

    public void DecrementY() {
        if (_currentYPosition > 0) {
            _currentYPosition--;
            UpdateHeight();
        }
    }

    public void UpdateHeight() {
        _editingPlane.SetNormalAndPosition(new Vector3(0f, _selectedFloor * Level.FLOOR_HEIGHT + _currentYPosition, 0f), Vector3.up);
    }

    /// <summary>
    /// Snaps the selected object to the grid.
    /// </summary>
    public void SnapSelected(SceneView sc) {
        if (Selection.transforms.Length > 0) {
            var pos = Selection.transforms[0].position;
            var snapped = new Vector3(
                Mathf.Round(pos.x),
                Mathf.Round(pos.y),
                Mathf.Round(pos.z)
            );
            Selection.transforms[0].position = snapped;
        }
    }

    public void SetCursorPosition (Vector3 newPos) {
        _cursorPosition = newPos;
        if (_snapToGrid) SnapCursor();
    }

    /// <summary>
    /// Snaps the preview asset to the grid.
    /// </summary>
    public void SnapCursor() {
        var pos = _cursorPosition;
        var snapped = new Vector3(
                Mathf.Round(pos.x),
                Mathf.Round(pos.y) + 0.5f,
                Mathf.Round(pos.z)
            );
        _cursorPosition = snapped;
    }

    /// <summary>
    /// Checks the placement of the preview asset to see if it is valid.
    /// </summary>
    public bool CheckPlacement() {
        // Check x in range
        float x = _cursorPosition.x;
        if (x < -0.01f || x >= (float)Level.FLOOR_WIDTH + 0.01f) return false;

        // Check y in range
        float y = _cursorPosition.y;
        if (y < -0.01f || y >= (float)Level.FLOOR_HEIGHT + 0.01f) return false;

        // Check z in range
        float z = _cursorPosition.z;
        if (z < -0.01f || z >= (float)Level.FLOOR_DEPTH + 0.01f) return false;

        return true;
    }

    public void ResetTool() {
        _selectedObjectIndexForPlacement = -1;
        DisablePreview();
        _currentTool = Tool.Select;
    }

    public bool CanPlaceAnotherObject(int index) {
        var limit = ObjectDatabaseManager.Instance.GetObjectLimit(index);
        if (limit < 0) return true;

        else return _objectCounts[(byte)index] < limit;
    }

    public bool CanPlaceAnotherCurrentObject() {
        return CanPlaceAnotherObject(_selectedObjectIndexForPlacement);
    }

    public Level.ObjectAttributes GetAttributesOfObject(GameObject obj) {
        var index = _loadedSpawners.IndexOf(obj.GetComponent<ObjectSpawner>());
        var data = _loadedLevel[_selectedFloor][index];
        return data;
    }

    public bool HasSelectedObject { get { return _selectedObject != null; } }

    public bool IsMovingObject { get { return _movingObjectID != -1; } }

    public void CreateCurrentSelectedObjectAtCursor() {
        CreateObject((byte)_selectedObjectIndexForPlacement, _cursorPosition, ActionType.Normal);
    }

    public void CreateObject(byte index, Vector3 position, ActionType actionType) {
        if (index < 0 || index >= byte.MaxValue) Debug.LogWarning ("Invalid object!");

        _loadedLevel.AddObject((int)index, _selectedFloor, position, _currentYRotation);

        GameObject spawner = CreateSpawner(index);
        spawner.transform.SetParent(_floorObjects[_selectedFloor].transform);
        spawner.transform.position = position;
        _loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
        _objectCounts[index]++;
        _dirty = true;

        switch (actionType) {
            case ActionType.Normal:
            case ActionType.Redo:
                _undoStack.Push(new CreateObjectAction(index, spawner));
                break;

            case ActionType.Undo:
                _redoStack.Push(new DeleteObjectAction(spawner, index));
                break;
        }
    }

    public void DeleteObject(GameObject obj, ActionType actionType) {
        byte deletedObjectIndex = (byte)ObjectIndexOfObject(obj);
        int levelIndex = LevelIndexOfObject(obj);

        var deleteAction = new DeleteObjectAction(obj, deletedObjectIndex);
        switch (actionType) {
            case ActionType.Normal:
            case ActionType.Redo:
                _undoStack.Push(deleteAction);
                break;

            case ActionType.Undo:
                _redoStack.Push(new CreateObjectAction(deletedObjectIndex, obj));
                break;
        }

        _loadedLevel.DeleteObject(_selectedFloor, levelIndex);
        _objectCounts[deletedObjectIndex]--;
        _loadedSpawners.Remove(obj.GetComponent<ObjectSpawner>());
        DestroyLoadedObject(obj);
    }

    public void MoveObject(GameObject obj, Vector3 oldPos, Vector3 newPos, ActionType actionType) {
        var moveAction = new MoveObjectAction(obj, oldPos, newPos);
        switch (actionType) {
            case ActionType.Normal:
            case ActionType.Redo:
                _undoStack.Push(moveAction);
                break;

            case ActionType.Undo:
                _redoStack.Push(new MoveObjectAction(obj, newPos, _movingObjectOriginalCoords));
                break;
        }

        var i = LevelIndexOfObject(obj);
        _loadedLevel[_selectedFloor][i].SetPosition(newPos);
    }

    public void Undo() {
        if (_undoStack.Count <= 0) return;

        var undoAction = _undoStack.Pop();
        undoAction.Undo();
    }

    public void Redo() {
        if (_redoStack.Count <= 0) return;

        var redoAction = _redoStack.Pop();
        redoAction.Redo();
    }

    public void AddRedo(LevelEditorAction redo) {
        _redoStack.Push(redo);
    }

    public void AddUndo(LevelEditorAction undo) {
        _undoStack.Push(undo);
    }

    public void EnableGrid() {
        _drawGrid = true;
    }

    public void DisableGrid() {
        _drawGrid = false;
    }

    public void StartMovingObject(GameObject obj) {
        var data = GetAttributesOfObject(obj);
        _movingObjectID = data.Index;
        Show3DAssetPreview(obj);
        _movingObjectOriginalCoords = data.Position;
        DeleteObject(obj, ActionType.None);
    }

    public void StopMovingObject() {
        if (_placementAllowed) {
            CreateObject((byte)_movingObjectID, _cursorPosition, ActionType.None);
            _movingObjectID = -1;
            DisablePreview();
        }
    }

    public void ResetMovingObject() {
        CreateObject((byte)_movingObjectID, _movingObjectOriginalCoords, ActionType.None);
        _movingObjectID = -1;
    }

    public void RotateSelectedObject(int rotation) {
        var attribs = AttributesOfObject(_selectedObject);
        var originalRotation = attribs.RotationY;
        var newRotation = (originalRotation + rotation) % 360;
        attribs.RotationY = newRotation;
        _selectedObject.transform.Rotate(0f, rotation, 0f);
    }

    /// <summary>
    /// Selects an asset from the object browser.
    /// </summary>
    /// <param name="index">Index of the object in the browser.</param>
    public void SelectObjectInCategory(int index, ObjectDatabase.Category category) {
        var data = ObjectDatabaseManager.Instance.AllObjectsInCategory(category)[index];
        _selectedObjectIndexForPlacement = data.index;
        var obj = ObjectDatabaseManager.Instance.GetObject(index);

        Show3DAssetPreview(obj);
    }

    /// <summary>
    /// Creates a new level.
    /// </summary>
    public void CreateNewLevel() {
        _loadedLevel = new Level();

        SetupLevelObjects();

        InitObjectCounts();

        StartEditing();

        onCreateLevel.Invoke();
    }

    /// <summary>
    /// Saves the current level.
    /// </summary>
    public void SaveCurrentLevelToJSON(bool doUseFilePanel = false) {
        /*var asset = _loadedLevel.ToLevelAsset();

        if (asset == default(LevelAsset)) {
            Debug.LogError("Failed to save level!");
            return;
        }*/

        if (doUseFilePanel || _loadedLevelPath == "") {
            string savePath = default(string);
            if (Application.isEditor) {
                savePath = EditorUtility.SaveFilePanel ("Save Level to JSON", Application.dataPath, _loadedLevel.Name, "json");
                if (savePath == default(string) || savePath == "") return;
                JSONFileUtility.SaveToJSONFile (_loadedLevel, savePath);
                //savePath = ScriptableObjectUtility.SaveScriptableObjectWithFilePanel(asset, "Save Level File", asset.ToString(), "asset");
            } else {
                // In-game editor file panel
            }
            
            _loadedLevelPath = savePath;
        } else {
            JSONFileUtility.SaveToJSONFile (_loadedLevel, _loadedLevelPath);
            //ScriptableObjectUtility.SaveScriptableObject(asset, _loadedLevelPath, true);
        }

        _dirty = false;

        onSaveLevel.Invoke();
    }

    /// <summary>
    /// Closes the currently loaded level.
    /// </summary>
    public void CloseLevel() {
        StopEditing();

        _loadedLevel = null;
        _loadedLevelPath = "";
        SelectTool(Tool.Select);
        _selectedFloor = 0;


        _undoStack.Clear();
        _redoStack.Clear();

        CleanupSpawners();

        _objectCounts.Clear();

        onCloseLevel.Invoke();
    }

    /// <summary>
    /// Prompts the user for a level asset to load.
    /// </summary>
    public void LoadLevelFromJSON() {

        /*string assetPath = default(string);
        LevelAsset asset = null;
        if (Application.isEditor)
            asset = ScriptableObjectUtility.LoadScriptableObjectWithFilePanel<LevelAsset>("Load Level File", "Assets", "asset", out assetPath);*/
        // else open load panel in-game
        var assetPath = EditorUtility.OpenFilePanel ("Open Level JSON", Application.dataPath, "json");

        // If user cancelled loading
        if (assetPath == default(string) || assetPath == "") return;

        var asset = JSONFileUtility.LoadFromJSONFile<Level>(assetPath);

        // If asset failed to load
        if (asset == null) {
            Debug.LogError("Failed to load level!");
            return;
        }

        /*if (!AssetDatabase.CopyAsset(assetPath, _TEMP_LEVEL_PATH)) {
            Debug.LogError ("Failed to create temp copy of loaded level! " + _TEMP_LEVEL_PATH);
            return;
        }

        var assetCopy = ScriptableObjectUtility.LoadScriptableObject<LevelAsset>(_TEMP_LEVEL_PATH);*/

        if (_loadedLevel != null) CloseLevel();

        /*Level level = assetCopy.Unpack();
        if (level.Equals(default(Level))) {
            Debug.LogError("Level file is corrupted at " + asset);
            return;
        }*/

        _loadedLevel = asset;

        _dirty = false;

        SetupLevelObjects();

        InitObjectCounts();
        GetObjectCounts();

        ReconstructFloor();

        StartEditing();

        onLoadLevel.Invoke();
    }

    /// <summary>
    /// Spawns objects from the currently loaded level.
    /// </summary>
    void ReconstructFloor() {
        var objects = _loadedLevel[_selectedFloor].Objects;
        for (int i = 0; i < objects.Length; i++) {
            var attribs = objects[i];
            byte index;

            try {
                index = attribs.Index;
            } catch (NullReferenceException) {
                continue;
            }

            if (index == byte.MaxValue) continue;

            var spawner = CreateSpawner (index);
            spawner.transform.parent = _floorObjects[_selectedFloor].transform;
            spawner.transform.position = attribs.Position;
            spawner.transform.rotation = Quaternion.Euler (attribs.EulerRotation);
            spawner.transform.localScale = attribs.Scale;
            _loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
        }
    }

    /// <summary>
    /// Creates base level objects.
    /// </summary>
    void SetupLevelObjects() {
        _loadedLevelObject = new GameObject(_loadedLevel.Name + " (Loaded Level)");
        _floorObjects = new GameObject[Level.MAX_FLOORS];
        for (int floor = 0; floor < Level.MAX_FLOORS; floor++) {
            _floorObjects[floor] = new GameObject("Floor " + floor.ToString());
            _floorObjects[floor].transform.SetParent(_loadedLevelObject.transform);
        }
    }

    void GetObjectCounts() {
        foreach (var obj in _loadedSpawners) {
            var index = ObjectIndexOfObject(obj.Template);
            _objectCounts[index]++;
        }
    }

    void InitObjectCounts() {
        _objectCounts.Clear();

        for (int i = 0; i < (int)byte.MaxValue; i++)
            _objectCounts.Add((byte)i, 0);
    }

    public GameObject CreateSpawner (GameObject template) {
        GameObject spawner = new GameObject (template.name + " Spawner", typeof (ObjectSpawner));
        spawner.GetComponent<ObjectSpawner>().SetTemplate (template);
        return spawner;
    }

    /// <summary>
    /// Returns a new instance of the object with the given index.
    /// </summary>
    public GameObject CreateSpawner(byte index) {
        var template = ObjectDatabaseManager.Instance.GetObject(index);
        return CreateSpawner (template);
    }

    /// <summary>
    /// Cleans up level object spawners in the scene.
    /// </summary>
    void CleanupSpawners() {
        if (_loadedSpawners == null) return;

        DestroyLoadedObject(_loadedLevelObject);
        _loadedSpawners.Clear();

        //while (_loadedSpawners.Count > 0) DestroyLoadedObject(_loadedSpawners.PopFront().gameObject);
    }

    void DestroyLoadedObject(GameObject obj) {
        if (Application.isEditor) DestroyImmediate(obj);
        else Destroy(obj);
    }

    public void PlayCurrentLevel () {
        Debug.Log("play");

        foreach (ObjectSpawner spawner in _loadedSpawners) {
            spawner.SpawnObject();
        }

        _playingLevel = true;
    }

    public void StopPlayingLevel () {
        Debug.Log ("stop");

        foreach (var spawner in _loadedSpawners)
            spawner.Reset();

        _playingLevel = false;
    }

    #endregion
    #region Nested Classes

    /// <summary>
    /// Class for LevelManager-specific events
    /// </summary>
    [Serializable]
    public class LevelManagerEvent : UnityEvent { }

    #endregion
}
