// LevelEditorWindow.cs

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Main level editor GUI class.
/// </summary>
public class LevelEditorWindow : EditorWindow, ILevelEditor {

    #region Constants

    // GUI style constants
    const string _SCENEVIEW_SKIN_PATH = "LevelEditorGUISkin.guiskin";

    // Object browser constants
    const int _OBJECTS_PER_ROW = 6;

    // Object editor constants
    const float _OBJECT_EDITOR_WIDTH = 128f;
    const float _OBJECT_EDITOR_HEIGHT = 256f;
    const float _OBJECT_EDITOR_OFFSET_X = 8f;
    const float _OBJECT_EDITOR_OFFSET_Y = 8f;

    // Toolbar constants
    const float _TOOLBAR_WIDTH_PERCENT = 0.6f;
    const float _TOOLBAR_HEIGHT_PERCENT = 0.1f;
    const float _TOOLBAR_HEIGHT = 48f;
    const float _TOOLBAR_UNDO_REDO_PERCENT = 0.15f;
    const float _TOOLBAR_SHOW_GRID_PERCENT = 0.2f;
    const float _TOOLBAR_SNAP_TO_GRID_PERCENT = 0.2f;

    // Side panel constants
    const float _SIDEPANEL_WIDTH = 64f;
    const float _SIDEPANEL_HEIGHT = 256f;

    // Action stack constants
    const float _ACTION_STACK_Y_OFFSET = 128f;
    const float _ACTION_STACK_WIDTH = 192f;
    const float _ACTION_STACK_HEIGHT = 256f;

    // General constants
    const float _DRAG_DIST = 8f;

