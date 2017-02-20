// LevelEditorWindow.cs

using UnityEngine;

#if UNITY_EDITOR
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

    // Object editor constants
    const float _OBJECT_EDITOR_WIDTH = 128f;
    const float _OBJECT_EDITOR_HEIGHT = 256f;

    const float _OBJECT_EDITOR_OFFSET_X = 8f;
    const float _OBJECT_EDITOR_OFFSET_Y = 8f;

    const float _TOOLBAR_WIDTH_PERCENT = 0.6f;
    const float _TOOLBAR_HEIGHT_PERCENT = 0.1f;
    const float _TOOLBAR_UNDO_REDO_PERCENT = 0.15f;
    const float _TOOLBAR_SHOW_GRID_PERCENT = 0.2f;
    const float _TOOLBAR_SNAP_TO_GRID_PERCENT = 0.2f;

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

    GameObject _hoveredObject;

    Vector3 _originalRotation;
    Vector3 _originalScale;

    /// <summary>
    /// Connected LM.
    /// </summary>
    LevelManager _levelManager;

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
        if (Application.isPlaying) OnEnablePlayer();
        else OnEnableEditor();
    }

    /// <summary>
    /// Called when the script is disabled (i.e. after compilation).
    /// </summary>
    void OnDisable() {
        //Debug.Log("LevelEditorWindow.OnDisable");
    }

    /// <summary>
    /// Called when the editor window is destroyed (closed, compiled).
    /// </summary>
    void OnDestroy() {
        //Debug.Log("LevelEditorWindow.OnDestroy");
        if (_levelManager) {
            if (LevelManager.Instance.LevelLoaded && LevelManager.Instance.Dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?",
                    "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevelToJSON();

                CloseLevelAndKeepPath();
            }

            DisconnectFromLevelManager();
        }

        // Deregister delegates
        SceneView.onSceneGUIDelegate -= DrawGrid;
        SceneView.onSceneGUIDelegate -= HandleInputs;
        SceneView.onSceneGUIDelegate -= DrawCursor;
        SceneView.onSceneGUIDelegate -= DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate -= DrawRoomBounds;
    }

    void OnGUI() {
        if (_Instance == null)
            _Instance = GetWindow(typeof(LevelEditorWindow))
                as LevelEditorWindow;

        // If not connected to LM, attempt to
        if (!_levelManager) ConnectToLevelManager();

        // Draw window GUI
        DrawLevelEditorGUI();
    }

    #endregion
    #region Methods

    void OnEnableEditor() {
        //Debug.Log("LevelEditorWindow.OnEnableEditor");
        LevelManager.onSingletonInitialized.AddListener(ConnectToLevelManager);

        // Register delegates
        SceneView.onSceneGUIDelegate += DrawGrid;
        SceneView.onSceneGUIDelegate += HandleInputs;
        SceneView.onSceneGUIDelegate += DrawCursor;
        SceneView.onSceneGUIDelegate += DrawSceneViewGUI;
        SceneView.onSceneGUIDelegate += DrawRoomBounds;
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
        _levelManager = LevelManager.Instance;

        // Add event listeners
        LevelManager.Instance.onCloseLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onCreateLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onLoadLevel.AddListener(CheckLevelLoaded);
        LevelManager.Instance.onSaveLevel.AddListener(CheckLevelLoaded);
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
            EditorGUILayout.LabelField(@"Level editor disabled while game is 
                running!\nUse game level editor instead.",
                EditorStyles.boldLabel);
            EditorGUILayout.EndScrollView();
            return;
        }

        // If not connected, there is probably no LevelManager or
        // ObjectDatabaseManager in the scene
        if (!_levelManager) {
            EditorGUILayout.LabelField("No LevelManager or ObjectDatabaseManager!",
                GUILayout.ExpandWidth(true));
        } else {

            // Draw create/close/load level buttons
            DrawEditorButton("Create new level", CreateNewLevel);
            DrawEditorButton("Load existing level", LoadExistingLevel);

            // Draw next controls if a level is loaded
            if (_levelLoaded) {

                // Draw level settings
                EditorGUILayout.Space();
                DrawLevelSettingsGUI();
                EditorGUILayout.Space();

                DrawEditorButton("Close level", CloseLevel);

                // Draw save level button
                DrawEditorButton("Save current level", SaveCurrentLevel);
                DrawEditorButton("Save as new level", SaveAsNewLevel);
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                // Draw object browser
                DrawObjectBrowserGUI();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws the editor header.
    /// </summary>
    void DrawHeaderGUI() {
        EditorGUILayout.LabelField("Under The Skin\nLevel Editor",
            LevelEditorStyles.Header,
            GUILayout.ExpandHeight(true),
            GUILayout.MaxHeight(64));
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
            if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                LevelManager.Instance.SaveCurrentLevelToJSON();
        }

        LevelManager.Instance.CloseLevel();
        EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, "");
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
        if (!LevelManager.Instance.LevelLoaded) return;
        var levelName = LevelManager.Instance.LoadedLevel.Name;
        var newName = EditorGUILayout.TextField("Name", levelName);
        if (newName != levelName)
            LevelManager.Instance.SetLoadedLevelName(newName);
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
        EditorGUILayout.LabelField("Object Browser", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        // Draw filter buttons
        EditorGUILayout.BeginHorizontal();
        for (int i = 1; i < (int)ObjectDatabase.Category.COUNT; i++) {
            bool filterSelected = i == (int)LevelManager.Instance.CurrentObjectFilter;
            var style = filterSelected ? LevelEditorStyles.SelectedButton : LevelEditorStyles.NormalButton;
            if (GUILayout.Button(((ObjectDatabase.Category)i).ToString(), style))
                LevelManager.Instance.SetObjectFilter((ObjectDatabase.Category)i);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

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
                    if (e.keyCode == KeyCode.S) { // Ctrl + S
                        if (LevelManager.Instance.LevelLoaded)
                            LevelManager.Instance.SaveCurrentLevelToJSON();
                    }
                }
            }

            // Mouse events
        } else if (e.isMouse) {
            if (MouseOverUI()) LevelManager.Instance.MouseOverUI = true;
            else LevelManager.Instance.MouseOverUI = false;

            HandleMouseMove(e);

            if (e.rawType == EventType.layout)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

            HandleInputs(e, sc.camera);
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
    public void HandleMouseMove(Event e) {
        // If mouse is moved
        if (e.delta != Vector2.zero) {

            // Update 3D cursor position
            Vector3 cursorPos;
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float distance;
            if (LevelManager.Instance.EditingPlane.Raycast(ray, out distance)) {
                cursorPos = ray.GetPoint(distance);
                LevelManager.Instance.SetCursorPosition(cursorPos);
            }

            if (LevelManager.Instance.CurrentTool != LevelManager.Tool.Place) {

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    ObjectSpawner spawner = hit.collider.gameObject.GetComponentInAncestors<ObjectSpawner>();
                    if (spawner != null) _hoveredObject = spawner.gameObject;
                } else _hoveredObject = null;
            }

            // Force a repaint to show changes
            SceneView.RepaintAll();
        }
    }

    public void HandleLeftDownOnButton() {
        //Debug.Log ("down");
        var selectedObject = LevelManager.Instance.SelectedObject;
        if (selectedObject != null) {
            _originalRotation = selectedObject.transform.localRotation.eulerAngles;
            _originalScale = selectedObject.transform.localScale;
        }
    }

    public void HandleLeftUpOnButton() {
        //Debug.Log("up");

        // Record attribute change if necessary
        var selectedObject = LevelManager.Instance.SelectedObject;
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
        }
    }

    /// <summary>
    /// Handles left click events.
    /// </summary>
    public void HandleLeftUp(Event e, Camera camera) {

        if (MouseOverUI()) return;

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
                    var spawner = clickedObject.GetComponent<ObjectSpawner>();
                    if (spawner == null) spawner = clickedObject.GetComponentInAncestors<ObjectSpawner>();
                    if (spawner != null) LevelManager.Instance.DeleteObject(spawner.gameObject, ActionType.Normal);
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

        // Draw toolbar
        Handles.BeginGUI();
        DrawSceneViewToolbar();

        if (LevelManager.Instance.LevelLoaded) {

            DrawSceneViewSidePanel();
            //DrawUndoStack();
            //DrawRedoStack();

            // Draw object editor (?)
            if (LevelManager.Instance.HasSelectedObject) {
                var selectedObject = LevelManager.Instance.SelectedObject;

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
        if (!LevelManager.Instance.PreviewActive) return;

        var cursorPos = LevelManager.Instance.CursorPosition;

        if (_hoveredObject) {
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
            Handles.DrawSolidDisc (_hoveredObject.transform.position, Vector3.up, _hoveredObject.transform.localScale.Max() / 2f);
        }

        var selectedObject = LevelManager.Instance.SelectedObject;
        if (selectedObject) {
            DrawCube(selectedObject.transform.position, selectedObject.transform.localScale.Max() * 1.05f, selectedObject.transform.localEulerAngles.y, Color.cyan);
            var ray = HandleUtility.GUIPointToWorldRay(_objectEditorRect.position);
            float d;
            if (LevelManager.Instance.EditingPlane.Raycast(ray, out d)) {
                Handles.color = Color.cyan;
                Handles.DrawLine(ray.GetPoint(d), selectedObject.transform.position);
            }
        }

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

        // Draw x lines
        for (int x = 0; x <= Level.FLOOR_WIDTH; x++) {
            Handles.color = (x == 0 || x == Level.FLOOR_WIDTH) ? edgeColor : lineColor;
            Handles.DrawLine(new Vector3(x - 0.5f, height, -0.5f),
                new Vector3(x - 0.5f, height, Level.FLOOR_DEPTH - 0.5f));
        }

        // Draw z lines
        for (int z = 0; z <= Level.FLOOR_DEPTH; z++) {
            Handles.color = (z == 0 || z == Level.FLOOR_DEPTH) ? edgeColor : lineColor;
            Handles.DrawLine(new Vector3(-0.5f, height, z - 0.5f), new Vector3(Level.FLOOR_WIDTH - 0.5f, height, z - 0.5f));
        }
    }

    public void DrawRoomBounds(SceneView sc) {
        var floor = LevelManager.Instance.SelectedFloor;

        // Calculate upper points
        var u1 = new Vector3(-0.5f, (floor + 1) * Level.FLOOR_HEIGHT, -0.5f);
        var u2 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor + 1) * Level.FLOOR_HEIGHT, -0.5f);
        var u3 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);
        var u4 = new Vector3(-0.5f, (floor + 1) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);

        // Calculate lower points
        var d1 = new Vector3(-0.5f, (floor) * Level.FLOOR_HEIGHT, -0.5f);
        var d2 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor) * Level.FLOOR_HEIGHT, -0.5f);
        var d3 = new Vector3(Level.FLOOR_WIDTH - 0.5f, (floor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);
        var d4 = new Vector3(-0.5f, (floor) * Level.FLOOR_HEIGHT, Level.FLOOR_DEPTH - 0.5f);

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

        // Calculate toolbar rect
        var toolbarWidth = _TOOLBAR_WIDTH_PERCENT * Screen.width;
        _toolbarRect = new Rect(4, 4, toolbarWidth, _TOOLBAR_HEIGHT_PERCENT * Screen.height);
        GUILayout.BeginArea(_toolbarRect);
        GUILayout.BeginHorizontal();

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
        _sidePanelRect = new Rect(4, 40, 32, 256);
        GUILayout.BeginArea(_sidePanelRect);
        GUILayout.BeginVertical();

        // Floor selector
        GUILayout.BeginVertical();
        GUILayout.Label("Floor", GUILayout.ExpandWidth(true));
        DrawEditorButton("^", LevelManager.Instance.UpOneFloor);
        GUILayout.Label(LevelManager.Instance.SelectedFloor.ToString(), GUILayout.ExpandWidth(true));
        DrawEditorButton("v", LevelManager.Instance.DownOneFloor);
        GUILayout.EndVertical();
        GUILayout.Space(32);

        // Y selector
        GUILayout.BeginVertical();
        GUILayout.Label("Y", GUILayout.ExpandWidth(true));
        DrawEditorButton("^", LevelManager.Instance.IncrementY);
        GUILayout.Label(LevelManager.Instance.CurrentYValue.ToString(), GUILayout.ExpandWidth(true));
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
        var selectedObject = LevelManager.Instance.SelectedObject;
        var attribs = LevelManager.Instance.AttributesOfObject(selectedObject);

        // Rotation label
        GUILayout.Label("Rotation", EditorStyles.whiteLabel);
        float yRotation = EditorGUILayout.FloatField("Rotation", attribs.RotationY);

        if (yRotation != attribs.RotationY) {
            var rot = attribs.EulerRotation;
            rot.y = yRotation;
            LevelManager.Instance.SetObjectEulerRotation(selectedObject, rot, ActionType.Normal);
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
                LevelManager.Instance.SetObject3DScale(selectedObject, scale, ActionType.Normal);
            }

            // Y scale
            var scaleY = EditorGUILayout.FloatField("Scale Y", attribs.ScaleY, GUILayout.ExpandWidth(false));
            if (scaleY != attribs.ScaleY) {
                var scale = selectedObject.transform.localScale;
                scale.y = scaleY;
                LevelManager.Instance.SetObject3DScale(selectedObject, scale, ActionType.Normal);
            }

            // Z scale
            var scaleZ = EditorGUILayout.FloatField("Scale Z", attribs.ScaleZ, GUILayout.ExpandWidth(false));
            if (scaleZ != attribs.ScaleZ) {
                var scale = selectedObject.transform.localScale;
                scale.z = scaleZ;
                LevelManager.Instance.SetObject3DScale(selectedObject, scale, ActionType.Normal);
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
                LevelManager.Instance.SetObject3DScale(selectedObject, scaleVec, ActionType.Normal);
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
#endif
