// LevelManager.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[ExecuteInEditMode]
public class LevelManager : SingletonMonoBehaviour<LevelManager> {

    #region Enums

    /// <summary>
    /// Enum for the current tool mode of the editor.
    /// </summary>
    public enum Tool {
        Select = 0,
        Place  = 1,
        Erase  = 2,
        Move   = 3
    }

    #endregion
    #region Nested Classes

    /// <summary>
    /// Class for LevelManager-specific events
    /// </summary>
    [Serializable]
    public class LevelManagerEvent : UnityEvent { }

    #endregion
    #region Constants

    /// <summary>
    /// Default level path.
    /// </summary>
    const string _LEVEL_PATH = "Assets/Levels/";

    /// <summary>
    /// Path for temporary level file.
    /// </summary>
    const string _TEMP_LEVEL_PATH = "Assets/Levels/~Temp.asset";

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
    [NonSerialized] Level _loadedLevel;

    /// <summary>
    /// Resource path of current loaded level.
    /// </summary>
    string _loadedLevelPath = default(string);

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
    List<GameObject> _loadedObjects = new List<GameObject>();

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
    const string _PREVIEW_MAT_PATH = "Assets/Materials/PreviewGhost.mat";

    /// <summary>
    /// Is object placement allowed at the current location?
    /// </summary>
    bool _placementAllowed = true;

    /// <summary>
    /// Index of the currently selected object.
    /// </summary>
    int _selectedObjectIndex;

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

    /// <summary>
    /// Was the last mouse movement a drag?
    /// </summary>
    bool _lastMouseMoveWasDrag = false;

    int _currentYPosition = 0;

    int _currentYRotation = 0;

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
    public Vector3 CursorPosition { get { return _cursorPosition; } }

    public bool MouseOverUI {
        get { return _mouseOverUI; }
        set { _mouseOverUI = value; }
    }

    #endregion
    #region Unity Callbacks

