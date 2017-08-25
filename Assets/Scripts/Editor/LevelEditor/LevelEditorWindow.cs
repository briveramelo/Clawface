// LevelEditorWindow.cs

using ModMan;

using UnityEditor;

using UnityEngine;

namespace Turing.LevelEditor
{

    /// <summary>
    /// Main level editor GUI class.
    /// </summary>
    public partial class LevelEditorWindow : EditorWindow, ILevelEditor
    {
        #region Constants

        // GUI style constants
        const string _SCENEVIEW_SKIN_PATH = "LevelEditorGUISkin.guiskin";

        // Object browser constants
        const int _OBJECTS_PER_ROW = 5;

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

        /// <summary>
        /// Is the left mouse button currently down?
        /// </summary>
        bool _leftMouseDown = false;

        /// <summary>
        /// Screen position of mouse when last pressed.
        /// </summary>
        Vector2 _mouseDownScreenPos;

        /// <summary>
        /// Is the mouse currently dragging?
        /// </summary>
        bool _isDragging = false;

        /// <summary>
        /// Current selection Rect.
        /// </summary>
        Rect _selectionRect;

        /// <summary>
        /// Current editor scene view.
        /// </summary>
        SceneView _sceneView;

        /// <summary>
        /// Last ray cast from the mouse pointer.
        /// </summary>
        Ray _pointerRay;

        /// <summary>
        /// Is the settings window currently expanded?
        /// </summary>
        bool _settingsWindowExpanded = false;

        /// <summary>
        /// GUISkin to use for the scene view GUI.
        /// </summary>
        GUISkin _sceneViewGUISkin;

        #endregion
        #region Unity Callbacks

        /// <summary>
        /// Gets and shows the instance of the editor window.
        /// </summary>
        [MenuItem("Level Editor/Editor Window")]
        public static void ShowWindow() 
        {
            _Instance = GetWindow(typeof(LevelEditorWindow))
                as LevelEditorWindow;
        }

        /// <summary>
        /// Called when the script is enabled (i.e. after compilation).
        /// </summary>
        void OnEnable() 
        {
            titleContent = new GUIContent("Level Editor");
            if (Application.isPlaying) OnEnablePlayer();
            else OnEnableEditor();
        }

