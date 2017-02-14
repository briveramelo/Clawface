// LevelEditorWindow.cs

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

/// <summary>
/// Main level editor GUI class.
/// </summary>
public class LevelEditorWindow : EditorWindow {

    #region Vars

    /// <summary>
    /// The current instance of this window.
    /// </summary>
    static LevelEditorWindow _Instance;


    // Object editor consts
    const float _OBJECT_EDITOR_WIDTH = 128f;
    const float _OBJECT_EDITOR_HEIGHT = 256f;

    const float _TOOLBAR_WIDTH_PERCENT = 0.6f;
    const float _TOOLBAR_HEIGHT_PERCENT = 0.1f;
    const float _TOOLBAR_UNDO_REDO_PERCENT = 0.1f;
    const float _TOOLBAR_SHOW_GRID_PERCENT = 0.25f;
    const float _TOOLBAR_SNAP_TO_GRID_PERCENT = 0.25f;

    const float _ACTION_STACK_Y_OFFSET = 128f;
    const float _ACTION_STACK_WIDTH = 192f;
    const float _ACTION_STACK_HEIGHT = 256f;

    /// <summary>
    /// Number of objects shown per row in the object browser.
    /// </summary>
    const int _OBJECTS_PER_ROW = 6;

    /// <summary>
    /// Current scroll position in the object browser.
    /// </summary>
    Vector2 _objectBrowserScrollPos = Vector2.zero;

    Vector2 _windowScrollPos = Vector2.zero;

    bool _editing = false;

    bool _levelLoaded = false;

    Rect _toolbarRect;
    Rect _sidePanelRect;
    Rect _objectEditorRect;

    Rect _undoStackRect;
    Rect _redoStackRect;

    /// <summary>
    /// Was the last mouse movement a drag?
    /// </summary>
    bool _lastMouseMoveWasDrag = false;

    #endregion
    #region Unity Callbacks

    /// <summary>
    /// Gets and shows the instance of the editor window.
    /// </summary>
    [MenuItem("Level Editor/Editor Window")]
    public static void ShowWindow() {
        _Instance = GetWindow(typeof(LevelEditorWindow)) as LevelEditorWindow;
    }

    /// <summary>
    /// Called when the script is enabled (esp. after compilation).
    /// </summary>
    void OnEnable() {
        titleContent = new GUIContent("Level Editor");
        _editing = false;
    }

    void OnDisable() {
        OnDestroy();
    }

    void OnDestroy() {
        if (LevelManager.Instance.LevelLoaded && LevelManager.Instance.Dirty) {
            if (EditorUtility.DisplayDialog("Save current level",
                "Do you wish to save the current level?",
                "Save", "Don't Save"))
                LevelManager.Instance.SaveCurrentLevel();
            LevelManager.Instance.CloseLevel();
        }

        if (_editing) DisconnectFromLevelManager();
    }

    void OnGUI() {
        if (_Instance == null)
            _Instance = GetWindow(typeof(LevelEditorWindow))
                as LevelEditorWindow;

        if (!_editing && LevelManager.Instance)
            ConnectToLevelManager();

        DrawLevelEditorGUI();
    }

    void ConnectToLevelManager() {
        // Start editing
        LevelManager.Instance.StartEditing();
        _editing = true;

        // Add event listeners
        LevelManager.Instance.onCloseLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onCreateLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onLoadLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onSaveLevel.AddListener(CheckLevelLoaded);

        // Register delegates
        SceneView.onSceneGUIDelegate += DrawGrid;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate += HandleInputs;
        SceneView.onSceneGUIDelegate += DrawCursor;
        SceneView.onSceneGUIDelegate += DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate += DrawRoomBounds;
    }

