// LevelManager.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ModMan;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public const float TILE_UNIT_WIDTH = 5f;

    /// <summary>
    /// Default level path.
    /// </summary>
    const string _LEVEL_PATH = "Assets/Resources/Levels/";

    /// <summary>
    /// Path of the 3D asset preview material.
    /// </summary>
    const string _PREVIEW_MAT_PATH = "Assets/Resources/Materials/PreviewGhost.mat";

    /// <summary>
    /// Editor name of the 3D asset preview object.
    /// </summary>
    const string _PREVIEW_NAME = "~LevelEditorPreview";

    /// <summary>
    /// Key name of last edited level string.
    /// </summary>
    public const string LAST_EDITED_LEVEL_STR = "LAST_EDITED_LEVEL";

    #endregion
    #region Vars

    ILevelEditor _editor;

    /// <summary>
    /// The currently loaded level in LM.
    /// !!! KEEP NONSERIALIZED !!!
    /// </summary>
    [NonSerialized]
    Level _loadedLevel;

    /// <summary>
    /// Resource path of current loaded level.
    /// </summary>
    [SerializeField]
    string _loadedLevelPath = "";

    /// <summary>
    /// GameObject representing the loaded level.
    /// </summary>
    [SerializeField]
    GameObject _loadedLevelObject;

    /// <summary>
    /// GameObjects representing the floors of the loaded level.
    /// </summary>
    [SerializeField]
    GameObject[] _floorObjects;

    /// <summary>
    /// All currently loaded objects in the editor.
    /// </summary>
    [SerializeField]
    List<ObjectSpawner> _loadedSpawners =
        new List<ObjectSpawner>();

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
    [SerializeField]
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
    GameObject _movingObject;

    /// <summary>
    /// Original coordinates of object being moved;
    /// </summary>
    Vector3 _movingObjectOriginalCoords;

    /// <summary>
    /// 3D asset preview object.
    /// </summary>
    AssetPreview _assetPreview;

    /// <summary>
    /// Ghost material for 3D asset preview.
    /// </summary>
    Material _previewMaterial;

    /// <summary>
    /// Is object placement allowed at the current location?
    /// </summary>
    bool _placementAllowed = true;

    /// <summary>
    /// Index of the currently selected object.
    /// </summary>
    int _selectedObjectIndexForPlacement;

    List<GameObject> _hoveredObjects = new List<GameObject>();

    /// <summary>
    /// Currently selected object in editor;
    /// </summary>
    List<GameObject> _selectedObjects = new List<GameObject>();

    /// <summary>
    /// Currently applied object filter.
    /// </summary>
    ObjectDatabase.Category _filter = ObjectDatabase.Category.Block;

    /// <summary>
    /// List of objects that pass the current filter.
    /// </summary>
    List<ObjectData> _filteredObjects;

    /// <summary>
    /// Current editing y position.
    /// </summary>
    int _currentYPosition = 0;

    /// <summary>
    /// Current editing y rotation.
    /// </summary>
    int _currentYRotation = 0;

    /// <summary>
    /// Is the level currently being played?
    /// </summary>
    bool _playingLevel = false;

    /// <summary>
    /// Current 3D cursor position.
    /// </summary>
    Vector3 _cursorPosition = Vector3.zero;

    /// <summary>
    /// Is the mouse currently over UI?
    /// This must be managed externally, as LM does not know what
    /// UI elements exist.
    /// </summary>
    bool _mouseOverUI = false;

    /// <summary>
    /// Stack of undo actions.
    /// </summary>
    Stack<LevelEditorAction> _undoStack = new Stack<LevelEditorAction>();

    /// <summary>
    /// Stack of redo actions.
    /// </summary>
    Stack<LevelEditorAction> _redoStack = new Stack<LevelEditorAction>();

    /// <summary>
    /// Number of objects of each type (to enforce maximums).
    /// </summary>
    [SerializeField]
    //Dictionary<byte, int> _objectCounts = new Dictionary<byte, int>();
    //ByteToIntDict _objectCounts = new ByteToIntDict();
    List<int> _objectCounts;

    Vector3 _mouseDownWorldPos;
    Vector2 _mouseDownScreenPos;

    public LevelManagerEvent onCreateLevel = new LevelManagerEvent();
    public LevelManagerEvent onLoadLevel = new LevelManagerEvent();
    public LevelManagerEvent onSaveLevel = new LevelManagerEvent();
    public LevelManagerEvent onCloseLevel = new LevelManagerEvent();
    public LevelManagerEvent onSelectObject = new LevelManagerEvent();
    public LevelManagerEvent onDeselectObject = new LevelManagerEvent();

    #endregion
    #region Properties

    public bool IsConnectedToEditor { get { return _editor != null; } }

    /// <summary>
    /// Returns true if a level is currently loaded (read-only).
    /// </summary>
    public bool LevelLoaded { get { return _loadedLevel != null; } }

    /// <summary>
    /// Returns the currently loaded level (read-only).
    /// </summary>
    public Level LoadedLevel { get { return _loadedLevel; } }

    /// <summary>
    /// Returns the path of the currently loaded level.
    /// </summary>
    public string LoadedLevelPath { get { return _loadedLevelPath; } }

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

    public List<GameObject> HoveredObjects { get { return _hoveredObjects; } }

    public List<GameObject> SelectedObjects { get { return _selectedObjects; } }

    /// <summary>
    /// Returns true if an object is currently selected for placement (read-only).
    /// </summary>
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
    /// Returns the position of the 3D cursor (read-only).
    /// </summary>
    public Vector3 CursorPosition {
        get { return _cursorPosition; }
        set { _cursorPosition = value; }
    }

    /// <summary>
    /// Returns true if the 3D preview is active (read-only).
    /// </summary>
    public bool PreviewActive { get { return _assetPreview != null; } }

    /// <summary>
    /// Returns the preview material (read-only).
    /// </summary>
    public Material PreviewMaterial { get { return _previewMaterial; } }

    /// <summary>
    /// Returns the current tool (read-only).
    /// </summary>
    public Tool CurrentTool { get { return _currentTool; } }

    /// <summary>
    /// Returns the undo stack (read-only).
    /// </summary>
    public Stack<LevelEditorAction> UndoStack { get { return _undoStack; } }

    /// <summary>
    /// Returns the redo stack (read-only).
    /// </summary>
    public Stack<LevelEditorAction> RedoStack { get { return _redoStack; } }

    /// <summary>
    /// Gets/sets whether or not the mouse is currently over UI.
    /// </summary>
    public bool MouseOverUI {
        get { return _mouseOverUI; }
        set { _mouseOverUI = value; }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Gets/sets the path of the last edited level.
    /// </summary>
    public static string LastEditedLevelPath {
        get { return EditorPrefs.GetString(LAST_EDITED_LEVEL_STR); }
        set { EditorPrefs.SetString(LAST_EDITED_LEVEL_STR, value); }
    }
#endif

    /// <summary>
    /// Returns the editing plane (read-only).
    /// </summary>
    public Plane EditingPlane { get { return _editingPlane; } }

    /// <summary>
    /// Returns true if an object is selected (read-only).
    /// </summary>
    public bool HasSelectedObjects { get { return _selectedObjects.Count > 0; } }

    /// <summary>
    /// Returns true if an object is being moved (read-only).
    /// </summary>
    public bool IsMovingObject { get { return _movingObject != null; } }

    public Vector2 MouseDownScreenPos {
        get { return _mouseDownScreenPos; }
        set { _mouseDownScreenPos = value; }
    }

    public Vector3 MouseDownWorldPos {
        get { return _mouseDownWorldPos; }
        set { _mouseDownWorldPos = value; }
    }

    #endregion
    #region Unity Callbacks

    new void Awake() {
        base.Awake();

        // Try to reacquire the loaded level on application play
        if (LevelObject.Instance != null)
            _loadedLevel = LevelObject.Instance.Level;
        else if (_loadedLevelObject != null)
            _loadedLevel = _loadedLevelObject.GetComponent<LevelObject>().Level;

        _selectedObjects.Clear();
        _hoveredObjects.Clear();

        if (Application.isPlaying) AwakePlayer();
        else AwakeEditor();
    }

    void OnApplicationQuit() {
        //Debug.Log("LevelManager.OnApplicationQuit");
        if (Application.isPlaying && _playingLevel)
            StopPlayingLevel();
    }

    void OnDestroy() {
        //Debug.Log("LevelManager.OnDestroy");

        _selectedObjects.Clear();
        _hoveredObjects.Clear();
    }

    #endregion
    #region Methods

    public void SetEditor(ILevelEditor editor) {
        _editor = editor;
    }

    /// <summary>
    /// Called on awake in the player.
    /// </summary>
    void AwakePlayer() {
        //Debug.Log("LevelManager.AwakePlayer");
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        var path = LastEditedLevelPath;
        //Debug.Log("Last loaded level path: " + path);
        if (path != default(string) && path != "") {
            LoadLevelFromJSON(path);

        }
#endif

        if (_loadedLevelObject != null) PlayCurrentLevel();
    }

    /// <summary>
    /// Called on awake in the editor.
    /// </summary>
    void AwakeEditor() {
        //Debug.Log("LevelManager.AwakeEditor");
#if UNITY_EDITOR
        var path = LastEditedLevelPath;
        if (path != default(string) && path != "") {
            LoadLevelFromJSON(path);
        }
#endif
    }

    /// <summary>
    /// Prepares the LM for editing.
    /// </summary>
    public void StartEditing() {
        if (_assetPreview != null) {
            if (Application.isEditor) DestroyImmediate(_assetPreview.gameObject);
            else Destroy(_assetPreview.gameObject);
        }

        _filter = ObjectDatabase.Category.Block;
        _filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(_filter);
        CreateAssetPreview();
        _selectedObjectIndexForPlacement = -1;
        _selectedFloor = 0;
        _currentYPosition = 0;
        _currentYRotation = 0;
        _editingPlane = new Plane(Vector3.up, Vector3.zero);
    }

    /// <summary>
    /// Stops the LM from editing.
    /// </summary>
    public void StopEditing() {
        if (_assetPreview != null)
            DestroyImmediate(_assetPreview.gameObject);
    }

    /// <summary>
    /// Initializes the 3D asset preview.
    /// </summary>
    void CreateAssetPreview() {
        _assetPreview = new GameObject(_PREVIEW_NAME,
            typeof(AssetPreview)).GetComponent<AssetPreview>();
        //_preview.hideFlags = HideFlags.HideAndDontSave;

        // Load preview material
        if (_previewMaterial == null) {

            if (Application.isEditor) LoadPreviewMaterialEditor();
            else _previewMaterial = Resources.Load<Material>(_PREVIEW_MAT_PATH);
            if (_previewMaterial == null) Debug.LogError("Failed to load preview material!");
        }
    }

    /// <summary>
    /// Loads the preview materials.
    /// </summary>
    void LoadPreviewMaterialEditor() {
#if UNITY_EDITOR
        _previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(_PREVIEW_MAT_PATH);
#endif
    }

    /// <summary>
    /// Sets the dirty status of the LM.
    /// </summary>
    public void SetDirty(bool dirty) { _dirty = dirty; }

    public void HandleLeftDown(Event e) {
        if (Application.isEditor) {

            #if UNITY_EDITOR
            _mouseDownScreenPos = e.mousePosition;
            var ray = HandleUtility.GUIPointToWorldRay(_mouseDownScreenPos);
            float dist;
            if (LevelManager.Instance.EditingPlane.Raycast(ray, out dist)) {
                _mouseDownWorldPos = ray.GetPoint(dist);
            }
            #endif
        }
    }

    public void HandleLeftUp(Event e, bool isDragging, Camera camera) {
        switch (_currentTool) {
            case Tool.Select:
                if (LevelLoaded) {
                    SelectObjects(_hoveredObjects);
                } else DeselectObjects();
                break;

            case Tool.Place:
                if (HasSelectedObjectForPlacement &&
                    CanPlaceAnotherCurrentObject()) {
                    if (_snapToGrid) SnapCursor();
                    CreateCurrentSelectedObjectAtCursor();
                    e.Use();
                }
                break;

            case Tool.Erase:
                if (_hoveredObjects.Count > 0) DeleteObject(_hoveredObjects[0], LevelEditorAction.ActionType.Normal);
                break;

            case Tool.Move:
                if (!IsMovingObject) { // Not currently moving
                    if (_hoveredObjects.Count > 0) StartMovingObject(_hoveredObjects[0]);
                } else { // Currently moving
                    StopMovingObject();
                }
                break;
        }
    }

    public void HandleMouseMove(Event e, bool isDragging) {

        var ray = _editor.PointerRay;
        float distance;
        if (_editingPlane.Raycast(ray, out distance))
            SetCursorPosition(ray.GetPoint(distance));
        if (_currentTool != Tool.Place) {
            if (isDragging) {
                _hoveredObjects = ObjectsInSelection();
            } else {



                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    ObjectSpawner spawner = hit.collider.gameObject.GetComponentInAncestors<ObjectSpawner>();
                    if (spawner != null)
                        HoverObject(spawner.gameObject);
                } else _hoveredObjects.Clear();
            }

        }
    }

    /// <summary>
    /// Selects the given tool.
    /// </summary>
    public void SelectTool(Tool tool) {
        _currentTool = tool;
        if (tool != Tool.Place) DisablePreview();
    }

    /// <summary>
    /// Sets the object browser filter.
    /// </summary>
    public void SetObjectFilter(ObjectDatabase.Category filterIndex) {
        _filter = filterIndex;
        _filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(_filter);
    }

    /// <summary>
    /// Sets the name of the loaded level.
    /// </summary>
    public void SetLoadedLevelName(string newName) {
        _loadedLevel.Name = newName;
        _loadedLevelObject.name = newName + " (Loaded Level)";
    }

    public void HoverObject(GameObject obj) {
        _hoveredObjects.Clear();
        _hoveredObjects.Add(obj);
    }

    public List<GameObject> ObjectsInSelection() {
        Camera camera = _editor.ActiveCamera;
        Rect selectionRect = _editor.SelectionRect;
        List<GameObject> result = new List<GameObject>();
        foreach (var spawner in _loadedSpawners) {
            Vector3 rawScreenPos = camera.WorldToScreenPoint(spawner.transform.position);
            Vector2 screenPos = new Vector2 (rawScreenPos.x, rawScreenPos.y);
            if (selectionRect.Contains(screenPos, true)) result.Add(spawner.gameObject);
        }
        Debug.Log (result.Count);
        return result;
    }


    /// <summary>
    /// Selects an object in the level.
    /// </summary>
    public void SelectObjects(List<GameObject> objects, bool clearFirst=true) {
        if (clearFirst) _selectedObjects.Clear();
        foreach (var obj in objects) {
            // If selected object is not spawner, find through parents
            var spawner = obj.GetComponent<ObjectSpawner>();
            if (spawner == null) spawner = obj.GetComponentInAncestors<ObjectSpawner>();

            if (spawner != null) {
                if (!_selectedObjects.Contains(spawner.gameObject)) {
                    _selectedObjects.Add(spawner.gameObject);
                    onSelectObject.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Deselects an object.
    /// </summary>
    public void DeselectObjects() {
        if (_selectedObjects != null) {
            onDeselectObject.Invoke();
            _selectedObjects.Clear();
        }
    }

    /// <summary>
    /// Shows a 3D preview of the currently selected asset.
    /// </summary>
    void ShowAssetPreview(GameObject obj) {
        if (obj == null) throw new System.NullReferenceException("No asset given!");
        _assetPreview.GetComponent<AssetPreview>().SetPreviewObject(obj);
        _assetPreview.Show();
    }

    /// <summary>
    /// Hides the 3D asset preview.
    /// </summary>
    void DisablePreview() {
        if (_assetPreview == null) return;
        _assetPreview.Hide();
    }

    /// <summary>
    /// Gets the attributes of the given object.
    /// </summary>
    public Level.ObjectAttributes AttributesOfObject(GameObject obj) {
        var index = LevelIndexOfObject(obj);
        return _loadedLevel[_selectedFloor][index];
    }

    /// <summary>
    /// Gets the index in the level of the given object.
    /// </summary>
    public int LevelIndexOfObject(GameObject obj) {
        return _loadedSpawners.IndexOf(obj.GetComponent<ObjectSpawner>());
    }

    /// <summary>
    /// Gets the database index of the given object.
    /// </summary>
    public byte ObjectIndexOfObject(GameObject obj) {
        var index = LevelIndexOfObject(obj);
        var data = _loadedLevel[_selectedFloor][index];
        if (data == null) {
            Debug.LogError("Object " + obj.name + " not found in the level file!");
            return byte.MaxValue;
        }
        return data.Index;
    }

    /// <summary>
    /// Moves editing up by one floor.
    /// </summary>
    public void UpOneFloor() {
        if (_selectedFloor < Level.MAX_FLOORS - 1) {
            CleanupFloor();
            _selectedFloor++;
            _currentYPosition = 0;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    /// <summary>
    /// Moves editing down by one floor.
    /// </summary>
    public void DownOneFloor() {
        if (_selectedFloor > 0) {
            CleanupFloor();
            _selectedFloor--;
            _currentYPosition = 0;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    /// <summary>
    /// Moves the editing plane up by one tile.
    /// </summary>
    public void IncrementY() {
        if (_currentYPosition < Level.FLOOR_HEIGHT - TILE_UNIT_WIDTH) {
            _currentYPosition++;
            UpdateHeight();
        }
    }

    /// <summary>
    /// Moves the editing plane down by one tile.
    /// </summary>
    public void DecrementY() {
        if (_currentYPosition > 0) {
            _currentYPosition--;
            UpdateHeight();
        }
    }

    /// <summary>
    /// Updates the editing plane according to the current floor and y-value.
    /// </summary>
    public void UpdateHeight() {
        _editingPlane.SetNormalAndPosition(Vector3.up,
            new Vector3(0f, _selectedFloor * Level.FLOOR_HEIGHT + _currentYPosition, 0f)
            );
    }

    /// <summary>
    /// Sets the cursor position, snapping if necessary.
    /// </summary>
    public void SetCursorPosition(Vector3 newPos) {
        _cursorPosition = newPos;
        if (_snapToGrid) SnapCursor();
        if (_assetPreview != null) _assetPreview.transform.position = _cursorPosition;
    }

    /// <summary>
    /// Snaps the preview asset to the grid.
    /// </summary>
    public void SnapCursor() {
        var pos = _cursorPosition;
        var snapped = new Vector3(
                Mathf.Round(pos.x / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH,
                Mathf.Round(pos.y / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH,
                Mathf.Round(pos.z / TILE_UNIT_WIDTH) * TILE_UNIT_WIDTH
            );
        _cursorPosition = snapped;
    }

    /// <summary>
    /// Checks the placement of the preview asset to see if it is valid.
    /// </summary>
    public bool CheckPlacement() {
        // Check x in range
        float x = _cursorPosition.x;
        if (x < -0.01f || x >= (float)Level.FLOOR_WIDTH * TILE_UNIT_WIDTH + 0.01f) return false;

        // Check y in range
        float y = _cursorPosition.y;
        float minY = _selectedFloor * Level.FLOOR_HEIGHT * TILE_UNIT_WIDTH - 0.01f;
        float maxY = (_selectedFloor + 1) * Level.FLOOR_HEIGHT * TILE_UNIT_WIDTH + 0.01f;
        if (y < minY || y >= maxY) return false;

        // Check z in range
        float z = _cursorPosition.z;
        if (z < -0.01f || z >= (float)Level.FLOOR_DEPTH * TILE_UNIT_WIDTH + 0.01f) return false;

        return true;
    }

    /// <summary>
    /// Resets the currently selected tool (to placement).
    /// </summary>
    public void ResetTool() {
        _selectedObjectIndexForPlacement = -1;
        DisablePreview();
        _currentTool = Tool.Select;
    }

    /// <summary>
    /// Returns true if allowed to place more of the object with the given index.
    /// </summary>
    public bool CanPlaceAnotherObject(int index) {
        var limit = ObjectDatabaseManager.Instance.GetObjectLimit(index);
        if (limit < 0) return true;

        else return _objectCounts[(byte)index] < limit;
    }

    /// <summary>
    /// Returns true if allowed to place more of the currently selected object.
    /// </summary>
    public bool CanPlaceAnotherCurrentObject() {
        return CanPlaceAnotherObject(_selectedObjectIndexForPlacement);
    }

    /// <summary>
    /// Gets the object attributes of the given object.
    /// </summary>
    public Level.ObjectAttributes GetAttributesOfObject(GameObject obj) {
        var index = _loadedSpawners.IndexOf(obj.GetComponent<ObjectSpawner>());
        var data = _loadedLevel[_selectedFloor][index];
        return data;
    }

    /// <summary>
    /// Creates one of the currently selected objects at the cursor.
    /// </summary>
    public void CreateCurrentSelectedObjectAtCursor() {
        CreateObject((byte)_selectedObjectIndexForPlacement, _cursorPosition, LevelEditorAction.ActionType.Normal);
    }

    /// <summary>
    /// Crates an object with the given index.
    /// </summary>
    public void CreateObject(byte index, Vector3 position, LevelEditorAction.ActionType actionType) {

        // Warn if invalid index
        if (index < 0 || index >= byte.MaxValue)
            throw new IndexOutOfRangeException("Invalid index! " + index);

        // Add object to level file
        _loadedLevel.AddObject((int)index, _selectedFloor, position, _currentYRotation);

        // Create spawner
        GameObject spawner = CreateSpawner(index);
        spawner.transform.SetParent(_floorObjects[_selectedFloor].transform);
        spawner.transform.position = position;
        _loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
        _objectCounts[(int)index]++;
        _dirty = true;

        // Update undo/redo stack
        switch (actionType) {
            case LevelEditorAction.ActionType.Normal:
            case LevelEditorAction.ActionType.Redo:
                _undoStack.Push(new CreateObjectAction(index, spawner));
                break;

            case LevelEditorAction.ActionType.Undo:
                _redoStack.Push(new DeleteObjectAction(spawner, index));
                break;
        }
    }

    /// <summary>
    /// Deletes the given object.
    /// </summary>
    public void DeleteObject(GameObject obj, LevelEditorAction.ActionType actionType) {

        // Get database index of object
        byte deletedObjectIndex = (byte)ObjectIndexOfObject(obj);
        int levelIndex = LevelIndexOfObject(obj);

        // Update undo/redo stack
        var deleteAction = new DeleteObjectAction(obj, deletedObjectIndex);
        switch (actionType) {
            case LevelEditorAction.ActionType.Normal:
            case LevelEditorAction.ActionType.Redo:
                _undoStack.Push(deleteAction);
                break;

            case LevelEditorAction.ActionType.Undo:
                _redoStack.Push(new CreateObjectAction(deletedObjectIndex, obj));
                break;
        }

        // Delete object from level
        _loadedLevel.DeleteObject(_selectedFloor, levelIndex);

        // Delete spawner
        _objectCounts[deletedObjectIndex]--;
        _loadedSpawners.Remove(obj.GetComponent<ObjectSpawner>());
        DestroyLoadedObject(obj);
        _dirty = true;
    }

    /// <summary>
    /// Moves an object.
    /// </summary>
    public void MoveObject(GameObject obj, Vector3 oldPos, Vector3 newPos, LevelEditorAction.ActionType actionType) {
        var moveAction = new MoveObjectAction(obj, oldPos, newPos);
        switch (actionType) {
            case LevelEditorAction.ActionType.Normal:
            case LevelEditorAction.ActionType.Redo:
                _undoStack.Push(moveAction);
                break;

            case LevelEditorAction.ActionType.Undo:
                _redoStack.Push(new MoveObjectAction(obj, newPos, oldPos));
                break;
        }

        //var attribs = AttributesOfObject(obj);
        SetObjectPosition(obj, newPos, actionType);
    }

    public void SetObjectPosition(GameObject obj, Vector3 pos, LevelEditorAction.ActionType actionType) {
        var attribs = AttributesOfObject(obj);
        attribs.SetPosition(pos);
        obj.transform.position = pos;
    }

    public void SetObjectEulerRotation(GameObject obj, Vector3 rot, LevelEditorAction.ActionType actionType) {
        var attribs = AttributesOfObject(obj);
        attribs.SetEulerRotation(rot);
        obj.transform.localRotation = Quaternion.Euler(rot);
    }

    public void SetObject3DScale(GameObject obj, Vector3 scale, LevelEditorAction.ActionType actionType) {
        var attribs = AttributesOfObject(obj);
        attribs.Set3DScale(scale);
        obj.transform.localScale = scale;
    }

    public void RecordAttributeChange(GameObject obj, ChangeObjectNormalAttributeAction.AttributeChanged attrib, Vector3 oldValue, Vector3 newValue, LevelEditorAction.ActionType actionType) {
        switch (actionType) {
            case LevelEditorAction.ActionType.Normal:
            case LevelEditorAction.ActionType.Redo:
                _undoStack.Push(new ChangeObjectNormalAttributeAction(obj, attrib, newValue));
                break;

            case LevelEditorAction.ActionType.Undo:
                _redoStack.Push(new ChangeObjectNormalAttributeAction(obj, attrib, oldValue));
                break;
        }
    }

    /// <summary>
    /// Undoes the most recent action.
    /// </summary>
    public void Undo() {
        if (_undoStack.Count <= 0) return;

        var undoAction = _undoStack.Pop();
        undoAction.Undo();
    }

    /// <summary>
    /// Redoes the most recent action.
    /// </summary>
    public void Redo() {
        if (_redoStack.Count <= 0) return;

        var redoAction = _redoStack.Pop();
        redoAction.Redo();
    }

    /// <summary>
    /// Enables the grid.
    /// </summary>
    public void EnableGrid() {
        _drawGrid = true;
    }

    /// <summary>
    /// Disables the grid.
    /// </summary>
    public void DisableGrid() {
        _drawGrid = false;
    }

    /// <summary>
    /// Starts moving an object.
    /// </summary>
    public void StartMovingObject(GameObject obj) {
        ShowAssetPreview(obj);
        _movingObjectOriginalCoords = obj.transform.position;
        _movingObject = obj;
        //DeleteObject(obj, ActionType.None);
    }

    /// <summary>
    /// Stops moving an object.
    /// </summary>
    public void StopMovingObject() {
        if (_placementAllowed) {
            //CreateObject((byte)_movingObjectID, _cursorPosition, ActionType.None);
            //_movingObject.transform.position = _cursorPosition;
            MoveObject(_movingObject, _movingObjectOriginalCoords, _cursorPosition, LevelEditorAction.ActionType.Normal);
            ResetMovingObject();
        }
    }

    /// <summary>
    /// Resets the object being moved.
    /// </summary>
    public void ResetMovingObject() {
        //CreateObject((byte)_movingObjectID, _movingObjectOriginalCoords, ActionType.None);
        //_movingObjectID = -1;
        _movingObject = null;
        DisablePreview();
    }

    /// <summary>
    /// Selects an asset from the object browser.
    /// </summary>
    /// <param name="index">Index of the object in the browser.</param>
    public void SelectObjectInCategory(int index, ObjectDatabase.Category category) {
        
        var data = ObjectDatabaseManager.Instance.AllObjectsInCategory(category)[index];
        
        _selectedObjectIndexForPlacement = data.index;
        var obj = ObjectDatabaseManager.Instance.GetObject(data.index);

        Debug.Log (string.Format ("Object {0} ({1}) selected from category {2}.", obj.name, index.ToString(), category.ToString()));

        ShowAssetPreview(obj);
    }

    /// <summary>
    /// Creates a new level.
    /// </summary>
    public void CreateNewLevel() {
        _loadedLevel = new Level();
        _dirty = false;
        SetupLevelObjects();
        InitObjectCounts();
        StartEditing();
        onCreateLevel.Invoke();
    }

    /// <summary>
    /// Saves the current level.
    /// </summary>
    public void SaveCurrentLevelToJSON(bool doUseFilePanel = false) {

        if (doUseFilePanel || _loadedLevelPath == "") {
            string savePath = default(string);
            if (Application.isEditor) {
#if UNITY_EDITOR
                savePath = EditorUtility.SaveFilePanel("Save Level to JSON", Application.dataPath, _loadedLevel.Name, "json");
                if (savePath == default(string) || savePath == "") return;
                JSONFileUtility.SaveToJSONFile(_loadedLevel, savePath);
#endif
            } else {
                // In-game editor file panel
            }

            _loadedLevelPath = savePath;
        } else JSONFileUtility.SaveToJSONFile(_loadedLevel, _loadedLevelPath);

        _dirty = false;
        onSaveLevel.Invoke();
    }

    /// <summary>
    /// Closes the currently loaded level.
    /// </summary>
    public void CloseLevel() {
        StopEditing();
        _hoveredObjects.Clear();
        _selectedObjects.Clear();

        _loadedLevel = null;
        _loadedLevelPath = "";
        SelectTool(Tool.Select);
        _selectedFloor = 0;

        _undoStack.Clear();
        _redoStack.Clear();

        CleanupLevel();
        _objectCounts.Clear();
        onCloseLevel.Invoke();
    }

    /// <summary>
    /// Loads a JSON level file from the given path.
    /// </summary>
    public void LoadLevelFromJSON(string path) {
        var asset = JSONFileUtility.LoadFromJSONFile<Level>(path);

        // If asset failed to load
        if (asset == null) {
            Debug.LogError("Failed to load level!");
            return;
        }

        if (_loadedLevel != null) CloseLevel();

#if UNITY_EDITOR
        LastEditedLevelPath = path;
#endif

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
    /// Prompts the user for a level asset to load.
    /// </summary>
    public void LoadLevelFromJSON() {
#if UNITY_EDITOR
        var assetPath = EditorUtility.OpenFilePanel("Open Level JSON", Application.dataPath, "json");

        // If user cancelled loading
        if (assetPath == default(string) || assetPath == "") return;

        LoadLevelFromJSON(assetPath);
#endif
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

            // Skip empty objects
            if (index == byte.MaxValue) continue;

            // Create spawner
            var spawner = CreateSpawner(index);
            spawner.transform.parent = _floorObjects[_selectedFloor].transform;
            spawner.transform.position = attribs.Position;
            spawner.transform.rotation = Quaternion.Euler(attribs.EulerRotation);
            spawner.transform.localScale = attribs.Scale;
            _loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
        }
    }

    /// <summary>
    /// Creates base level objects.
    /// </summary>
    void SetupLevelObjects() {
        _loadedLevelObject = new GameObject(_loadedLevel.Name + " (Loaded Level)", typeof(LevelObject));
        _floorObjects = new GameObject[Level.MAX_FLOORS];
        for (int floor = 0; floor < Level.MAX_FLOORS; floor++) {
            _floorObjects[floor] = new GameObject("Floor " + floor.ToString());
            _floorObjects[floor].transform.SetParent(_loadedLevelObject.transform);
        }
    }

    /// <summary>
    /// Counts the number of each object in the scene.
    /// </summary>
    void GetObjectCounts() {
        foreach (var obj in _loadedSpawners) {
            var index = ObjectIndexOfObject(obj.Template);
            _objectCounts[index]++;
        }
    }

    /// <summary>
    /// Resets the object counter.
    /// </summary>
    void InitObjectCounts() {
        if (_objectCounts == null)
            _objectCounts = new List<int>();

        for (int i = 0; i < (int)byte.MaxValue; i++) {
            if (i >= _objectCounts.Count) _objectCounts.Add (0);
            else _objectCounts[i] = 0;
        }
    }

    /// <summary>
    /// Creates a spawner for the given template.
    /// </summary>
    public GameObject CreateSpawner(GameObject template) {
        GameObject spawner = new GameObject(template.name + " Spawner", typeof(ObjectSpawner));
        spawner.GetComponent<ObjectSpawner>().SetTemplate(template);
        return spawner;
    }

    /// <summary>
    /// Returns a new instance of the object with the given index.
    /// </summary>
    public GameObject CreateSpawner(byte index) {
        var template = ObjectDatabaseManager.Instance.GetObject(index);
        return CreateSpawner(template);
    }

    /// <summary>
    /// Removes all spawners from the current floor.
    /// </summary>
    void CleanupFloor() {
        while (_loadedSpawners.Count > 0) {
            if (_loadedSpawners[0] == null || _loadedSpawners[0].gameObject == null) {
                _loadedSpawners.RemoveAt(0);
                continue;
            }
            DestroyLoadedObject(_loadedSpawners.PopFront().gameObject);
        }
    }

    /// <summary>
    /// Cleans up the floor and level.
    /// </summary>
    void CleanupLevel() {
        CleanupFloor();

        if (_loadedLevelObject != null) {
            DestroyLoadedObject(_loadedLevelObject);
            _loadedLevelObject = null;
        }

        for (int i = 0; i < _floorObjects.Length; i++) _floorObjects[i] = null;
    }

    /// <summary>
    /// Uses the proper function to destroy the given object.
    /// </summary>
    public void DestroyLoadedObject(GameObject obj) {
        if (Application.isEditor) DestroyImmediate(obj);
        else Destroy(obj);
    }

    /// <summary>
    /// Starts playing the currently loaded level.
    /// </summary>
    public void PlayCurrentLevel() {
        //Debug.Log("Play");

        // Activate all spawners
        foreach (ObjectSpawner spawner in _loadedSpawners)
            spawner.Play();

        _playingLevel = true;
    }

    /// <summary>
    /// Stops playing the currently loaded level.
    /// </summary>
    public void StopPlayingLevel() {
        //Debug.Log("Stop");

        // Reset all spawners
        foreach (var spawner in _loadedSpawners)
            spawner.ResetSpawner();

        _playingLevel = false;
    }

    #endregion
    #region Nested Classes

    /// <summary>
    /// Class for LevelManager-specific events
    /// </summary>
    [Serializable]
    public class LevelManagerEvent : UnityEvent { }

    [Serializable]
    public class ByteToIntDict : Dictionary<byte, int> { }

    #endregion
}