    new void Awake () {
        base.Awake();

        if (Application.isPlaying)
            DontDestroyOnLoad (gameObject);
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
        _selectedObjectIndex = -1;
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

    public void HandleInputs(Event currEvent, Camera camera) {

        var invertedMouse = Event.current.mousePosition;
        var screenHeight = Screen.height;
        invertedMouse.y = screenHeight * 0.925f - invertedMouse.y;
        var ray = camera.ScreenPointToRay(invertedMouse);
        float distance;
        if (_editingPlane.Raycast(ray, out distance)) {
            _cursorPosition = ray.GetPoint(distance);
            _cursorPosition.y += 0.5f;
        }

        if (currEvent.isMouse) {
            switch (currEvent.type) {
                case EventType.MouseDrag:
                    _lastMouseMoveWasDrag = true;
                    break;

                case EventType.MouseDown:
                    _lastMouseMoveWasDrag = false;
                    break;

                case EventType.MouseUp:
                    switch (currEvent.button) {
                        case 0: // Left click
                            HandleLeftClick();
                            break;
                        case 1: // Right click
                            HandleRightClick();
                            break;
                    }
                    break;
            }

            HandleMouseMove();
        }
    }

    public void HandleLeftClick() {

        if (_mouseOverUI) return;
        if (_lastMouseMoveWasDrag) return;

        switch (_currentTool) {
            case Tool.Select:
                if (LevelLoaded && CheckPlacement()) {
                    var selectedObject = Selection.activeGameObject;

                    if (selectedObject != null) {
                        _selectedObject = selectedObject;
                        onSelectObject.Invoke();
                    } else _selectedObject = null;
                }
                break;

            case Tool.Place:
                if (_selectedObjectIndex != -1 && CheckPlacement()  && CanPlaceAnotherObject(_selectedObjectIndex)) {
                    if (_snapToGrid) SnapCursor();
                    CreateObject((byte)_selectedObjectIndex, _cursorPosition);
                    Event.current.Use();
                }
                break;

            case Tool.Erase:
                DeleteObject(Selection.activeGameObject);
                break;

            case Tool.Move:
                if (_movingObjectID == -1) { // Not currently moving
                    var clickedObj = Selection.activeGameObject;
                    var clickedIndex = _loadedObjects.IndexOf (clickedObj);
                    var clickedData = _loadedLevel[_selectedFloor][clickedIndex];
                    var clickedID = clickedData.index;
                    if (clickedID == byte.MaxValue) return; // Nothing selected

                    _movingObjectID = clickedID;
                    Show3DAssetPreview(clickedObj);
                    _movingObjectOriginalCoords = clickedData.Position;
                    DeleteObject(clickedObj);
                } else { // Currently moving
                    if (_placementAllowed) {
                        CreateObject((byte)_selectedObjectIndex, _cursorPosition);
                        _movingObjectID = -1;
                        DisablePreview();
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Handles right clicks in the scene view.
    /// </summary>
    void HandleRightClick() {
        if (_mouseOverUI) return;

        if (!_lastMouseMoveWasDrag) {

            if (_currentTool == Tool.Move && _movingObjectID != -1) {
                CreateObject((byte)_movingObjectID, _movingObjectOriginalCoords);
                _movingObjectID = -1;
            }

            _selectedObjectIndex = -1;
            DisablePreview();
            _currentTool = Tool.Select;
        }
    }

    /// <summary>
    /// Handles mouse movement in the scene view.
    /// </summary>
    void HandleMouseMove() {

        //_mouseOverUI = 

        if (_preview == null) return;

        _preview.transform.position = _cursorPosition;

        if (LevelLoaded && _snapToGrid)
            SnapCursor();
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

    public Level.ObjectAttributes AttributesOfObject (GameObject obj) {
        var index = LevelIndexOfObject (obj);
        return _loadedLevel[_selectedFloor][index];
    }

    public Vector3 PositionOfObject (GameObject obj) {
        var attribs = AttributesOfObject (obj);
        return attribs.Position;
    }

    public int LevelIndexOfObject (GameObject obj) {
        return _loadedObjects.IndexOf (obj);
    }

    public byte ObjectIndexOfObject (GameObject obj) {
        var index = LevelIndexOfObject (obj);
        var data = _loadedLevel[_selectedFloor][index];
        return data.index;
    }

    public void UpOneFloor () {
        if (_selectedFloor < Level.MAX_FLOORS - 1) {
            CleanupObjects();
            _selectedFloor++;
            _currentYPosition = 0;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    public void DownOneFloor () {
        if (_selectedFloor > 0) {
            CleanupObjects();
            _selectedFloor--;
            _currentYPosition = 0;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    public void IncrementY () {
        if (_currentYPosition < Level.FLOOR_HEIGHT - 1) {
            _currentYPosition++;
            UpdateHeight();
        }
    }

    public void DecrementY () {
        if (_currentYPosition > 0) {
            _currentYPosition--;
            UpdateHeight();
        }
    }

    public void UpdateHeight () {
        _editingPlane.SetNormalAndPosition (new Vector3 (0f, _selectedFloor * Level.FLOOR_HEIGHT + _currentYPosition, 0f), Vector3.up);
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

    public bool CanPlaceAnotherObject (int index) {
        var limit = ObjectDatabaseManager.Instance.GetObjectLimit (index);
        if (limit < 0) return true;
        
        else return _objectCounts[(byte)index] < limit;
    }

    /// <summary>
    /// Draws the 3D cursor in the scene view.
    /// </summary>
    public void DrawCursor(SceneView sc) {
        if (_preview == null) return;
        switch (_currentTool) {
            case Tool.Select:
                DrawCube(_cursorPosition, 1.05f, 0f, Color.white);
                if (_selectedObject != null) DrawCube(_cursorPosition, 1.1f, -_selectedObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad, Color.cyan);
                break;

            case Tool.Place:
                if (CheckPlacement()  && CanPlaceAnotherObject(_selectedObjectIndex))
                    DrawCube(_cursorPosition, 1f, 0f, Color.green);
                else DrawCube(_cursorPosition, 1f, 0f, Color.gray);
                break;

            case Tool.Erase:
                DrawCube(_cursorPosition, 1.1f, 0f, Color.red);
                break;

            case Tool.Move:
                if (CheckPlacement()) DrawCube(_cursorPosition, 1.1f, 0f, Color.blue);
                else DrawCube(_cursorPosition, 1.1f, 0f, Color.gray);
                break;
        }
    }

    /// <summary>
    /// Draws a wireframe cube.
    /// </summary>
    void DrawCube(Vector3 center, float edgeWidth, float rotation, Color color) {
        float halfWidth = edgeWidth / 2f;
        float theta = Mathf.PI / 4f;
        float hyp = halfWidth / Mathf.Sin(theta);

        Vector3 up1 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation - theta),
            center.y + halfWidth,
            center.z + hyp * Mathf.Sin(rotation - theta)
        );
        Vector3 up2 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation + theta),
            center.y + halfWidth,
            center.z + hyp * Mathf.Sin(rotation + theta)
        );
        Vector3 up3 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation + 3 * theta),
            center.y + halfWidth,
            center.z + hyp * Mathf.Sin(rotation + 3 * theta)
        );
        Vector3 up4 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation + 5 * theta),
            center.y + halfWidth,
            center.z + hyp * Mathf.Sin(rotation + 5 * theta)
        );
        Vector3 dn1 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation - theta),
            center.y - halfWidth,
            center.z + hyp * Mathf.Sin(rotation - theta)
        );
        Vector3 dn2 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation + theta),
            center.y - halfWidth,
            center.z + hyp * Mathf.Sin(rotation + theta)
        );
        Vector3 dn3 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation + 3 * theta),
            center.y - halfWidth,
            center.z + hyp * Mathf.Sin(rotation + 3 * theta)
        );
        Vector3 dn4 = new Vector3(
            center.x + hyp * Mathf.Cos(rotation + 5 * theta),
            center.y - halfWidth,
            center.z + hyp * Mathf.Sin(rotation + 5 * theta)
        );

        Handles.color = color;
        Handles.DrawLine(up1, up2);
        Handles.DrawLine(up2, up3);
        Handles.DrawLine(up3, up4);
        Handles.DrawLine(up4, up1);
        Handles.DrawLine(up1, dn1);
        Handles.DrawLine(up2, dn2);
        Handles.DrawLine(up3, dn3);
        Handles.DrawLine(up4, dn4);
        Handles.DrawLine(dn1, dn2);
        Handles.DrawLine(dn2, dn3);
        Handles.DrawLine(dn3, dn4);
        Handles.DrawLine(dn4, dn1);
    }

    /// <summary>
    /// Draws the asset placement grid.
    /// </summary>
    public void DrawGrid(SceneView sc) {
        if (!_drawGrid) return;

        var height = _selectedFloor * Level.FLOOR_HEIGHT + _currentYPosition;

        Color lineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Color edgeColor = new Color(0.65f, 0.65f, 0.65f, 0.65f);

        for (int x = 0; x <= Level.FLOOR_WIDTH; x++) {
            Handles.color = (x == 0 || x == Level.FLOOR_WIDTH) ? edgeColor : lineColor;

            Handles.DrawLine(new Vector3(x - 0.5f, height, -0.5f),
                new Vector3(x - 0.5f, height, Level.FLOOR_DEPTH - 0.5f));
        }

        for (int z = 0; z <= Level.FLOOR_DEPTH; z++) {
            Handles.color = (z == 0 || z == Level.FLOOR_DEPTH) ? edgeColor : lineColor;

            Handles.DrawLine(new Vector3(-0.5f, height, z - 0.5f), new Vector3(Level.FLOOR_WIDTH - 0.5f, height, z - 0.5f));
        }
    }

    public void DrawRoomBounds (SceneView sc) {
        var u1 = new Vector3 (-0.5f, (_selectedFloor + 1) * Level.FLOOR_HEIGHT, -0.5f);
        var u2 = new Vector3 (Level.FLOOR_WIDTH-0.5f, (_selectedFloor + 1) * Level.FLOOR_HEIGHT, -0.5f);
        var u3 = new Vector3 (Level.FLOOR_WIDTH-0.5f, (_selectedFloor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH-0.5f);
        var u4 = new Vector3 (-0.5f, (_selectedFloor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH-0.5f);

        var d1 = new Vector3 (-0.5f, (_selectedFloor) * Level.FLOOR_HEIGHT, -0.5f);
        var d2 = new Vector3 (Level.FLOOR_WIDTH-0.5f, (_selectedFloor) * Level.FLOOR_HEIGHT, -0.5f);
        var d3 = new Vector3 (Level.FLOOR_WIDTH-0.5f, (_selectedFloor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH-0.5f);
        var d4 = new Vector3 (-0.5f, (_selectedFloor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH-0.5f);

        Handles.color = Color.white;
        Handles.DrawLine (u1, u2);
        Handles.DrawLine (u2, u3);
        Handles.DrawLine (u3, u4);
        Handles.DrawLine (u4, u1);

        Handles.DrawLine (u1, d1);
        Handles.DrawLine (u2, d2);
        Handles.DrawLine (u3, d3);
        Handles.DrawLine (u4, d4);

        Handles.DrawLine (d1, d2);
        Handles.DrawLine (d2, d3);
        Handles.DrawLine (d3, d4);
        Handles.DrawLine (d4, d1);
    }

    public bool HasSelectedObject { get { return _selectedObject != null; } }

    public void CreateObject(byte index, Vector3 position, bool clearRedoStack = true) {
        _loadedLevel.AddObject ((int)index, _selectedFloor, position, _currentYRotation);

        GameObject instance = SpawnObject(index);
        instance.transform.SetParent(_floorObjects[_selectedFloor].transform);
        instance.transform.position = position;
        _loadedObjects.Add (instance);
        _objectCounts[index]++;
        _dirty = true;
        _undoStack.Push(new CreateObjectAction(index, position, instance));
        if (clearRedoStack) _redoStack.Clear();
    }

    public void DeleteObject(GameObject obj, bool clearRedoStack = true) {
        byte deletedObjectIndex = (byte)ObjectIndexOfObject (obj);
        int levelIndex = LevelIndexOfObject (obj);
        _loadedLevel.DeleteObject (_selectedFloor, levelIndex);
        _objectCounts[deletedObjectIndex]--;
        _undoStack.Push(new DeleteObjectAction(obj, deletedObjectIndex));
        DestroyLoadedObject(obj);
        _dirty = true;
        if (clearRedoStack) _redoStack.Clear();
    }

    public void Undo() {
        if (_undoStack.Count <= 0) return;

        var undoAction = _undoStack.Pop();
        AddRedo(undoAction);
        undoAction.Undo();
    }

    public void Redo() {
        if (_redoStack.Count <= 0) return;

        var redoAction = _redoStack.Pop();
        AddUndo(redoAction);
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

    public void RotateSelectedObject(int rotation) {
        var attribs = AttributesOfObject (_selectedObject);
        attribs.yRotation = (attribs.yRotation + rotation) % 360;
        _selectedObject.transform.Rotate(0f, rotation, 0f);
    }

    /// <summary>
    /// Selects an asset from the object browser.
    /// </summary>
    /// <param name="index">Index of the object in the browser.</param>
    public void SelectObjectInCategory(int index, ObjectDatabase.Category category) {
        var data = ObjectDatabaseManager.Instance.AllObjectsInCategory(category)[index];
        _selectedObjectIndex = data.index;
        var obj = ObjectDatabaseManager.Instance.GetObject(index);

        Show3DAssetPreview(obj);
    }

    /// <summary>
    /// Creates a new level.
    /// </summary>
    public void CreateNewLevel() { 
        _loadedLevel = new Level();

        SetupLevelObjects();

        InitObjectCounts ();

        onCreateLevel.Invoke();
    }

    /// <summary>
    /// Saves the current level.
    /// </summary>
    public void SaveCurrentLevel(bool doUseFilePanel=false) {
        var asset = _loadedLevel.ToLevelAsset();

        if (asset == default(LevelAsset)) {
            Debug.LogError("Failed to save level!");
            return;
        }

        if (doUseFilePanel || _loadedLevelPath == default(string)) {
            string savePath = default(string);
            if (Application.isEditor) {
                savePath = ScriptableObjectUtility.SaveScriptableObjectWithFilePanel(asset, "Save Level File", asset.ToString(), "asset");
            } else {
                // In-game editor file panel
            }
            if (savePath == default(string) || savePath == "") return;
            _loadedLevelPath = savePath;
        } else {
            ScriptableObjectUtility.SaveScriptableObject(asset, _loadedLevelPath, true);
        }

        _dirty = false;

        onSaveLevel.Invoke();
    }

    /// <summary>
    /// Closes the currently loaded level.
    /// </summary>
    public void CloseLevel() {
        _loadedLevel = null;
        _loadedLevelPath = default(string);
        SelectTool(Tool.Select);
        _selectedFloor = 0;


        _undoStack.Clear();
        _redoStack.Clear();

        CleanupObjects();

        _objectCounts.Clear();

        onCloseLevel.Invoke();
    }

    /// <summary>
    /// Prompts the user for a level asset to load.
    /// </summary>
    public void LoadLevel() {

        string assetPath = default(string);
        LevelAsset asset = null;
        if (Application.isEditor)
            asset = ScriptableObjectUtility.LoadScriptableObjectWithFilePanel<LevelAsset>("Load Level File", "Assets", "asset", out assetPath);
        // else open load panel in-game

        // If user cancelled loading
        if (assetPath == default(string) || assetPath == "") return;

        // If asset failed to load
        if (asset == null) {
            Debug.LogError("Failed to load level!");
            return;
        }

        AssetDatabase.CopyAsset (assetPath, "Assets/Levels/~Temp.asset");
        var assetCopy = ScriptableObjectUtility.LoadScriptableObject<LevelAsset> (_TEMP_LEVEL_PATH);

        if (_loadedLevel != null) CloseLevel();

        Level level = assetCopy.Unpack();
        if (level.Equals(default(Level))) {
            Debug.LogError("Level file is corrupted at " + asset);
            return;
        }

        _loadedLevel = level;

        _dirty = false;

        SetupLevelObjects();

        GetObjectCounts();

        ReconstructFloor();

        onLoadLevel.Invoke();
    }

    /// <summary>
    /// Spawns objects from the currently loaded level.
    /// </summary>
    void ReconstructFloor() {

        Debug.Log(string.Format("LEDIT: Loading floor {0}...", _selectedFloor.ToString()));
        foreach (var attribs in _loadedLevel[_selectedFloor].Objects) {
            var obj = SpawnObject (attribs.index);
            obj.transform.parent = _floorObjects[_selectedFloor].transform;
            obj.transform.position = attribs.Position;
            _loadedObjects.Add (obj);
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

    void GetObjectCounts () {
        foreach (var obj in _loadedObjects) {
            var index = ObjectIndexOfObject (obj);
            _objectCounts[index]++;
        }
    }

    void InitObjectCounts () {
        _objectCounts.Clear();

        for (int i = 0; i < (int)byte.MaxValue; i++)
            _objectCounts.Add ((byte)i, 0);
    }

    /// <summary>
    /// Returns a new instance of the object with the given index.
    /// </summary>
    public GameObject SpawnObject(byte index) {
        var template = ObjectDatabaseManager.Instance.GetObject(index);
        if (Application.isEditor) {
            return (GameObject)PrefabUtility.InstantiatePrefab(template);
        } else {
            return (GameObject)Instantiate(template);
        }
    }

    /// <summary>
    /// Cleans up level objects in the scene.
    /// </summary>
    void CleanupObjects() {
        if (_loadedObjects == null) return;

        DestroyLoadedObject (_loadedLevelObject);

        while (_loadedObjects.Count > 0) DestroyLoadedObject (_loadedObjects.PopFront());
    }

    void DestroyLoadedObject(GameObject obj) {
        if (Application.isEditor) DestroyImmediate(obj);
        else Destroy(obj);
    }

    #endregion
}
