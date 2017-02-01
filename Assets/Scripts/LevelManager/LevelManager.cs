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
        Select,
        Place,
        Erase,
        Move
    }

    #endregion
    #region Nested Classes

    public class LevelManagerEvent : UnityEvent { }

    #endregion
    #region Vars

    [System.NonSerialized]
    Level _loadedLevel;

    bool _dirty = false;

    GameObject[,,,] _loadedObjects =
        new GameObject[Level.MAX_FLOORS, Level.FLOOR_WIDTH,
        Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH];

    /// <summary>
    /// Default level path.
    /// </summary>
    const string _LEVEL_PATH = "Assets/Levels/";

    /// <summary>
    /// GameObject representing the loaded level.
    /// </summary>
    GameObject _loadedLevelObject;

    /// <summary>
    /// GameObjects representing the floors of the loaded level.
    /// </summary>
    GameObject[] _floorObjects;

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
    /// Current state of the editor tool.
    /// </summary>
    Tool _currentTool = Tool.Select;

    /// <summary>
    /// ID of object being moved.
    /// </summary>
    int _movingObjectID = -1;

    /// <summary>
    /// Original coordinates of object being moved;
    /// </summary>
    Level.CoordinateSet _movingObjectOriginalCoords;

    const string _PREVIEW_NAME = "~LevelEditorPreview";

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

    Level.CoordinateSet _selectedObjectCoords;

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

    const int _MAX_UNDOS = 32;

    int _currentY = 0;

    [SerializeField]
    Stack<LevelEditorAction> _undoStack = new Stack<LevelEditorAction>();

    [SerializeField]
    Stack<LevelEditorAction> _redoStack = new Stack<LevelEditorAction>();

    Dictionary<byte, int> _objectCounts = new Dictionary<byte, int>();

    public LevelManagerEvent onCreateLevel = new LevelManagerEvent();
    public LevelManagerEvent onLoadLevel = new LevelManagerEvent();
    public LevelManagerEvent onSaveLevel = new LevelManagerEvent();
    public LevelManagerEvent onCloseLevel = new LevelManagerEvent();

    #endregion
    #region Properties

    public bool LevelLoaded { get { return _loadedLevel != null; } }

    public bool Dirty { get { return _dirty; } }

    public string LoadedLevelName { get { return _loadedLevel.name; } }

    public bool GridEnabled { get { return _drawGrid; } }

    public ObjectDatabase.Category CurrentObjectFilter { get { return _filter; } }

    public List<ObjectData> FilteredObjects { get { return _filteredObjects; } }

    public int SelectedFloor { get { return _selectedFloor; } }

    public int CurrentYValue { get { return _currentY; } }

    #endregion
    #region Unity Callbacks

    #endregion
    #region Methods

    public void StartEditing() {
        if (_preview != null) {
            if (Application.isEditor) DestroyImmediate(_preview);
            else Destroy(_preview);
        }

        /*while (true) {
            var preview = GameObject.Find (_PREVIEW_NAME);
            if (preview == null) break;
            DestroyObject (preview);
        }*/

        _filter = ObjectDatabase.Category.Block;
        _filteredObjects = ObjectDatabaseManager.Instance.AllObjectsInCategory(_filter);
        _preview = new GameObject(_PREVIEW_NAME, typeof(MeshFilter), typeof(MeshRenderer));
        //_preview.hideFlags = HideFlags.HideAndDontSave;
        _selectedObjectIndex = -1;
        _selectedFloor = 0;
        _currentY = 0;
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
        _loadedLevel.name = newName;
        _loadedLevelObject.name = newName + " (Loaded Level)";
    }

    public void HandleInputs(Event currEvent, Camera camera) {

        var invertedMouse = Event.current.mousePosition;
        var screenHeight = Screen.height;
        invertedMouse.y = screenHeight * 0.925f - invertedMouse.y;
        var ray = camera.ScreenPointToRay(invertedMouse);
        float distance;
        var point = default(Vector3);
        if (_editingPlane.Raycast(ray, out distance)) {
            point = ray.GetPoint(distance);
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

            HandleMouseMove(point);
        }
    }

    public void HandleLeftClick() {

        if (_lastMouseMoveWasDrag) return;

        int x = PreviewArrayX();
        int y = PreviewArrayY();
        int z = PreviewArrayZ();

        switch (_currentTool) {
            case Tool.Select:
                if (CheckPlacement()) {
                    var selectedObject = _loadedObjects[_selectedFloor, x, y, z];

                    if (selectedObject != null) {
                        _selectedObject = selectedObject;
                        _selectedObjectCoords = new Level.CoordinateSet(_selectedFloor, x, y, z);
                    } else _selectedObject = null;
                }
                break;

            case Tool.Place:
                if (_selectedObjectIndex != -1 && _placementAllowed) {
                    CreateObject((byte)_selectedObjectIndex, _selectedFloor, (byte)x, (byte)y, (byte)z);
                    Event.current.Use();
                }
                break;

            case Tool.Erase:
                DeleteObject(_selectedFloor, PreviewArrayX(), PreviewArrayY(), PreviewArrayZ());
                break;

            case Tool.Move:
                if (_movingObjectID == -1) { // Not currently moving
                    var clickedObj = _loadedLevel[_selectedFloor][x, y, z];
                    var clickedID = clickedObj.index;
                    if (clickedID == byte.MaxValue) return; // Nothing selected

                    _movingObjectID = clickedID;
                    Show3DAssetPreview(_loadedObjects[_selectedFloor, x, y, z]);
                    _movingObjectOriginalCoords = new Level.CoordinateSet(_selectedFloor, x, y, z);
                    DeleteObject(_movingObjectOriginalCoords);
                } else { // Currently moving
                    if (_placementAllowed) {
                        CreateObject((byte)_selectedObjectIndex, _selectedObjectIndex, x, y, z);
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
    void HandleMouseMove(Vector3 previewPosition) {

        if (_preview == null) return;

        _preview.transform.position = previewPosition;

        if (LevelLoaded)
            SnapPreview();
    }

    /// <summary>
    /// Shows a 3D preview of the currently selected asset.
    /// </summary>
    void Show3DAssetPreview(GameObject obj) {
        var meshFilter = _preview.GetComponent<MeshFilter>();
        var meshRenderer = _preview.GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = obj.GetComponentInChildren<MeshFilter>().sharedMesh;
        meshRenderer.enabled = true;
        meshRenderer.sharedMaterial = _previewMaterial;

        var color = meshRenderer.sharedMaterial.color;
        color.a = 0.25f;
        meshRenderer.sharedMaterial.color = color;
    }

    /// <summary>
    /// Hides the 3D asset preview.
    /// </summary>
    void DisablePreview() {
        if (_preview == null) return;
        _preview.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Returns the array position of the 3D preview.
    /// </summary>
    int PreviewArrayX() {
        return Mathf.RoundToInt(_preview.transform.position.x);
    }

    /// <summary>
    /// Returns the array position of the 3D preview.
    /// </summary>
    int PreviewArrayY() {
        return Mathf.RoundToInt(_preview.transform.position.y);
    }

    /// <summary>
    /// Returns the array position of the 3D preview.
    /// </summary>
    int PreviewArrayZ() {
        return Mathf.RoundToInt(_preview.transform.position.z);
    }

    public Plane EditingPlane { get { return _editingPlane; } }

    public void UpOneFloor () {
        if (_selectedFloor < Level.MAX_FLOORS - 1) {
            CleanupObjects();
            _selectedFloor++;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    public void DownOneFloor () {
        if (_selectedFloor > 0) {
            CleanupObjects();
            _selectedFloor--;
            UpdateHeight();
            ReconstructFloor();
        }
    }

    public void IncrementY () {
        if (_currentY < Level.FLOOR_HEIGHT - 1) {
            _currentY++;
            UpdateHeight();
        }
    }

    public void DecrementY () {
        if (_currentY > 0) {
            _currentY--;
            UpdateHeight();
        }
    }

    public void UpdateHeight () {
        _editingPlane.SetNormalAndPosition (new Vector3 (0f, _selectedFloor * Level.FLOOR_HEIGHT + _currentY, 0f), Vector3.up);
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
    public void SnapPreview() {
        var pos = _preview.transform.position;
        var snapped = new Vector3(
                Mathf.Round(pos.x),
                Mathf.Round(pos.y),
                Mathf.Round(pos.z)
            );
        _preview.transform.position = snapped;
        _placementAllowed = CheckPlacement();
    }

    /// <summary>
    /// Checks the placement of the preview asset to see if it is valid.
    /// </summary>
    public bool CheckPlacement() {
        // Check x in range
        int x = PreviewArrayX();
        if (x < 0 || x >= Level.FLOOR_WIDTH) return false;

        // Check y in range
        int y = PreviewArrayY();
        if (y < 0 || y >= Level.FLOOR_HEIGHT) return false;

        // Check z in range
        int z = PreviewArrayZ();
        if (z < 0 || z >= Level.FLOOR_DEPTH) return false;

        // Check if obstructed
        if (_loadedLevel[_selectedFloor][x, y, z].index != byte.MaxValue)
            return false;

        return true;
    }

    /// <summary>
    /// Draws the 3D cursor in the scene view.
    /// </summary>
    public void DrawCursor(SceneView sc) {
        if (_preview == null) return;
        switch (_currentTool) {
            case Tool.Select:
                DrawCube(_preview.transform.position, 1.05f, 0f, Color.white);
                if (_selectedObject != null) DrawCube(_selectedObject.transform.position, 1.1f, -_selectedObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad, Color.cyan);
                break;

            case Tool.Place:
                if (_placementAllowed)
                    DrawCube(_preview.transform.position, 1f, 0f, Color.green);
                else DrawCube(_preview.transform.position, 1f, 0f, Color.gray);
                break;

            case Tool.Erase:
                DrawCube(_preview.transform.position, 1.1f, 0f, Color.red);
                break;

            case Tool.Move:
                if (_placementAllowed) DrawCube(_preview.transform.position, 1.1f, 0f, Color.blue);
                else DrawCube(_preview.transform.position, 1.1f, 0f, Color.gray);
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

        Debug.DrawLine(up1, up2, color, 16f, false);
        Debug.DrawLine(up2, up3, color, 0f, false);
        Debug.DrawLine(up3, up4, color, 0f, false);
        Debug.DrawLine(up4, up1, color, 0f, false);
        Debug.DrawLine(up1, dn1, color, 0f, false);
        Debug.DrawLine(up2, dn2, color, 0f, false);
        Debug.DrawLine(up3, dn3, color, 0f, false);
        Debug.DrawLine(up4, dn4, color, 0f, false);
        Debug.DrawLine(dn1, dn2, color, 0f, false);
        Debug.DrawLine(dn2, dn3, color, 0f, false);
        Debug.DrawLine(dn3, dn4, color, 0f, false);
        Debug.DrawLine(dn4, dn1, color, 0f, false);
    }

    /// <summary>
    /// Draws the asset placement grid.
    /// </summary>
    public void DrawGrid(SceneView sc) {
        if (!_drawGrid) return;

        var height = _selectedFloor * Level.FLOOR_HEIGHT + _currentY;

        Color lineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Color edgeColor = new Color(0.65f, 0.65f, 0.65f, 0.65f);

        for (int x = 0; x <= Level.FLOOR_WIDTH; x++) {
            Color color = (x == 0 || x == Level.FLOOR_WIDTH) ? edgeColor : lineColor;

            Debug.DrawLine(new Vector3(x - 0.5f, height, -0.5f),
                new Vector3(x - 0.5f, height, Level.FLOOR_DEPTH - 0.5f),
                color, 0, true);
        }

        for (int z = 0; z <= Level.FLOOR_DEPTH; z++) {
            Color color = (z == 0 || z == Level.FLOOR_DEPTH) ? edgeColor : lineColor;

            Debug.DrawLine(new Vector3(-0.5f, height, z - 0.5f), new Vector3(Level.FLOOR_WIDTH - 0.5f, height, z - 0.5f),
                color, 0, true);
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

        Debug.DrawLine (u1, u2);
        Debug.DrawLine (u2, u3);
        Debug.DrawLine (u3, u4);
        Debug.DrawLine (u4, u1);

        Debug.DrawLine (u1, d1);
        Debug.DrawLine (u2, d2);
        Debug.DrawLine (u3, d3);
        Debug.DrawLine (u4, d4);

        Debug.DrawLine (d1, d2);
        Debug.DrawLine (d2, d3);
        Debug.DrawLine (d3, d4);
        Debug.DrawLine (d4, d1);
    }

    public byte GetIndex(Level.CoordinateSet coords) {
        return _loadedLevel[coords.floor][coords.x, coords.y, coords.z].index;
    }

    public bool HasSelectedObject { get { return _selectedObject != null; } }

    public GameObject SelectedObject { get { return _selectedObject; } }

    public void CreateObject(byte index, Level.CoordinateSet coords, bool clearRedoStack = true) {
        _loadedLevel[coords.floor][coords.x, coords.y, coords.z].index = index;
        GameObject instance = SpawnObject(index);
        instance.transform.SetParent(_floorObjects[coords.floor].transform);
        instance.transform.position = new Vector3(coords.x, coords.floor * Level.FLOOR_HEIGHT + coords.y, coords.z);
        _loadedObjects[coords.floor, coords.x, coords.y, coords.z] = instance;
        _dirty = true;
        _undoStack.Push(new CreateObjectAction(index, coords));
        if (clearRedoStack) _redoStack.Clear();
    }

    public void CreateObject(byte index, int floor, int x, int y, int z, bool clearRedoStack = true) {
        CreateObject(index, new Level.CoordinateSet(floor, x, y, z), clearRedoStack);
    }

    public void DeleteObject(Level.CoordinateSet coords, bool clearRedoStack = true) {
        byte deletedObjectIndex = _loadedLevel[coords.floor][coords.x, coords.y, coords.z].index;
        _loadedLevel[coords.floor][coords.x, coords.y, coords.z].index = byte.MaxValue;
        var obj = _loadedObjects[coords.floor, coords.x, coords.y, coords.z];
        DestroyLoadedObject(obj);
        _dirty = true;
        _undoStack.Push(new DeleteObjectAction(deletedObjectIndex, coords));
        if (clearRedoStack) _redoStack.Clear();
    }

    public void DeleteObject(int floor, int x, int y, int z, bool clearRedoStack = true) {
        DeleteObject(new Level.CoordinateSet(floor, x, y, z), clearRedoStack);
    }

    public void Undo() {
        if (_undoStack.Count <= 0) return;

        var undoAction = _undoStack.Pop();
        var redoObj = undoAction.ToRedo();
        undoAction.Undo();
        AddRedo(redoObj);
    }

    public void Redo() {
        if (_redoStack.Count <= 0) return;

        var redoAction = _redoStack.Pop();
        var undoObj = redoAction.ToUndo();
        redoAction.Redo();
        AddUndo(undoObj);
    }

    public void AddRedo(LevelEditorAction redo) {
        Debug.Log(redo.ToString());
        _redoStack.Push(redo);
    }

    public void AddUndo(LevelEditorAction undo) {
        Debug.Log(undo.ToString());
        _undoStack.Push(undo);
    }

    public void EnableGrid() {
        _drawGrid = true;
    }

    public void DisableGrid() {
        _drawGrid = false;
    }

    public void RotateSelectedObject(int rotation) {
        var obj = _loadedLevel.ObjectAt(_selectedObjectCoords);
        obj.yRotation = (obj.yRotation + rotation) % 360;
        _selectedObject.transform.Rotate(0f, rotation, 0f);
    }

    /// <summary>
    /// Selects an asset from the object browser.
    /// </summary>
    /// <param name="index">Index of the object in the browser.</param>
    public void SelectObjectInCategory(int index, ObjectDatabase.Category category) {
        var data = ObjectDatabaseManager.Instance.AllObjectsInCategory(category)[index];
        _selectedObjectIndex = index;
        var obj = ObjectDatabaseManager.Instance.GetObject(index);

        Show3DAssetPreview(obj);
    }

    /// <summary>
    /// Creates a new level.
    /// </summary>
    public void CreateNewLevel() {
        _loadedLevel = new Level();

        SetupLevelObjects();

        onCreateLevel.Invoke();
    }

    /// <summary>
    /// Saves the current level.
    /// </summary>
    public void SaveCurrentLevel() {
        var asset = _loadedLevel.ToLevelAsset();

        if (asset == default(LevelAsset)) {
            Debug.LogError("Failed to save level!");
            return;
        }

        ScriptableObjectUtility.SaveScriptableObjectWithFilePanel(asset, "Save Level File", asset.ToString(), "asset");

        _dirty = false;

        onSaveLevel.Invoke();
    }

    /// <summary>
    /// Closes the currently loaded level.
    /// </summary>
    public void CloseLevel() {
        _loadedLevel = null;
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

        string assetPath;
        LevelAsset asset = ScriptableObjectUtility.LoadScriptableObjectWithFilePanel<LevelAsset>("Load Level File", "Assets", "asset", out assetPath);

        // If user cancelled loading
        if (assetPath == default(string) || assetPath == "") return;

        // If asset failed to load
        if (asset == null) {
            Debug.LogError("Failed to load level!");
            return;
        }

        if (_loadedLevel != null) CloseLevel();

        Level level = asset.Unpack();
        if (level.Equals(default(Level))) {
            Debug.LogError("Level file is corrupted at " + asset);
            return;
        }

        _loadedLevel = level;

        _dirty = false;

        GetObjectCounts();

        ReconstructFloor();

        onLoadLevel.Invoke();
    }

    /// <summary>
    /// Spawns objects from the currently loaded level.
    /// </summary>
    void ReconstructFloor() {
        SetupLevelObjects();

        Debug.Log(string.Format("LEDIT: Loading floor {0}...", _selectedFloor.ToString()));
        for (int x = 0; x < Level.FLOOR_WIDTH; x++) {
            for (int y = 0; y < Level.FLOOR_HEIGHT; y++) {
                for (int z = 0; z < Level.FLOOR_DEPTH; z++) {
                    byte value = _loadedLevel[_selectedFloor][x, y, z].index;

                    //Debug.Log (string.Format ("Floor {0}: {1}, {2}, {3} = {4}", floor, x, y, z, value));

                    // Skip empty tiles
                    if (value == byte.MaxValue) continue;

                    var obj = SpawnObject(value);
                    obj.transform.parent = _floorObjects[_selectedFloor].transform;
                    obj.transform.position = new Vector3(x, _selectedFloor * Level.FLOOR_HEIGHT + y, z);
                }
            }
        }
    }

    /// <summary>
    /// Creates base level objects.
    /// </summary>
    void SetupLevelObjects() {
        _loadedLevelObject = new GameObject(_loadedLevel.name + " (Loaded Level)");
        _floorObjects = new GameObject[Level.MAX_FLOORS];
        for (int floor = 0; floor < Level.MAX_FLOORS; floor++) {
            _floorObjects[floor] = new GameObject("Floor " + floor.ToString());
            _floorObjects[floor].transform.SetParent(_loadedLevelObject.transform);
        }
    }

    void GetObjectCounts () {
        _objectCounts.Clear();

        for (int i = 0; i < (int)byte.MaxValue; i++)
            _objectCounts.Add ((byte)i, 0);

        for (int floor = 0; floor < Level.MAX_FLOORS; floor++) {
            for (int x = 0; x < Level.FLOOR_WIDTH; x++) {
                for (int y = 0; y < Level.FLOOR_HEIGHT; y++) {
                    for (int z = 0; z < Level.FLOOR_DEPTH; z++) {
                        var index = _loadedLevel[floor][x,y,z].index;

                        if (index == byte.MaxValue) continue;

                        _objectCounts[index]++;
                    }
                }
            }
        }
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

        for (int x = 0; x < Level.FLOOR_WIDTH; x++) {
            for (int y = 0; y < Level.FLOOR_HEIGHT; y++) {
                for (int z = 0; z < Level.FLOOR_DEPTH; z++) {
                    if (_loadedObjects[_selectedFloor, x, y, z] == null) continue;

                    DestroyLoadedObject(_loadedObjects[_selectedFloor, x, y, z]);
                }
            }
        }

        if (_loadedLevelObject != null) DestroyImmediate(_loadedLevelObject);
    }

    void DestroyLoadedObject(GameObject obj) {
        if (Application.isEditor) DestroyImmediate(obj);
        else Destroy(obj);
    }

    #endregion
}