    // Color 'constants'
    Color _DRAGBOX_FILL_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.25f);
    Color _DRAGBOX_STROKE_COLOR = new Color(0.6f, 0.6f, 0.6f, 0.5f);
   
    #endregion
    #region Vars

    /// <summary>
    /// The current instance of this window.
    /// </summary>
    static LevelEditorWindow _Instance;

    /// <summary>
    /// Current scroll position in the object browser.
    /// </summary>
    Vector2 _objectBrowserScrollPos = Vector2.zero;

    /// <summary>
    /// Scroll position of the editor window.
    /// </summary>
    Vector2 _windowScrollPos = Vector2.zero;

    /// <summary>
    /// Is a level currently loaded?
    /// </summary>
    bool _levelLoaded = false;

    /// <summary>
    /// Rect positions for the scene view controls.
    /// </summary>
    Rect _toolbarRect, _sidePanelRect;

    /// <summary>
    /// Rect for the object editor.
    /// </summary>
    Rect _objectEditorRect;

    /// <summary>
    /// Rect positions for the undo/redo stack displays.
    /// </summary>
    Rect _undoStackRect, _redoStackRect;

    /// <summary>
    /// Was the last mouse movement a drag?
    /// </summary>
    bool _lastMouseMoveWasDrag = false;

    /// <summary>
    /// Allow non-uniform scale on objects?
    /// </summary>
    bool _allowNonUniformScale = false;

    /// <summary>
    /// Object rotation before edit.
    /// </summary>
    Vector3 _originalRotation;

    /// <summary>
    /// Object scale before edit.
    /// </summary>
    Vector3 _originalScale;

    /// <summary>
    /// Connected LM.
    /// </summary>
    LevelManager _levelManager;


    bool _leftMouseDown = false;
    Vector2 _mouseDownScreenPos;
    bool _isDragging = false;
    Rect _selectionRect;

    SceneView _sceneView;
    Ray _pointerRay;

    bool _settingsWindowExpanded = false;

    GUISkin _sceneViewGUISkin;

    #endregion
    #region Unity Callbacks

    /// <summary>
    /// Gets and shows the instance of the editor window.
    /// </summary>
    [MenuItem("Level Editor/Editor Window")]
    public static void ShowWindow() {
        _Instance = GetWindow(typeof(LevelEditorWindow)) as LevelEditorWindow;
        if (LevelManager.Instance == null) {
            GameObject lm = new GameObject ("LevelManager", typeof (LevelManager));
        }
        if (ObjectDatabaseManager.Instance == null) {
            GameObject objdb = new GameObject ("ObjectDatabaseManager", typeof(ObjectDatabaseManager));
        }
    }

    /// <summary>
    /// Called when the script is enabled (esp. after compilation).
    /// </summary>
    void OnEnable() {
        titleContent = new GUIContent("Level Editor");
        if (Application.isPlaying) OnEnablePlayer();
        else OnEnableEditor();
    }

    /// <summary>
    /// Called when the script is disabled (i.e. after compilation).
    /// </summary>
    void OnDisable() {
        //Debug.Log("LevelEditorWindow.OnDisable");

        // Deregister delegates
        SceneView.onSceneGUIDelegate -= DrawGrid;
        SceneView.onSceneGUIDelegate -= HandleInputs;
        SceneView.onSceneGUIDelegate -= DrawCursor;
        SceneView.onSceneGUIDelegate -= DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate -= DrawRoomBounds;
    }

    /// <summary>
    /// Called when the editor window is destroyed (closed, compiled).
    /// </summary>
    void OnDestroy() {
        //Debug.Log("LevelEditorWindow.OnDestroy");
        if (_levelManager) {
            if (LevelManager.Instance.LevelLoaded) {
                if (LevelManager.Instance.Dirty) {
                    if (EditorUtility.DisplayDialog("Save current level",
                        "Do you wish to save the current level?",
                        "Save", "Don't Save"))
                        LevelManager.Instance.SaveCurrentLevelToJSON();
                }
                CloseLevelAndKeepPath();
            }

            DisconnectFromLevelManager();
        }
    }

    void OnGUI() {
        if (_Instance == null)
            _Instance = GetWindow(typeof(LevelEditorWindow))
                as LevelEditorWindow;

        // If not connected to LM, attempt to
        if (!Application.isPlaying &&
            (!_levelManager || !LevelManager.Instance.IsConnectedToEditor) &&
            LevelManager.Instance != null)
            ConnectToLevelManager();

        // Draw window GUI
        DrawLevelEditorGUI();
    }

    #endregion
    #region ILevelEditor Implementation

    /// <summary>
    /// Returns the active scene view camera (read-only).
    /// </summary>
    public Camera ActiveCamera {
        get {
            if (_sceneView == null) {
                Debug.LogError("No scene view set!");
                return null;
            }
            return _sceneView.camera;
        }
    }

    /// <summary>
    /// Returns the current selection rectangle (read-only).
    /// </summary>
    public Rect SelectionRect { get { return _selectionRect; } }

    /// <summary>
    /// Returns the ray currently cast by the pointer (read-only).
    /// </summary>
    public Ray PointerRay { get { return _pointerRay; } }

    #endregion
    #region Methods

    void OnEnableEditor() {
        //Debug.Log("LevelEditorWindow.OnEnableEditor");
        if (LevelManager.Instance == null)
            LevelManager.OnSingletonInitializedEditor.AddListener(ConnectToLevelManager);
        else ConnectToLevelManager();

        if (_sceneViewGUISkin == null)
            _sceneViewGUISkin = EditorGUIUtility.Load (_SCENEVIEW_SKIN_PATH) as GUISkin;
        if (_sceneViewGUISkin == null) Debug.LogError ("Failed to load SceneView GUISkin!");
    }

    void OnEnablePlayer() {
        //Debug.Log("LevelEditorWindow.OnEnablePlayer");
    }

    /// <summary>
    /// Connects the editor window the LevelManager.
    /// </summary>
    void ConnectToLevelManager() {

        // Start editing
        LevelManager.Instance.StartEditing();
        LevelManager.Instance.SetEditor(this);
        _levelManager = LevelManager.Instance;

        // Add event listeners
        LevelManager.Instance.onCloseLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onCreateLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onLoadLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onSaveLevel.AddListener(CheckLevelLoaded);

        // Register scene viewdelegates
        SceneView.onSceneGUIDelegate += DrawGrid;
        SceneView.onSceneGUIDelegate += HandleInputs;
        SceneView.onSceneGUIDelegate += DrawCursor;
        SceneView.onSceneGUIDelegate += DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate += DrawRoomBounds;
    }

    /// <summary>
    /// Draws the level editor window.
    /// </summary>
    void DrawLevelEditorGUI() {
        _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

        // Draw header
        DrawHeaderGUI();

        // Check if game is running
        if (Application.isPlaying) {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(@"Level editor disabled while game is 
                running!\nUse game level editor instead.",
                EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            return;
        }

        // If not connected, there is probably no LevelManager or
        // ObjectDatabaseManager in the scene
        if (!_levelManager) {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("No LevelManager or ObjectDatabaseManager!",
                GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
        } else {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.ExpandWidth(true));

            // Draw create/close/load level buttons
            DrawEditorButton("Create new level", CreateNewLevel);
            DrawEditorButton("Load existing level", LoadExistingLevel);

            EditorGUILayout.EndHorizontal();

            // Draw next controls if a level is loaded
            EditorGUI.BeginDisabledGroup(!_levelLoaded);

            // Draw level settings
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawLevelSettingsGUI();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            DrawEditorButton("Close level", CloseLevel);

            // Draw save level button
            DrawEditorButton("Save current level", SaveCurrentLevel);
            DrawEditorButton("Save as new level", SaveAsNewLevel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Draw object browser
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawObjectBrowserGUI();
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the editor header.
    /// </summary>
    void DrawHeaderGUI() {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Project Turing Level Editor",
            EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Creates a new level.
    /// </summary>
    void CreateNewLevel() {
        if (_levelLoaded && LevelManager.Instance.Dirty) {
            if (EditorUtility.DisplayDialog("Save current level",
                "Do you wish to save the current level?", "Save", "Don't Save"))
                LevelManager.Instance.SaveCurrentLevelToJSON();
        }
        LevelManager.Instance.CreateNewLevel();
    }

    /// <summary>
    /// Closes the current level, saving its path.
    /// </summary>
    void CloseLevelAndKeepPath() {
        var path = LevelManager.Instance.LoadedLevelPath;
        CloseLevel();
        EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, path);
    }

    /// <summary>
    /// Closes the current level and discards its path.
    /// </summary>
    void CloseLevel() {
        // If dirty, prompt to save
        if (LevelManager.Instance.Dirty) {
            var option = EditorUtility.DisplayDialogComplex(
                "Close current level",
                "Do you wish to save the current level?",
                "Save",
                "Don't Save",
                "Cancel");

            switch (option) {
                case 0: // Save level
                    LevelManager.Instance.SaveCurrentLevelToJSON();
                    LevelManager.Instance.CloseLevel();
                    EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, "");
                    break;
                case 1: // Don't save level
                    LevelManager.Instance.CloseLevel();
                    EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, "");
                    break;
                case 2: // Cancel
                    break;
            }
        } else {
            LevelManager.Instance.CloseLevel();
            EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, "");
        }

        Repaint();
    }

    /// <summary>
    /// Loads an existing level.
    /// </summary>
    void LoadExistingLevel() {
        if (_levelLoaded && LevelManager.Instance.Dirty) {
            if (EditorUtility.DisplayDialog("Save current level",
                "Do you wish to save the current level?", "Save", "Don't Save"))
                LevelManager.Instance.SaveCurrentLevelToJSON();
        }
        LevelManager.Instance.LoadLevelFromJSON();
    }

    /// <summary>
    /// Draws the level settings fields.
    /// </summary>
    void DrawLevelSettingsGUI() {
        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Level name
        string levelName = "";
        if (LevelManager.Instance != null && LevelManager.Instance.LoadedLevel != null)
            levelName = LevelManager.Instance.LoadedLevel.Name;
        var newName = EditorGUILayout.TextField("Name", levelName);
        if (LevelManager.Instance != null && newName != levelName)
            LevelManager.Instance.SetLoadedLevelName(newName);
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Saves the current level.
    /// </summary>
    void SaveCurrentLevel() {
        LevelManager.Instance.SaveCurrentLevelToJSON();
    }

    /// <summary>
    /// Saves the current level, forcing the file browser to open.
    /// </summary>
    void SaveAsNewLevel() {
        LevelManager.Instance.SaveCurrentLevelToJSON(true);
    }

    /// <summary>
    /// Draws the object browser.
    /// </summary>
    void DrawObjectBrowserGUI() {
        // Draw label
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Object Browser", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        // Draw filter buttons
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filters", GUILayout.Width(48f));
        for (int i = 1; i < (int)ObjectDatabase.Category.COUNT; i++) {
            bool filterSelected = i == (int)LevelManager.Instance.CurrentObjectFilter;
            var style = filterSelected ? EditorStyles.toolbarButton : EditorStyles.helpBox;
            if (GUILayout.Button(((ObjectDatabase.Category)i).ToString(), style))
                LevelManager.Instance.SetObjectFilter((ObjectDatabase.Category)i);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        int objectIndex = 0;
        int objectsInRow = 0;

        var filter = LevelManager.Instance.CurrentObjectFilter;
        var filteredObjects = LevelManager.Instance.FilteredObjects;
        if (filteredObjects != null) {

            // Draw object buttons
            _objectBrowserScrollPos = EditorGUILayout.BeginScrollView(_objectBrowserScrollPos);
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

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

        EditorGUILayout.EndVertical();
    }

    void DrawLevelEditorSettingsGUI () {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _settingsWindowExpanded = EditorGUILayout.Foldout (_settingsWindowExpanded, "Settings", true);
        if (_settingsWindowExpanded) {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Disconnects from the LevelManager.
    /// </summary>
    void DisconnectFromLevelManager() {
        // Stop editing level
        LevelManager.Instance.StopEditing();
        if (LevelManager.Instance.LevelLoaded)
            LevelManager.Instance.CloseLevel();
        _levelManager = null;
    }

    /// <summary>
    /// Returns the current instance of the editor window.
    /// </summary>
    public static LevelEditorWindow Instance { get { return _Instance; } }

    /// <summary>
    /// Processes GUI input events.
    /// </summary>
    void HandleInputs(SceneView sc) {
        if (Application.isPlaying) return;

        var e = Event.current;

        // No idea why this works, but this allows 
        // MouseUp events to be processed for left mouse button.
        // It also disables default handles.
        if (e.rawType == EventType.layout) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            return;
        }

        // Key events
        if (e.isKey) {
            if (e.control) {
                if (e.shift) {
                    if (e.keyCode == KeyCode.S) { // Ctrl + Shift + S
                        if (LevelManager.Instance.LevelLoaded)
                            LevelManager.Instance.SaveCurrentLevelToJSON(true);
                    }
                } else {
                    switch (e.keyCode) {
                        case KeyCode.R:
                            if (LevelManager.Instance.RedoStack.Count > 0)
                                LevelManager.Instance.Redo();
                            else {
                                try {
                                    Undo.PerformRedo();
                                } catch (System.NullReferenceException) { }
                            }
                            e.Use();
                            break;

                        case KeyCode.S: // Ctrl + S
                            if (LevelManager.Instance.LevelLoaded)
                                LevelManager.Instance.SaveCurrentLevelToJSON();
                            break;

                        case KeyCode.Z: // Ctrl + Z
                            if (LevelManager.Instance.UndoStack.Count > 0)
                                LevelManager.Instance.Undo();
                            else {
                                try {
                                    Undo.PerformUndo();
                                } catch (System.NullReferenceException) { }
                            }
                            e.Use();
                            break;
                    }
                }
            }

            // Mouse events
        } else if (e.isMouse) {
            if (LevelManager.Instance != null) {

                if (MouseOverUI()) LevelManager.Instance.MouseOverUI = true;
                else LevelManager.Instance.MouseOverUI = false;

                HandleMouseMove(sc, e);

                if (e.rawType == EventType.layout)
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

                HandleInputs(e, sc.camera);
            }
        }
    }

    /// <summary>
    /// Handles inputs.
    /// </summary>
    public void HandleInputs(Event currEvent, Camera camera) {


        // Mouse events
        if (currEvent.isMouse) {
            switch (currEvent.rawType) {
                case EventType.MouseDrag:
                    _lastMouseMoveWasDrag = true;
                    break;

                case EventType.MouseDown:
                    _lastMouseMoveWasDrag = false;
                    switch (currEvent.button) {
                        case 0: // Left mouse button down
                            HandleLeftDown(currEvent);
                            break;
                    }
                    break;

                case EventType.MouseUp:
                    switch (currEvent.button) {
                        case 0: // Left click
                            HandleLeftUp(currEvent, camera);
                            break;
                        case 1: // Right click
                            HandleRightUp();
                            break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Handles mouse movement.
    /// </summary>
    public void HandleMouseMove(SceneView sc, Event e) {
        // If mouse is moved
        if (e.delta != Vector2.zero) {

            if (_leftMouseDown)
                _isDragging = Vector2.Distance(_mouseDownScreenPos, e.mousePosition) >= _DRAG_DIST;

            _pointerRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            LevelManager.Instance.HandleMouseMove(e, _isDragging);


            // Force a repaint to show changes
            SceneView.RepaintAll();
        }
    }

    void HandleLeftDown(Event e) {
        _leftMouseDown = true;
        _mouseDownScreenPos = e.mousePosition;
        LevelManager.Instance.HandleLeftDown(e);
    }

    public void HandleLeftDownOnButton() {
        //Debug.Log ("down");
        /*var selectedObjects = LevelManager.Instance.SelectedObjects;
        if (selectedObjects != null) {
            _originalRotation = selectedObject.transform.localRotation.eulerAngles;
            _originalScale = selectedObject.transform.localScale;
        }*/
    }

    public void HandleLeftUpOnButton() {
        //Debug.Log("up");

        // Record attribute change if necessary
        /*var selectedObjects = LevelManager.Instance.SelectedObject;
        if (selectedObject != null) {
            var newRot = selectedObject.transform.localRotation.eulerAngles;
            if (_originalRotation != newRot) {
                LevelManager.Instance.RecordAttributeChange(selectedObject, ChangeObjectNormalAttributeAction.AttributeChanged.Rotation, _originalRotation, newRot, ActionType.Normal);
            } else {
                var newScale = selectedObject.transform.localScale;
                if (_originalScale != newScale) {
                    LevelManager.Instance.RecordAttributeChange(selectedObject, ChangeObjectNormalAttributeAction.AttributeChanged.Scale, _originalScale, newScale, ActionType.Normal);
                }
            }
        }*/
    }

    /// <summary>
    /// Handles left click events.
    /// </summary>
    public void HandleLeftUp(Event e, Camera camera) {


        _leftMouseDown = false;

        if (MouseOverUI()) return;

        LevelManager.Instance.HandleLeftUp(e, _isDragging, camera);

        _isDragging = false;
    }

    /// <summary>
    /// Handles right clicks in the scene view.
    /// </summary>
    void HandleRightUp() {
        if (MouseOverUI()) return;

        if (!_lastMouseMoveWasDrag) {

            if (LevelManager.Instance.CurrentTool == LevelManager.Tool.Move &&
                LevelManager.Instance.IsMovingObject) {
                LevelManager.Instance.ResetMovingObject();
            }

            LevelManager.Instance.ResetTool();
        }
    }

    /// <summary>
    /// Draws the level editor GUI in the scene view.
    /// </summary>
    /// <param name="sc"></param>
    void DrawSceneViewGUI(SceneView sc) {
        if (_sceneView == null) _sceneView = sc;

        GUI.skin = _sceneViewGUISkin;

        // Draw toolbar
        Handles.BeginGUI();
        DrawSceneViewToolbar();

        if (_isDragging) {
            var dragPoint = Event.current.mousePosition;
            //_selectionRect = new Rect(_mouseDownScreenPos.x, _mouseDownScreenPos.y, dragPoint.x - _mouseDownScreenPos.x, dragPoint.y - _mouseDownScreenPos.y);
            _selectionRect = new Rect(_mouseDownScreenPos, dragPoint - _mouseDownScreenPos);
            Handles.DrawSolidRectangleWithOutline(_selectionRect, _DRAGBOX_FILL_COLOR, _DRAGBOX_STROKE_COLOR);
        }

        if (LevelManager.Instance == null) return;

        if (LevelManager.Instance.LevelLoaded) {

            DrawSceneViewSidePanel();
            //DrawUndoStack();
            //DrawRedoStack();

            // Draw object editor (?)
            if (LevelManager.Instance.HasSelectedObjects && LevelManager.Instance.SelectedObjects.Count == 1) {

                var selectedObject = LevelManager.Instance.SelectedObjects[0];

                var xMin = _OBJECT_EDITOR_WIDTH / 2;
                var xMax = sc.camera.pixelWidth - _OBJECT_EDITOR_WIDTH / 2;
                var yMin = _OBJECT_EDITOR_HEIGHT / 2;
                var yMax = sc.camera.pixelHeight - _OBJECT_EDITOR_HEIGHT / 2;

                var objectEditorScreenPoint = HandleUtility.WorldToGUIPoint(selectedObject.transform.position);
                objectEditorScreenPoint.x = Mathf.Clamp(objectEditorScreenPoint.x + _OBJECT_EDITOR_OFFSET_X, xMin, xMax);
                objectEditorScreenPoint.y = Mathf.Clamp(objectEditorScreenPoint.y + _OBJECT_EDITOR_OFFSET_Y, yMin, yMax);
                _objectEditorRect = new Rect(
                    objectEditorScreenPoint.x - xMin,
                    objectEditorScreenPoint.y - yMin,
                    _OBJECT_EDITOR_WIDTH, _OBJECT_EDITOR_WIDTH);

                GUILayout.Window(0, _objectEditorRect, DrawObjectEditor, selectedObject.name, GUILayout.Width(_objectEditorRect.width), GUILayout.Height(_objectEditorRect.height));
            } else _objectEditorRect = new Rect(0f, 0f, 0f, 0f);
        }

        Handles.EndGUI();
    }

    /// <summary>
    /// Draws the 3D cursor in the scene view.
    /// </summary>
    public void DrawCursor(SceneView sc) {
        if (LevelManager.Instance == null) return;
        if (!LevelManager.Instance.PreviewActive) return;

        var cursorPos = LevelManager.Instance.CursorPosition;
        var w = LevelManager.TILE_UNIT_WIDTH;

        if (LevelManager.Instance.HasSelectedObjects) {
            Color color = Color.black;
            switch (LevelManager.Instance.CurrentTool) {
                case LevelManager.Tool.Select:
                    color = Color.cyan;
                    break;

                case LevelManager.Tool.Erase:
                    color = Color.magenta;
                    break;

                case LevelManager.Tool.Move:
                    color = Color.blue;
                    break;
            }

            //DrawCube(_hoveredObject.transform.position, 
            //    _hoveredObject.transform.localScale.Max() * 1.05f, 
            //    _hoveredObject.transform.localRotation.eulerAngles.y, color);

            Handles.color = color;
            foreach (var hovered in LevelManager.Instance.HoveredObjects)
                Handles.DrawSolidDisc(hovered.transform.position, Vector3.up, hovered.transform.localScale.Max() / 2f * w);
        }

        if (LevelManager.Instance.HasSelectedObjects) {
            var selectedObject = LevelManager.Instance.SelectedObjects[0];
            if (selectedObject) {
                DrawCube(selectedObject.transform.position, selectedObject.transform.localScale.Max() * 1.05f * w, selectedObject.transform.localEulerAngles.y, Color.cyan);
                var ray = HandleUtility.GUIPointToWorldRay(_objectEditorRect.position);
                float d;
                if (LevelManager.Instance.EditingPlane.Raycast(ray, out d)) {
                    Handles.color = Color.cyan;
                    Handles.DrawLine(ray.GetPoint(d), selectedObject.transform.position);
                }
            }
        }

        switch (LevelManager.Instance.CurrentTool) {
            case LevelManager.Tool.Select:
                DrawCube(cursorPos, 1.05f * w, 0f, Color.white);
                if (LevelManager.Instance.HasSelectedObjects) {
                    foreach (GameObject obj in LevelManager.Instance.SelectedObjects)
                        DrawCube(cursorPos, 1.1f * w, -obj.transform.rotation.eulerAngles.y * Mathf.Deg2Rad, Color.cyan);
                }
                break;

            case LevelManager.Tool.Place:
                if (LevelManager.Instance.CheckPlacement() &&
                    LevelManager.Instance.CanPlaceAnotherCurrentObject())
                    DrawCube(cursorPos, 1f * w, 0f, Color.green);
                else DrawCube(cursorPos, 1f * w, 0f, Color.gray);
                break;

            case LevelManager.Tool.Erase:
                DrawCube(cursorPos, 1.1f * w, 0f, Color.red);
                break;

            case LevelManager.Tool.Move:
                if (LevelManager.Instance.CheckPlacement()) DrawCube(cursorPos, 1.1f * w, 0f, Color.blue);
                else DrawCube(cursorPos, 1.1f * w, 0f, Color.gray);
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
        if (LevelManager.Instance == null) return;
        if (!LevelManager.Instance.GridEnabled) return;

        var height = LevelManager.Instance.SelectedFloor * Level.FLOOR_HEIGHT + LevelManager.Instance.CurrentYValue;
        float w = LevelManager.TILE_UNIT_WIDTH;

        Color lineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Color edgeColor = new Color(0.65f, 0.65f, 0.65f, 0.65f);

        // Draw x lines
        for (int x = 0; x <= Level.FLOOR_WIDTH; x++) {
            Handles.color = (x == 0 || x == Level.FLOOR_WIDTH) ? edgeColor : lineColor;
            Handles.DrawLine(new Vector3((x - 0.5f) * w, height, -0.5f * w),
                new Vector3((x - 0.5f) * w, height, (Level.FLOOR_DEPTH - 0.5f) * w));
        }

        // Draw z lines
        for (int z = 0; z <= Level.FLOOR_DEPTH; z++) {
            Handles.color = (z == 0 || z == Level.FLOOR_DEPTH) ? edgeColor : lineColor;
            Handles.DrawLine(new Vector3(-0.5f * w, height, (z - 0.5f) * w), new Vector3((Level.FLOOR_WIDTH - 0.5f) * w, height, (z - 0.5f) * w));
        }
    }

    public void DrawRoomBounds(SceneView sc) {
        if (LevelManager.Instance == null) return;
        var floor = LevelManager.Instance.SelectedFloor;
        float w = LevelManager.TILE_UNIT_WIDTH;

        // Calculate upper points
        var u1 = new Vector3(-0.5f, (floor + 1) * Level.FLOOR_HEIGHT, -0.5f) * w;
        var u2 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor + 1) * Level.FLOOR_HEIGHT, -0.5f) * w;
        var u3 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f) * w;
        var u4 = new Vector3(-0.5f, (floor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f) * w;

        // Calculate lower points
        var d1 = new Vector3(-0.5f, (floor) * Level.FLOOR_HEIGHT, -0.5f) * w;
        var d2 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor) * Level.FLOOR_HEIGHT, -0.5f) * w;
        var d3 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f) * w;
        var d4 = new Vector3(-0.5f, (floor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f) * w;

        Handles.color = Color.white;

        // Draw upper rect
        Handles.DrawLine(u1, u2);
        Handles.DrawLine(u2, u3);
        Handles.DrawLine(u3, u4);
        Handles.DrawLine(u4, u1);

        // Draw connectors
        Handles.DrawLine(u1, d1);
        Handles.DrawLine(u2, d2);
        Handles.DrawLine(u3, d3);
        Handles.DrawLine(u4, d4);

        // Draw lower rect
        Handles.DrawLine(d1, d2);
        Handles.DrawLine(d2, d3);
        Handles.DrawLine(d3, d4);
        Handles.DrawLine(d4, d1);
    }

    /// <summary>
    /// Draws the GUI toolbar in the scene view.
    /// </summary>
    void DrawSceneViewToolbar() {
        if (Application.isPlaying) return;

        // Calculate toolbar rect
        var toolbarWidth = _TOOLBAR_WIDTH_PERCENT * Screen.width;
        _toolbarRect = new Rect(4, 4, toolbarWidth, _TOOLBAR_HEIGHT);
        GUILayout.BeginArea(_toolbarRect, _sceneViewGUISkin.window);
        GUILayout.BeginHorizontal();

        if (LevelManager.Instance == null) {
            GUILayout.Label("No LevelManager in scene!");
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            return;
        }

        var levelLoaded = LevelManager.Instance.LevelLoaded;
        if (!levelLoaded) GUILayout.Label("No level loaded.");
        else {

            // Show level name
            GUILayout.Label(LevelManager.Instance.LoadedLevel.Name + (LevelManager.Instance.Dirty ? "*" : ""));

            // Draw undo/redo buttons
            var undoRedoWidth = toolbarWidth * _TOOLBAR_UNDO_REDO_PERCENT;
            DrawEditorButton("Undo", LevelManager.Instance.Undo, GUILayout.Width(undoRedoWidth));
            DrawEditorButton("Redo", LevelManager.Instance.Redo, GUILayout.Width(undoRedoWidth));

            // Draw draw grid toggle
            var drawGridToggleWidth = toolbarWidth * _TOOLBAR_SHOW_GRID_PERCENT;
            var drawGrid = LevelManager.Instance.GridEnabled;
            var newDrawGrid = GUILayout.Toggle(drawGrid, "Draw Grid", GUILayout.Width(drawGridToggleWidth));
            if (drawGrid != newDrawGrid) {
                if (newDrawGrid) LevelManager.Instance.EnableGrid();
                else LevelManager.Instance.DisableGrid();
            }

            // Draw snap to grid toggle
            var snapToGridToggleWidth = toolbarWidth * _TOOLBAR_SNAP_TO_GRID_PERCENT;
            var snapToGrid = LevelManager.Instance.SnapToGrid;
            var newSnapToGrid = GUILayout.Toggle(snapToGrid, "Snap To Grid", GUILayout.Width(snapToGridToggleWidth));
            if (snapToGrid != newSnapToGrid) {
                if (newSnapToGrid) LevelManager.Instance.SnapToGrid = true;
                else LevelManager.Instance.SnapToGrid = false;
            }

            // Draw move/erase buttons
            DrawEditorButton("Move", SelectMoveTool);
            DrawEditorButton("Erase", SelectEraseTool);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Selects the move tool.
    /// </summary>
    void SelectMoveTool() {
        LevelManager.Instance.SelectTool(LevelManager.Tool.Move);
    }

    /// <summary>
    /// Selects the erase tool.
    /// </summary>
    void SelectEraseTool() {
        LevelManager.Instance.SelectTool(LevelManager.Tool.Erase);
    }

    /// <summary>
    /// Draws the side panel in the scene view.
    /// </summary>
    void DrawSceneViewSidePanel() {

        // Calculate side panel rect
        _sidePanelRect = new Rect(4, 64, _SIDEPANEL_WIDTH, _SIDEPANEL_HEIGHT);
        GUILayout.BeginArea(_sidePanelRect, _sceneViewGUISkin.window);
        GUILayout.BeginVertical();

        // Floor selector
        GUILayout.BeginVertical();
        GUILayout.Label("Floor", GUILayout.ExpandWidth(true));
        DrawEditorButton("^", LevelManager.Instance.UpOneFloor);
        GUILayout.Box (LevelManager.Instance.SelectedFloor.ToString());
        DrawEditorButton("v", LevelManager.Instance.DownOneFloor);
        GUILayout.EndVertical();
        GUILayout.Space(32);

        // Y selector
        GUILayout.BeginVertical();
        GUILayout.Label("Y-Value", GUILayout.ExpandWidth(true));
        DrawEditorButton("^", LevelManager.Instance.IncrementY);
        GUILayout.Box(LevelManager.Instance.CurrentYValue.ToString());
        DrawEditorButton("v", LevelManager.Instance.DecrementY);
        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draw undo stack
    /// </summary>
    void DrawUndoStack() {
        // Calculate rect
        _undoStackRect = new Rect(
            Screen.width - _ACTION_STACK_WIDTH,
            _ACTION_STACK_Y_OFFSET,
            _ACTION_STACK_WIDTH,
            _ACTION_STACK_HEIGHT);

        // Draw labels
        GUILayout.BeginArea(_undoStackRect);
        GUILayout.Label("Undo Stack");
        foreach (var undo in LevelManager.Instance.UndoStack)
            GUILayout.Label(undo.ToShortString());
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws the redo stack.
    /// </summary>
    void DrawRedoStack() {
        // Calculate rect
        _redoStackRect = new Rect(
            Screen.width - _ACTION_STACK_WIDTH,
            _ACTION_STACK_HEIGHT + _ACTION_STACK_Y_OFFSET,
            _ACTION_STACK_WIDTH, _ACTION_STACK_HEIGHT);

        // Draw labels
        GUILayout.BeginArea(_redoStackRect);
        GUILayout.Label("Redo Stack");
        foreach (var redo in LevelManager.Instance.RedoStack)
            GUILayout.Label(redo.ToShortString());
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws the object attribute editor.
    /// </summary>
    void DrawObjectEditor(int windowID) {
        var selectedObject = LevelManager.Instance.SelectedObjects[0];
        var attribs = LevelManager.Instance.AttributesOfObject(selectedObject);

        // Rotation label
        GUILayout.Label("Rotation", EditorStyles.whiteLabel);
        float yRotation = EditorGUILayout.FloatField("Rotation", attribs.RotationY);

        if (yRotation != attribs.RotationY) {
            var rot = attribs.EulerRotation;
            rot.y = yRotation;
            LevelManager.Instance.SetObjectEulerRotation(selectedObject, rot, LevelEditorAction.ActionType.Normal);
        }

        // Scale label
        GUILayout.Label("Scale", EditorStyles.whiteBoldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Allow non-uniform scale", EditorStyles.whiteLabel);

        // Allow non uniform scale checkbox
        //_allowNonUniformScale = GUILayout.Toggle(_allowNonUniformScale, "Allow non-uniform scale", EditorStyles.whiteLabel);
        _allowNonUniformScale = GUILayout.Toggle(_allowNonUniformScale, "");

        GUILayout.EndHorizontal();
        if (_allowNonUniformScale) {
            GUILayout.BeginVertical();

            // X scale
            var scaleX = EditorGUILayout.FloatField("Scale X", attribs.ScaleX, GUILayout.ExpandWidth(false));
            if (scaleX != attribs.ScaleX) {
                var scale = selectedObject.transform.localScale;
                scale.x = scaleX;
                LevelManager.Instance.SetObject3DScale(selectedObject, scale, LevelEditorAction.ActionType.Normal);
            }

            // Y scale
            var scaleY = EditorGUILayout.FloatField("Scale Y", attribs.ScaleY, GUILayout.ExpandWidth(false));
            if (scaleY != attribs.ScaleY) {
                var scale = selectedObject.transform.localScale;
                scale.y = scaleY;
                LevelManager.Instance.SetObject3DScale(selectedObject, scale, LevelEditorAction.ActionType.Normal);
            }

            // Z scale
            var scaleZ = EditorGUILayout.FloatField("Scale Z", attribs.ScaleZ, GUILayout.ExpandWidth(false));
            if (scaleZ != attribs.ScaleZ) {
                var scale = selectedObject.transform.localScale;
                scale.z = scaleZ;
                LevelManager.Instance.SetObject3DScale(selectedObject, scale, LevelEditorAction.ActionType.Normal);
            }
            GUILayout.EndVertical();

            // Enforce uniform scale
        } else {
            var scale = EditorGUILayout.FloatField("Scale", attribs.ScaleX);
            if (scale != attribs.ScaleX) {
                attribs.ScaleX = scale;
                attribs.ScaleY = scale;
                attribs.ScaleZ = scale;
                var scaleVec = new Vector3(scale, scale, scale);
                LevelManager.Instance.SetObject3DScale(selectedObject, scaleVec, LevelEditorAction.ActionType.Normal);
            }
        }
    }

    /// <summary>
    /// Returns true if the mouse is over any of the scene view GUI.
    /// </summary>
    bool MouseOverUI() {
        var mousePos = Event.current.mousePosition;
        return _toolbarRect.Contains(mousePos) ||
            _sidePanelRect.Contains(mousePos) ||
            _objectEditorRect.Contains(mousePos);
    }

    /// <summary>
    /// Checks that a level is loaded.
    /// </summary>
    void CheckLevelLoaded() {
        _levelLoaded = LevelManager.Instance.LevelLoaded;
    }

    /// <summary>
    /// Draws a simple editor button.
    /// </summary>
    void DrawEditorButton(string label, ButtonAction buttonAction, params GUILayoutOption[] options) {
        if (GUILayout.Button(label, options)) buttonAction();
    }

    #endregion
    #region Delegates

    /// <summary>
    /// Delegate for editor button-friendly functions.
    /// </summary>
    delegate void ButtonAction();

    #endregion
}
