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

    const float _TOOLBAR_WIDTH_PERCENT = 0.4f;
    const float _TOOLBAR_HEIGHT_PERCENT = 0.1f;
    const float _TOOLBAR_UNDO_REDO_PERCENT = 0.25f;
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
        SceneView.onSceneGUIDelegate += LevelManager.Instance.DrawGrid;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate += HandleInputs;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.DrawCursor;
        SceneView.onSceneGUIDelegate += DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.DrawRoomBounds;
    }

    void DrawLevelEditorGUI() {
        _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

        // Draw header
        DrawHeaderGUI();

        EditorGUILayout.Space();

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
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawGrid;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate -= HandleInputs;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawCursor;
        SceneView.onSceneGUIDelegate -= DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawRoomBounds;
    }

    public static LevelEditorWindow Instance { get { return _Instance; } }

    /// <summary>
    /// Processes GUI input events.
    /// </summary>
    void HandleInputs(SceneView sc) {
        var e = Event.current;

        // No idea why this works, but this allows 
        // MouseUp events to be processed for left mouse button
        if (e.type == EventType.layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

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
            LevelManager.Instance.HandleInputs(Event.current, sc.camera);
        }
    }

    void DrawSceneViewGUI(SceneView sc) {

        Handles.BeginGUI();

        // Draw toolbar
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
            var newDrawGrid = GUILayout.Toggle(drawGrid, "Draw Grid", GUILayout.Width(drawGridToggleWidth));
            if (drawGrid != newDrawGrid) {
                if (newDrawGrid) LevelManager.Instance.EnableGrid();
                else LevelManager.Instance.DisableGrid();
            }

            var snapToGridToggleWidth = toolbarWidth * _TOOLBAR_SNAP_TO_GRID_PERCENT;

            var snapToGrid = LevelManager.Instance.SnapToGrid;
            var newSnapToGrid = GUILayout.Toggle(snapToGrid, "Snap To Grid", GUILayout.Width(snapToGridToggleWidth));
            if (snapToGrid != newSnapToGrid) {
                if (newSnapToGrid) LevelManager.Instance.SnapToGrid = true;
                else LevelManager.Instance.SnapToGrid = false;
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        if (levelLoaded) {

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

            DrawUndoStack();
            DrawRedoStack();

            // Draw object editor (?)
            if (LevelManager.Instance.HasSelectedObject) {
                var selectedObject = LevelManager.Instance.SelectedObject;
                var objectEditorScreenPoint = sc.camera.WorldToScreenPoint(selectedObject.transform.position);
                objectEditorScreenPoint.x = Mathf.Clamp(objectEditorScreenPoint.x, _OBJECT_EDITOR_WIDTH, sc.camera.pixelWidth - _OBJECT_EDITOR_WIDTH);
                objectEditorScreenPoint.y = Mathf.Clamp(Screen.height * 0.925f - objectEditorScreenPoint.y, _OBJECT_EDITOR_HEIGHT, sc.camera.pixelHeight - _OBJECT_EDITOR_HEIGHT);
                _objectEditorRect = new Rect(objectEditorScreenPoint.x, objectEditorScreenPoint.y, _OBJECT_EDITOR_WIDTH, _OBJECT_EDITOR_WIDTH);

                GUILayout.Window(0, _objectEditorRect, DrawObjectEditor, selectedObject.name);
            }
        }

        Handles.EndGUI();
    }

    void DrawUndoStack () {
        _undoStackRect = new Rect (
            Screen.width - _ACTION_STACK_WIDTH, 
            _ACTION_STACK_Y_OFFSET, 
            _ACTION_STACK_WIDTH, 
            _ACTION_STACK_HEIGHT);
        GUILayout.BeginArea (_undoStackRect);

        GUILayout.Label ("Undo Stack");

        foreach (var undo in LevelManager.Instance.UndoStack) {
            GUILayout.Label (undo.ToShortString());
        }

        GUILayout.EndArea();
    }

    void DrawRedoStack () {
        _redoStackRect = new Rect (
            Screen.width - _ACTION_STACK_WIDTH, 
            _ACTION_STACK_HEIGHT + _ACTION_STACK_Y_OFFSET, 
            _ACTION_STACK_WIDTH, 
            _ACTION_STACK_HEIGHT);
        GUILayout.BeginArea (_redoStackRect);

        GUILayout.Label ("Redo Stack");

        foreach (var redo in LevelManager.Instance.RedoStack) {
            GUILayout.Label (redo.ToShortString());
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

    bool MouseOverUI () {
        var mousePos = Event.current.mousePosition;
        return _toolbarRect.Contains (mousePos) || _sidePanelRect.Contains (mousePos) || _objectEditorRect.Contains(mousePos);
    }

    void CheckLevelLoaded() {
        _levelLoaded = LevelManager.Instance.LevelLoaded;
    }

    #endregion
}
