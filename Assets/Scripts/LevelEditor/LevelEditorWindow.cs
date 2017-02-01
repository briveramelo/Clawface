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

    const float _OBJECT_EDITOR_WIDTH = 128f;
    const float _OBJECT_EDITOR_HEIGHT = 256f;

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

    #endregion
    #region Unity Callbacks

    /// <summary>
    /// Gets and shows the instance of the editor window.
    /// </summary>
    [MenuItem("Level Editor/Editor Window")]
    public static void ShowWindow() {
        _Instance = EditorWindow.GetWindow(typeof(LevelEditorWindow)) as LevelEditorWindow;
    }

    void OnEnable() {
        titleContent = new GUIContent("Level Editor");
        _editing = false;
    }

    void OnDisable() {
        LevelManager.Instance.StopEditing();
        _editing = false;
        OnDestroy();
    }

    void OnFocus() {
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawGrid;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate -= HandleInputs;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawCursor;
        SceneView.onSceneGUIDelegate -= ShowSceneViewGUI;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawRoomBounds;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.DrawGrid;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate += HandleInputs;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.DrawCursor;
        SceneView.onSceneGUIDelegate += ShowSceneViewGUI;
        SceneView.onSceneGUIDelegate += LevelManager.Instance.DrawRoomBounds;
    }

    void OnDestroy() {
        if (LevelManager.Instance.LevelLoaded && LevelManager.Instance.Dirty) {
            if (EditorUtility.DisplayDialog("Save current level",
                "Do you wish to save the current level?", "Save", "Don't Save"))
                LevelManager.Instance.SaveCurrentLevel();
            LevelManager.Instance.CloseLevel();
        }

        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawGrid;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.SnapSelected;
        SceneView.onSceneGUIDelegate -= HandleInputs;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawCursor;
        SceneView.onSceneGUIDelegate -= ShowSceneViewGUI;
        SceneView.onSceneGUIDelegate -= LevelManager.Instance.DrawRoomBounds;
    }

    void OnGUI() {
        if (_Instance == null)
            _Instance = EditorWindow.GetWindow(typeof(LevelEditorWindow)) as LevelEditorWindow;

        if (!_editing) {
            if (LevelManager.Instance) {
                LevelManager.Instance.StartEditing();
                _editing = true;
                LevelManager.Instance.onCloseLevel.AddListener(CheckLevelLoaded);
                LevelManager.Instance.onCreateLevel.AddListener(CheckLevelLoaded);
                LevelManager.Instance.onLoadLevel.AddListener(CheckLevelLoaded);
                LevelManager.Instance.onSaveLevel.AddListener(CheckLevelLoaded);
            }
        }

        bool dirty = LevelManager.Instance.Dirty;

        _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

        //
        // Header
        //
        EditorGUILayout.LabelField("Under The Skin\nLevel Editor",
            LevelEditorStyles.Header,
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(64));

        EditorGUILayout.Space();

        // Check if game is running
        if (Application.isPlaying) {
            EditorGUILayout.LabelField("Level editor disabled while game is running!\nUse game level editor instead.",
                EditorStyles.boldLabel);
            return;
        }

        //
        // Create new level button
        //
        if (GUILayout.Button("Create new level",
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(32))) {
            if (_levelLoaded && dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevel();
            }
            LevelManager.Instance.CreateNewLevel();
        }

        //
        // Close level button
        //
        if (_levelLoaded && GUILayout.Button("Close level",
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(32))) {
            if (dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevel();
            }
            LevelManager.Instance.CloseLevel();
            Repaint();
        }

        //
        // Load existing level button
        //
        if (GUILayout.Button("Load existing level",
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(32))) {
            if (_levelLoaded && dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevel();
            }
            LevelManager.Instance.LoadLevel();
        }

        //
        // Level options
        //
        if (_levelLoaded) {

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            // Level name
            var levelName = LevelManager.Instance.LoadedLevelName;
            var newName = EditorGUILayout.TextField("Name", levelName);
            if (newName != levelName) {
                LevelManager.Instance.SetLoadedLevelName(newName);
            }

            EditorGUILayout.Space();

            //
            // Save level as asset button
            //
            if (newName != default(string)) {
                if (GUILayout.Button("Save level"))
                    LevelManager.Instance.SaveCurrentLevel();
                if (GUILayout.Button("Save level as asset"))
                    LevelManager.Instance.SaveCurrentLevel(true);

            }

            try {
                if (EditorGUI.EndChangeCheck())
                    LevelManager.Instance.SetDirty(true);
            } catch (System.InvalidOperationException) { }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            //
            // Toolbar
            //

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Erase Tool")) {
                LevelManager.Instance.SelectTool(LevelManager.Tool.Erase);
            }
            if (GUILayout.Button("Move Tool")) {
                LevelManager.Instance.SelectTool(LevelManager.Tool.Move);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            //
            // Show grid toggle
            //
            if (EditorGUILayout.Toggle("Show Grid", LevelManager.Instance.GridEnabled)) {
                LevelManager.Instance.EnableGrid();
            } else LevelManager.Instance.DisableGrid();

            EditorGUILayout.Space();

            //
            // Object browser
            //
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
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight (true));

            int objectIndex = 0;
            int objectsInRow = 0;

            var filter = LevelManager.Instance.CurrentObjectFilter;
            var filteredObjects = LevelManager.Instance.FilteredObjects;


            while (objectIndex < filteredObjects.Count) {
                if (GUILayout.Button(filteredObjects[objectIndex].prefab.name, GUILayout.ExpandWidth(false))) {
                    Debug.Log (objectIndex + " " + filter);
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

        EditorGUILayout.EndScrollView();
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
            LevelManager.Instance.HandleInputs(Event.current, sc.camera);
        }
    }

    void ShowSceneViewGUI(SceneView sc) {

        Handles.BeginGUI();

        // Draw toolbar
        var toolbarRect = new Rect(4, 4, 256, 32);
        GUILayout.BeginArea(toolbarRect);

        GUILayout.BeginHorizontal();

        var levelLoaded = LevelManager.Instance.LevelLoaded;
        if (!levelLoaded)
            GUILayout.Label("No level loaded.");
        else {
            GUILayout.Label(LevelManager.Instance.LoadedLevelName + (LevelManager.Instance.Dirty ? "*" : ""));

            if (GUILayout.Button("Undo")) {
                LevelManager.Instance.Undo();
            }

            if (GUILayout.Button("Redo")) {
                LevelManager.Instance.Redo();
            }

            var drawGrid = LevelManager.Instance.GridEnabled;
            var newDrawGrid = GUILayout.Toggle(drawGrid, "Draw Grid");
            if (drawGrid != newDrawGrid) {
                if (newDrawGrid) LevelManager.Instance.EnableGrid();
                else LevelManager.Instance.DisableGrid();
            }

        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        if (levelLoaded) {

            // Draw side panel
            var sidePanelRect = new Rect(4, 40, 32, 256);
            GUILayout.BeginArea(sidePanelRect);

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

            // Draw object editor (?)
            if (LevelManager.Instance.HasSelectedObject) {
                var selectedObject = LevelManager.Instance.SelectedObject;
                var objectEditorScreenPoint = sc.camera.WorldToScreenPoint(selectedObject.transform.position);
                objectEditorScreenPoint.x = Mathf.Clamp(objectEditorScreenPoint.x, _OBJECT_EDITOR_WIDTH, sc.camera.pixelWidth - _OBJECT_EDITOR_WIDTH);
                objectEditorScreenPoint.y = Mathf.Clamp(Screen.height * 0.925f - objectEditorScreenPoint.y, _OBJECT_EDITOR_HEIGHT, sc.camera.pixelHeight - _OBJECT_EDITOR_HEIGHT);
                var objectEditorRect = new Rect(objectEditorScreenPoint.x, objectEditorScreenPoint.y, _OBJECT_EDITOR_WIDTH, _OBJECT_EDITOR_WIDTH);

                GUILayout.Window(0, objectEditorRect, DrawObjectEditor, selectedObject.name);
            }
        }

        Handles.EndGUI();
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

    void CheckLevelLoaded() {
        _levelLoaded = LevelManager.Instance.LevelLoaded;
    }

    #endregion
}