    void DrawLevelEditorGUI() {
        _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

        // Draw header
        DrawHeaderGUI();

        //        EditorGUILayout.Space();

        // Check if game is running
        if (Application.isPlaying) {
            EditorGUILayout.LabelField(@"Level editor disabled while game is 
                running!\nUse game level editor instead.",
                EditorStyles.boldLabel);
            return;
        }

        DrawCreateNewLevelButtonGUI();
        DrawCloseLevelButtonGUI();
        DrawLoadExistingLevelButtonGUI();

        EditorGUILayout.Space();

        //
        // Level options
        //
        if (_levelLoaded) {

            DrawLevelSettingsGUI();

            EditorGUILayout.Space();

            DrawSaveLevelButtonsGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawObjectBrowserGUI();
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawHeaderGUI() {
        // Title
        EditorGUILayout.LabelField("Under The Skin\nLevel Editor",
            LevelEditorStyles.Header,
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(64));
    }

    void DrawCreateNewLevelButtonGUI() {
        if (GUILayout.Button("Create new level",
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(32))) {
            if (_levelLoaded && LevelManager.Instance.Dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevel();
            }
            LevelManager.Instance.CreateNewLevel();
        }
    }

    void DrawCloseLevelButtonGUI() {
        if (_levelLoaded && GUILayout.Button("Close level",
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(32))) {
            if (LevelManager.Instance.Dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevel();
            }
            LevelManager.Instance.CloseLevel();
            Repaint();
        }
    }

    void DrawLoadExistingLevelButtonGUI() {
        if (GUILayout.Button("Load existing level",
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(32))) {
            if (_levelLoaded && LevelManager.Instance.Dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevel();
            }
            LevelManager.Instance.LoadLevel();
        }
    }

    void DrawLevelSettingsGUI() {
        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Level name
        var levelName = LevelManager.Instance.LoadedLevel.Name;
        var newName = EditorGUILayout.TextField("Name", levelName);
        if (newName != levelName) {
            LevelManager.Instance.SetLoadedLevelName(newName);
        }
    }

    void DrawSaveLevelButtonsGUI() {
        if (LevelManager.Instance.LoadedLevel.Name != default(string)) {
            if (GUILayout.Button("Save level"))
                LevelManager.Instance.SaveCurrentLevel();
            if (GUILayout.Button("Save level as asset"))
                LevelManager.Instance.SaveCurrentLevel(true);
        }
    }

    void DrawObjectBrowserGUI() {
        EditorGUILayout.LabelField("Object Browser", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        // Filter buttons
        EditorGUILayout.BeginHorizontal();
        for (int i = 1; i < (int)ObjectDatabase.Category.COUNT; i++) {
            bool filterSelected = i == (int)LevelManager.Instance.CurrentObjectFilter;
            var style = filterSelected ? LevelEditorStyles.SelectedButton : LevelEditorStyles.NormalButton;
            if (GUILayout.Button(((ObjectDatabase.Category)i).ToString(), style)) {
                LevelManager.Instance.SetObjectFilter((ObjectDatabase.Category)i);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        //------------------------------

        _objectBrowserScrollPos = EditorGUILayout.BeginScrollView(_objectBrowserScrollPos);
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

        int objectIndex = 0;
        int objectsInRow = 0;

        var filter = LevelManager.Instance.CurrentObjectFilter;
        var filteredObjects = LevelManager.Instance.FilteredObjects;


        while (objectIndex < filteredObjects.Count) {
            if (GUILayout.Button(filteredObjects[objectIndex].prefab.name, GUILayout.ExpandWidth(false))) {
                LevelManager.Instance.SelectObjectInCategory(objectIndex, filter);
                LevelManager.Instance.SelectTool(LevelManager.Tool.Place);
            }

            objectIndex++;
            objectsInRow++;

            if (objectsInRow >= _OBJECTS_PER_ROW) {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                objectsInRow = 0;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    void DisconnectFromLevelManager() {
        // Stop editing level
        LevelManager.Instance.StopEditing();
        _editing = false;

        // Deregister delegates
        SceneView.onSceneGUIDelegate -= DrawGrid;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate -= HandleInputs;
        SceneView.onSceneGUIDelegate -= DrawCursor;
        SceneView.onSceneGUIDelegate -= DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate -= DrawRoomBounds;
    }

    public static LevelEditorWindow Instance { get { return _Instance; } }

    /// <summary>
    /// Processes GUI input events.
    /// </summary>
    void HandleInputs(SceneView sc) {
        var e = Event.current;

        // No idea why this works, but this allows 
        // MouseUp events to be processed for left mouse button
        if (e.type == EventType.layout) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            return;
        }

        if (e.isKey) {
            if (e.control) {
                if (e.shift) {
                    if (e.keyCode == KeyCode.S) { // Ctrl + Shift + S
                        LevelManager.Instance.SaveCurrentLevel(true);
                    }
                } else {
                    if (e.keyCode == KeyCode.S) { // Ctrl + S
                        LevelManager.Instance.SaveCurrentLevel();
                    }
                }
            }
        } else if (e.isMouse) {
            if (MouseOverUI()) LevelManager.Instance.MouseOverUI = true;
            else LevelManager.Instance.MouseOverUI = false;

            HandleMouseMove(e);



            if (e.type == EventType.layout) {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }


            HandleInputs(e, sc.camera);
        }
    }

    public void HandleInputs(Event currEvent, Camera camera) {

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
                            HandleLeftClick(currEvent, camera);
                            break;
                        case 1: // Right click
                            HandleRightClick();
                            break;
                    }
                    break;
            }
        }
    }

    public void HandleMouseMove(Event e) {
        if (e.delta != Vector2.zero) {
            Vector3 cursorPos;
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float distance;
            if (LevelManager.Instance.EditingPlane.Raycast(ray, out distance)) {
                cursorPos = ray.GetPoint(distance);
                LevelManager.Instance.CursorPosition = cursorPos;
            }
            SceneView.RepaintAll();
        }
    }

    public void HandleLeftClick(Event e, Camera camera) {

        if (MouseOverUI()) return;
        if (_lastMouseMoveWasDrag) return;

        switch (LevelManager.Instance.CurrentTool) {
            case LevelManager.Tool.Select:
                if (LevelManager.Instance.LevelLoaded &&
                    LevelManager.Instance.CheckPlacement()) {
                    var selectRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit selectHit;
                    if (Physics.Raycast(selectRay, out selectHit)) {
                        var clickedObject = selectHit.collider.gameObject;
                        LevelManager.Instance.SelectObject(clickedObject);
                    } else LevelManager.Instance.DeselectObject();
                } else LevelManager.Instance.DeselectObject();
                break;

            case LevelManager.Tool.Place:
                if (LevelManager.Instance.HasSelectedObjectForPlacement &&
                    LevelManager.Instance.CheckPlacement() &&
                    LevelManager.Instance.CanPlaceAnotherCurrentObject()) {
                    if (LevelManager.Instance.SnapToGrid)
                        LevelManager.Instance.SnapCursor();
                    LevelManager.Instance.CreateCurrentSelectedObjectAtCursor();
                    Event.current.Use();
                }
                break;

            case LevelManager.Tool.Erase:
                var eraseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                RaycastHit eraseHit;
                if (Physics.Raycast(eraseRay, out eraseHit)) {
                    var clickedObject = eraseHit.collider.gameObject;
                    LevelManager.Instance.DeleteObject(clickedObject, ActionType.Normal);
                }
                break;

            case LevelManager.Tool.Move:
                if (!LevelManager.Instance.IsMovingObject) { // Not currently moving
                    var moveRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit moveHit;
                    if (Physics.Raycast(moveRay, out moveHit)) {
                        var clickedObject = moveHit.collider.gameObject;
                        LevelManager.Instance.StartMovingObject(clickedObject);
                    }
                } else { // Currently moving
                    LevelManager.Instance.StopMovingObject();
                }
                break;
        }
    }

    /// <summary>
    /// Handles right clicks in the scene view.
    /// </summary>
    void HandleRightClick() {
        if (MouseOverUI()) return;

        if (!_lastMouseMoveWasDrag) {

            if (LevelManager.Instance.CurrentTool == LevelManager.Tool.Move && LevelManager.Instance.IsMovingObject) {
                LevelManager.Instance.ResetMovingObject();
            }

            LevelManager.Instance.ResetTool();
        }
    }

    void DrawSceneViewGUI(SceneView sc) {

        Handles.BeginGUI();

        DrawSceneViewToolbar();

        if (LevelManager.Instance.LevelLoaded) {

            DrawSceneViewSidePanel();

            DrawUndoStack();
            DrawRedoStack();

            // Draw object editor (?)
            if (LevelManager.Instance.HasSelectedObject) {
                var selectedObject = LevelManager.Instance.SelectedObject;

                var xMin = _OBJECT_EDITOR_WIDTH / 2;
                var xMax = sc.camera.pixelWidth - _OBJECT_EDITOR_WIDTH / 2;
                var yMin = _OBJECT_EDITOR_HEIGHT / 2;
                var yMax = sc.camera.pixelHeight - _OBJECT_EDITOR_HEIGHT / 2;

                var objectEditorScreenPoint = HandleUtility.WorldToGUIPoint(selectedObject.transform.position);
                objectEditorScreenPoint.x = Mathf.Clamp(objectEditorScreenPoint.x, xMin, xMax);
                objectEditorScreenPoint.y = Mathf.Clamp(objectEditorScreenPoint.y, yMin, yMax);
                _objectEditorRect = new Rect(
                    objectEditorScreenPoint.x - xMin, 
                    objectEditorScreenPoint.y - yMin, 
                    _OBJECT_EDITOR_WIDTH, _OBJECT_EDITOR_WIDTH);

                GUILayout.Window(0, _objectEditorRect, DrawObjectEditor, selectedObject.name);
            }
        }

        Handles.EndGUI();
    }

    /// <summary>
    /// Draws the 3D cursor in the scene view.
    /// </summary>
    public void DrawCursor(SceneView sc) {
        if (!LevelManager.Instance.PreviewActive) return;

        var cursorPos = LevelManager.Instance.CursorPosition;

        switch (LevelManager.Instance.CurrentTool) {
            case LevelManager.Tool.Select:
                DrawCube(cursorPos, 1.05f, 0f, Color.white);
                if (LevelManager.Instance.HasSelectedObject)
                    DrawCube(cursorPos, 1.1f, -LevelManager.Instance.SelectedObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad, Color.cyan);
                break;

            case LevelManager.Tool.Place:
                if (LevelManager.Instance.CheckPlacement() &&
                    LevelManager.Instance.CanPlaceAnotherCurrentObject())
                    DrawCube(cursorPos, 1f, 0f, Color.green);
                else DrawCube(cursorPos, 1f, 0f, Color.gray);
                break;

            case LevelManager.Tool.Erase:
                DrawCube(cursorPos, 1.1f, 0f, Color.red);
                break;

            case LevelManager.Tool.Move:
                if (LevelManager.Instance.CheckPlacement()) DrawCube(cursorPos, 1.1f, 0f, Color.blue);
                else DrawCube(cursorPos, 1.1f, 0f, Color.gray);
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
        if (!LevelManager.Instance.GridEnabled) return;

        var height = LevelManager.Instance.SelectedFloor * Level.FLOOR_HEIGHT + LevelManager.Instance.CurrentYValue;

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

    public void DrawRoomBounds(SceneView sc) {
        var floor = LevelManager.Instance.SelectedFloor;

        var u1 = new Vector3(-0.5f, (floor + 1) * Level.FLOOR_HEIGHT, -0.5f);
        var u2 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor + 1) * Level.FLOOR_HEIGHT, -0.5f);
        var u3 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);
        var u4 = new Vector3(-0.5f, (floor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);

        var d1 = new Vector3(-0.5f, (floor) * Level.FLOOR_HEIGHT, -0.5f);
        var d2 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor) * Level.FLOOR_HEIGHT, -0.5f);
        var d3 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);
        var d4 = new Vector3(-0.5f, (floor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);

        Handles.color = Color.white;
        Handles.DrawLine(u1, u2);
        Handles.DrawLine(u2, u3);
        Handles.DrawLine(u3, u4);
        Handles.DrawLine(u4, u1);

        Handles.DrawLine(u1, d1);
        Handles.DrawLine(u2, d2);
        Handles.DrawLine(u3, d3);
        Handles.DrawLine(u4, d4);

        Handles.DrawLine(d1, d2);
        Handles.DrawLine(d2, d3);
        Handles.DrawLine(d3, d4);
        Handles.DrawLine(d4, d1);
    }

    void DrawSceneViewToolbar() {
        var toolbarWidth = _TOOLBAR_WIDTH_PERCENT * Screen.width;
        _toolbarRect = new Rect(4, 4, toolbarWidth, _TOOLBAR_HEIGHT_PERCENT * Screen.height);
        GUILayout.BeginArea(_toolbarRect);

        GUILayout.BeginHorizontal();

        var levelLoaded = LevelManager.Instance.LevelLoaded;
        if (!levelLoaded)
            GUILayout.Label("No level loaded.");
        else {
            GUILayout.Label(LevelManager.Instance.LoadedLevel.Name + (LevelManager.Instance.Dirty ? "*" : ""));

            var undoRedoWidth = toolbarWidth * _TOOLBAR_UNDO_REDO_PERCENT;

            if (GUILayout.Button("Undo", GUILayout.Width(undoRedoWidth))) {
                LevelManager.Instance.Undo();
            }

            if (GUILayout.Button("Redo", GUILayout.Width(undoRedoWidth))) {
                LevelManager.Instance.Redo();
            }

            var drawGridToggleWidth = toolbarWidth * _TOOLBAR_SHOW_GRID_PERCENT;

            var drawGrid = LevelManager.Instance.GridEnabled;
            //var newDrawGrid = GUILayout.Toggle(drawGrid, "Draw Grid", GUILayout.Width(drawGridToggleWidth));
            var newDrawGrid = GUILayout.Toggle(drawGrid, "Draw Grid");
            if (drawGrid != newDrawGrid) {
                if (newDrawGrid) LevelManager.Instance.EnableGrid();
                else LevelManager.Instance.DisableGrid();
            }

            var snapToGridToggleWidth = toolbarWidth * _TOOLBAR_SNAP_TO_GRID_PERCENT;

            var snapToGrid = LevelManager.Instance.SnapToGrid;
            //var newSnapToGrid = GUILayout.Toggle(snapToGrid, "Snap To Grid", GUILayout.Width(snapToGridToggleWidth));
            var newSnapToGrid = GUILayout.Toggle(snapToGrid, "Snap To Grid");
            if (snapToGrid != newSnapToGrid) {
                if (newSnapToGrid) LevelManager.Instance.SnapToGrid = true;
                else LevelManager.Instance.SnapToGrid = false;
            }

            if (GUILayout.Button("Move"))
                LevelManager.Instance.SelectTool(LevelManager.Tool.Move);

            if (GUILayout.Button("Erase"))
                LevelManager.Instance.SelectTool(LevelManager.Tool.Erase);
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    void DrawUndoStack() {
        _undoStackRect = new Rect(
            Screen.width - _ACTION_STACK_WIDTH,
            _ACTION_STACK_Y_OFFSET,
            _ACTION_STACK_WIDTH,
            _ACTION_STACK_HEIGHT);
        GUILayout.BeginArea(_undoStackRect);

        GUILayout.Label("Undo Stack");

        foreach (var undo in LevelManager.Instance.UndoStack) {
            GUILayout.Label(undo.ToShortString());
        }

        GUILayout.EndArea();
    }

    void DrawSceneViewSidePanel() {
        // Draw side panel
        _sidePanelRect = new Rect(4, 40, 32, 256);
        GUILayout.BeginArea(_sidePanelRect);

        GUILayout.BeginVertical();

        // Floor selector
        GUILayout.BeginVertical();

        GUILayout.Label("Floor", GUILayout.ExpandWidth(true));

        if (GUILayout.Button("^")) {
            LevelManager.Instance.UpOneFloor();
        }

        GUILayout.Label(LevelManager.Instance.SelectedFloor.ToString(), GUILayout.ExpandWidth(true));

        if (GUILayout.Button("v")) {
            LevelManager.Instance.DownOneFloor();
        }

        GUILayout.EndVertical();

        GUILayout.Space(32);


        // Y selector
        GUILayout.BeginVertical();

        GUILayout.Label("Y", GUILayout.ExpandWidth(true));

        if (GUILayout.Button("^")) {
            LevelManager.Instance.IncrementY();
        }

        GUILayout.Label(LevelManager.Instance.CurrentYValue.ToString(), GUILayout.ExpandWidth(true));

        if (GUILayout.Button("v")) {
            LevelManager.Instance.DecrementY();
        }

        GUILayout.EndVertical();

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    void DrawRedoStack() {
        _redoStackRect = new Rect(
            Screen.width - _ACTION_STACK_WIDTH,
            _ACTION_STACK_HEIGHT + _ACTION_STACK_Y_OFFSET,
            _ACTION_STACK_WIDTH,
            _ACTION_STACK_HEIGHT);
        GUILayout.BeginArea(_redoStackRect);

        GUILayout.Label("Redo Stack");

        foreach (var redo in LevelManager.Instance.RedoStack) {
            GUILayout.Label(redo.ToShortString());
        }

        GUILayout.EndArea();
    }


    void DrawObjectEditor(int windowID) {

        var selectedObject = LevelManager.Instance.SelectedObject;

        GUILayout.Label("Rotation");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("<-")) {
            LevelManager.Instance.RotateSelectedObject(-5);
        }

        GUILayout.Label(Mathf.RoundToInt(selectedObject.transform.rotation.eulerAngles.y).ToString() + "°", EditorStyles.centeredGreyMiniLabel);

        if (GUILayout.Button("->")) {
            LevelManager.Instance.RotateSelectedObject(5);
        }

        GUILayout.EndHorizontal();
    }

    bool MouseOverUI() {
        var mousePos = Event.current.mousePosition;
        return _toolbarRect.Contains(mousePos) || _sidePanelRect.Contains(mousePos) || _objectEditorRect.Contains(mousePos);
    }

    void CheckLevelLoaded() {
        _levelLoaded = LevelManager.Instance.LevelLoaded;
    }

    #endregion
}