        /// <summary>
        /// Called when the script is disabled (i.e. after compilation).
        /// </summary>
        void OnDisable() 
        {
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
        void OnDestroy() 
        {
            // If LevelManager exists
            if (_levelManager) 
            {
                // If a level is loaded
                if (LevelManager.Instance.LevelLoaded) 
                {
                    // Ask to save if level is dirty
                    if (LevelManager.Instance.Dirty) 
                    {
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

        // Invoked when GUI is repainted
        void OnGUI() 
        {
            if (_Instance == null)
                _Instance = GetWindow(typeof(LevelEditorWindow))
                    as LevelEditorWindow;

            // If not connected to LM, attempt to do so
            if (!Application.isPlaying &&
                (!_levelManager || !LevelManager.Instance.IsConnectedToEditor) 
                && LevelManager.Instance != null)
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

        /// <summary>
        /// Invoked when this script is enabled in the editor.
        /// </summary>
        void OnEnableEditor() 
        {
            if (LevelManager.Instance == null)
                LevelManager.OnSingletonInitializedEditor.AddListener(
                    ConnectToLevelManager);
            else ConnectToLevelManager();

            // Attempt to load GUISkin
            if (_sceneViewGUISkin == null)
                _sceneViewGUISkin = EditorGUIUtility.Load(_SCENEVIEW_SKIN_PATH)
                    as GUISkin;

            // If failed to load GUISkin, show error
            if (_sceneViewGUISkin == null) 
                Debug.LogError("Failed to load SceneView GUISkin!");
        }

        /// <summary>
        /// Invoked when this script is enabled in the playe.
        /// </summary>
        void OnEnablePlayer() 
        {
            //Debug.Log("LevelEditorWindow.OnEnablePlayer");
        }

        /// <summary>
        /// Connects the editor window the LevelManager.
        /// </summary>
        void ConnectToLevelManager() 
        {
            // Start editing
            LevelManager.Instance.StartEditing();
            LevelManager.Instance.SetEditor(this);
            _levelManager = LevelManager.Instance;

            // Add event listeners
            LevelManager.Instance.onCloseLevel.AddListener(CheckLevelLoaded);
            LevelManager.Instance.onCreateLevel.AddListener(CheckLevelLoaded);
            LevelManager.Instance.onLoadLevel.AddListener(CheckLevelLoaded);
            LevelManager.Instance.onSaveLevel.AddListener(CheckLevelLoaded);

            // Register scene view delegates
            SceneView.onSceneGUIDelegate += DrawGrid;
            SceneView.onSceneGUIDelegate += HandleInputs;
            SceneView.onSceneGUIDelegate += DrawCursor;
            SceneView.onSceneGUIDelegate += DrawSceneViewGUI;
            SceneView.onSceneGUIDelegate += DrawRoomBounds;
        }



        /// <summary>
        /// Creates a new level.
        /// </summary>
        void CreateNewLevel() 
        {
            // Prompt to save current level if loaded and dirty
            if (_levelLoaded && LevelManager.Instance.Dirty) {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", 
                    "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevelToJSON();
            }

            LevelManager.Instance.CreateNewLevel();
        }

        /// <summary>
        /// Closes the current level, saving its path.
        /// </summary>
        void CloseLevelAndKeepPath() 
        {
            // Save level path
            var path = LevelManager.Instance.LoadedLevelPath;
            EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, path);

            CloseLevel();
        }

        /// <summary>
        /// Closes the current level and discards its path.
        /// </summary>
        void CloseLevel() 
        {
            // If dirty, prompt to save
            if (LevelManager.Instance.Dirty) {
                var option = EditorUtility.DisplayDialogComplex(
                    "Close current level",
                    "Do you wish to save the current level?",
                    "Save",
                    "Don't Save",
                    "Cancel");

                switch (option) 
                {
                    // Save level
                    case 0: 
                        LevelManager.Instance.SaveCurrentLevelToJSON();
                        LevelManager.Instance.CloseLevel();
                        EditorPrefs.SetString(
                            LevelManager.LAST_EDITED_LEVEL_STR, "");
                        break;

                    // Don't save level
                    case 1: 
                        LevelManager.Instance.CloseLevel();
                        EditorPrefs.SetString(
                            LevelManager.LAST_EDITED_LEVEL_STR, "");
                        break;

                    // Cancel
                    case 2: 
                        break;
                }
            } 
            
            // If not dirty, just close
            else 
            {
                LevelManager.Instance.CloseLevel();
                EditorPrefs.SetString(LevelManager.LAST_EDITED_LEVEL_STR, "");
            }

            // Force a GUI repaint
            Repaint();
        }

        /// <summary>
        /// Loads an existing level.
        /// </summary>
        void LoadExistingLevel() 
        {
            // If level loaded and dirty, prompt to save first
            if (_levelLoaded && LevelManager.Instance.Dirty) 
            {
                if (EditorUtility.DisplayDialog("Save current level",
                    "Do you wish to save the current level?", "Save", "Don't Save"))
                    LevelManager.Instance.SaveCurrentLevelToJSON();
            }

            LevelManager.Instance.LoadLevelFromJSON();
        }

        /// <summary>
        /// Saves the current level.
        /// </summary>
        void SaveCurrentLevel() 
        {
            LevelManager.Instance.SaveCurrentLevelToJSON();
        }

        /// <summary>
        /// Saves the current level, forcing the file browser to open.
        /// </summary>
        void SaveAsNewLevel() 
        {
            LevelManager.Instance.SaveCurrentLevelToJSON(true);
        }

        /// <summary>
        /// Disconnects from the LevelManager.
        /// </summary>
        void DisconnectFromLevelManager() 
        {
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
        void HandleInputs(SceneView sc) 
        {
            // Don't do anything while playing
            if (Application.isPlaying) return;

            // Get current GUI event
            var e = Event.current;

            // No idea why this works, but this allows 
            // MouseUp events to be processed for left mouse button.
            // It also disables default handles.
            if (e.rawType == EventType.layout) 
            {
                HandleUtility.AddDefaultControl(
                    GUIUtility.GetControlID(GetHashCode(),
                    FocusType.Passive));
                return;
            }

            // Key events
            if (e.isKey) 
            {
                // Ctrl
                if (e.control) 
                {
                    // Ctrl + Shift
                    if (e.shift) 
                    {
                        // Ctrl + Shift + S (Save as New)
                        if (e.keyCode == KeyCode.S) 
                        { 
                            if (LevelManager.Instance.LevelLoaded)
                                LevelManager.Instance.SaveCurrentLevelToJSON(true);
                        }
                    } 
                    
                    // Ctrl
                    else 
                    {
                        switch (e.keyCode) 
                        {
                            // Ctrl + R (Redo)
                            case KeyCode.R:
                                if (LevelManager.Instance.RedoStack.Count > 0)
                                    LevelManager.Instance.Redo();

                                else 
                                {
                                    try 
                                    {
                                        Undo.PerformRedo();
                                    } 
                                    
                                    catch (System.NullReferenceException) { }
                                }

                                e.Use();
                                break;
                            
                            // Ctrl + S (Save)
                            case KeyCode.S: 
                                if (LevelManager.Instance.LevelLoaded)
                                    LevelManager.Instance.SaveCurrentLevelToJSON();
                                break;
                            
                            // Ctrl + Z (Undo)
                            case KeyCode.Z: 
                                if (LevelManager.Instance.UndoStack.Count > 0)
                                    LevelManager.Instance.Undo();

                                else 
                                { 
                                    try 
                                    {
                                        Undo.PerformUndo();
                                    } 
                                    
                                    catch (System.NullReferenceException) { }
                                }

                                e.Use();
                                break;
                        }
                    }
                }
            } 

            // Mouse events
            else if (e.isMouse) 
            {
                if (LevelManager.Instance != null) 
                {
                    // Check if mouse if over UI
                    if (MouseOverUI()) LevelManager.Instance.MouseOverUI = true;
                    else LevelManager.Instance.MouseOverUI = false;

                    HandleMouseMove(sc, e);

                    if (e.rawType == EventType.layout)
                        HandleUtility.AddDefaultControl(
                            GUIUtility.GetControlID(GetHashCode(), 
                            FocusType.Passive));

                    HandleInputs(e, sc.camera);
                }
            }
        }

        /// <summary>
        /// Handles inputs.
        /// </summary>
        public void HandleInputs(Event currEvent, Camera camera) 
        {
            // Mouse events
            if (currEvent.isMouse) 
            {
                switch (currEvent.rawType) 
                {
                    // Drag event
                    case EventType.MouseDrag:
                        _lastMouseMoveWasDrag = true;
                        break;

                    // MouseDown event
                    case EventType.MouseDown:
                        _lastMouseMoveWasDrag = false;
                        switch (currEvent.button) 
                        {
                            // Left mouse button down
                            case 0: 
                                HandleLeftDown(currEvent);
                                break;
                        }
                        break;

                    // MouseUp event
                    case EventType.MouseUp:
                        switch (currEvent.button) 
                        {
                            // Left click
                            case 0: 
                                HandleLeftUp(currEvent, camera);
                                break;

                            // Right click
                            case 1: 
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
        public void HandleMouseMove(SceneView sc, Event e) 
        {
            // If mouse is moved
            if (e.delta != Vector2.zero) 
            {
                // Check if dragging
                if (_leftMouseDown)
                    _isDragging = Vector2.Distance(_mouseDownScreenPos, e.mousePosition) >= _DRAG_DIST;

                // Calculate pointer ray
                _pointerRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                LevelManager.Instance.HandleMouseMove(e, _isDragging);

                // Force a repaint to show changes
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// Handles a left mouse click.
        /// </summary>
        void HandleLeftDown(Event e) 
        {
            _leftMouseDown = true;
            _mouseDownScreenPos = e.mousePosition;
            LevelManager.Instance.HandleLeftDown(e);
        }

        public void HandleLeftDownOnButton() 
        {
            //Debug.Log ("down");
            /*var selectedObjects = LevelManager.Instance.SelectedObjects;
            if (selectedObjects != null) {
                _originalRotation = selectedObject.transform.localRotation.eulerAngles;
                _originalScale = selectedObject.transform.localScale;
            }*/
        }

        public void HandleLeftUpOnButton() 
        {
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
        public void HandleLeftUp(Event e, Camera camera) 
        {
            _leftMouseDown = false;

            if (MouseOverUI()) return;

            LevelManager.Instance.HandleLeftUp(e, _isDragging, camera);

            _isDragging = false;
        }

        /// <summary>
        /// Handles right clicks in the scene view.
        /// </summary>
        void HandleRightUp() 
        {
            if (MouseOverUI()) return;

            if (!_lastMouseMoveWasDrag) 
            {

                if (LevelManager.Instance.CurrentTool == LevelManager.Tool.Move 
                    && LevelManager.Instance.IsMovingObject) 
                {
                    LevelManager.Instance.ResetMovingObject();
                }

                LevelManager.Instance.ResetTool();
            }
        }



        /// <summary>
        /// Draws the 3D cursor in the scene view.
        /// </summary>
        public void DrawCursor(SceneView sc) 
        {
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

            if (LevelManager.Instance.HasSelectedObjects) 
            {
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

            switch (LevelManager.Instance.CurrentTool) 
            {
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
        void DrawCube(Vector3 center, float edgeWidth, float rotation, Color color) 
        {
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
        public void DrawGrid(SceneView sc) 
        {
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

        public void DrawRoomBounds(SceneView sc) 
        {
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
        /// Selects the move tool.
        /// </summary>
        void SelectMoveTool() 
        {
            LevelManager.Instance.SelectTool(LevelManager.Tool.Move);
        }

        /// <summary>
        /// Selects the erase tool.
        /// </summary>
        void SelectEraseTool() 
        {
            LevelManager.Instance.SelectTool(LevelManager.Tool.Erase);
        }

        /// <summary>
        /// Returns true if the mouse is over any of the scene view GUI.
        /// </summary>
        bool MouseOverUI() 
        {
            var mousePos = Event.current.mousePosition;
            return _toolbarRect.Contains(mousePos) ||
                _sidePanelRect.Contains(mousePos) ||
                _objectEditorRect.Contains(mousePos);
        }

        /// <summary>
        /// Checks that a level is loaded.
        /// </summary>
        void CheckLevelLoaded() 
        {
            _levelLoaded = LevelManager.Instance.LevelLoaded;
        }

        #endregion
        #region Delegates

        /// <summary>
        /// Delegate for editor button-friendly functions.
        /// </summary>
        delegate void ButtonAction();

        #endregion
    }
}