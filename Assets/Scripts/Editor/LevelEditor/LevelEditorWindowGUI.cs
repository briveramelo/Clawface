﻿// LevelEditorWindowGUI.cs
// Author: Aaron

using UnityEditor;

using UnityEngine;

namespace Turing.LevelEditor
{
    public sealed partial class LevelEditorWindow :
        EditorWindow
    {
        #region Private Methods

        /// <summary>
        /// Draws the level editor window.
        /// </summary>
        void DrawLevelEditorGUI() 
        {
            // Start main scroll view
            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

            // Draw header
            DrawHeaderGUI();

            // Check if game is running
            if (Application.isPlaying) 
            {
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
            if (!levelManager) 
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No LevelManager or ObjectDatabaseManager!",
                    GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
            } 
            
            else 
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox,
                    GUILayout.ExpandWidth(true));

                // Draw create/close/load level buttons
                DrawEditorButton("Create new level", CreateNewLevel);
                DrawEditorButton("Load existing level", LoadExistingLevel);

                EditorGUILayout.EndHorizontal();

                // Draw next controls if a level is loaded
                EditorGUI.BeginDisabledGroup(!levelLoaded);

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
        void DrawHeaderGUI() 
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Project Turing Level Editor",
                EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the level settings fields.
        /// </summary>
        void DrawLevelSettingsGUI() 
        {
            // Draw label
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
        /// Draws the object browser.
        /// </summary>
        void DrawObjectBrowserGUI() 
        {
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

            if (filteredObjects != null) 
            {
                // Draw object buttons
                objectBrowserScrollPos = EditorGUILayout.BeginScrollView(objectBrowserScrollPos);
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                var buttonWidth = EditorGUIUtility.currentViewWidth / (float)OBJECTS_PER_ROW - EditorStyles.helpBox.padding.horizontal - EditorStyles.helpBox.padding.left;

                GUILayoutOption[] buttonStyle = { GUILayout.ExpandWidth(true), GUILayout.Width (buttonWidth) };

                while (objectIndex < filteredObjects.Count) {
                    if (GUILayout.Button(filteredObjects[objectIndex].prefab.name, buttonStyle)) {
                        LevelManager.Instance.SelectObjectInCategory(objectIndex, filter);
                        LevelManager.Instance.SelectTool(LevelManager.Tool.Place);
                    }

                    objectIndex++;
                    objectsInRow++;

                    if (objectsInRow >= OBJECTS_PER_ROW) {
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

        void DrawLevelEditorSettingsGUI() 
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            settingsWindowExpanded = EditorGUILayout.Foldout(
                settingsWindowExpanded, "Settings", true);

            if (settingsWindowExpanded) 
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the level editor GUI in the scene view.
        /// </summary>
        /// <param name="sc"></param>
        void DrawSceneViewGUI(SceneView sc) 
        {
            if (sceneView == null) sceneView = sc;

            GUI.skin = sceneViewGUISkin;

            // Draw toolbar
            Handles.BeginGUI();
            DrawSceneViewToolbarGUI();

            if (isDragging) 
            {
                var dragPoint = Event.current.mousePosition;
                //_selectionRect = new Rect(_mouseDownScreenPos.x, _mouseDownScreenPos.y, dragPoint.x - _mouseDownScreenPos.x, dragPoint.y - _mouseDownScreenPos.y);
                selectionRect = new Rect(mouseDownScreenPos, dragPoint - mouseDownScreenPos);
                Handles.DrawSolidRectangleWithOutline(selectionRect, DRAGBOX_FILL_COLOR, DRAGBOX_STROKE_COLOR);
            }

            if (LevelManager.Instance == null) return;

            if (LevelManager.Instance.LevelLoaded) 
            {
                DrawSceneViewSidePanelGUI();
                //DrawUndoStack();
                //DrawRedoStack();

                // Draw object editor (?)
                if (LevelManager.Instance.HasSelectedObjects && 
                    LevelManager.Instance.SelectedObjects.Count == 1) 
                {
                    var selectedObject = LevelManager.Instance.SelectedObjects[0];

                    var xMin = OBJECT_EDITOR_WIDTH / 2;
                    var xMax = sc.camera.pixelWidth - OBJECT_EDITOR_WIDTH / 2;
                    var yMin = OBJECT_EDITOR_HEIGHT / 2;
                    var yMax = sc.camera.pixelHeight - OBJECT_EDITOR_HEIGHT / 2;

                    var objectEditorScreenPoint = HandleUtility.WorldToGUIPoint(selectedObject.transform.position);
                    objectEditorScreenPoint.x = Mathf.Clamp(objectEditorScreenPoint.x + OBJECT_EDITOR_OFFSET_X, xMin, xMax);
                    objectEditorScreenPoint.y = Mathf.Clamp(objectEditorScreenPoint.y + OBJECT_EDITOR_OFFSET_Y, yMin, yMax);
                    objectEditorRect = new Rect(
                        objectEditorScreenPoint.x - xMin,
                        objectEditorScreenPoint.y - yMin,
                        OBJECT_EDITOR_WIDTH, OBJECT_EDITOR_WIDTH);

                    GUILayout.Window(0, objectEditorRect, DrawObjectEditorGUI, 
                        selectedObject.name, 
                        GUILayout.Width(objectEditorRect.width), 
                        GUILayout.Height(objectEditorRect.height));
                } 
                
                else objectEditorRect = new Rect(0f, 0f, 0f, 0f);
            }

            Handles.EndGUI();
        }

        /// <summary>
        /// Draws the GUI toolbar in the scene view.
        /// </summary>
        void DrawSceneViewToolbarGUI() 
        {
            if (Application.isPlaying) return;

            // Calculate toolbar rect
            var toolbarWidth = TOOLBAR_WIDTH_PERCENT * Screen.width;
            toolbarRect = new Rect(4, 4, toolbarWidth, TOOLBAR_HEIGHT);
            GUILayout.BeginArea(toolbarRect, sceneViewGUISkin.window);
            GUILayout.BeginHorizontal();

            if (LevelManager.Instance == null) 
            {
                GUILayout.Label("No LevelManager in scene!");
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                return;
            }

            var levelLoaded = LevelManager.Instance.LevelLoaded;
            if (!levelLoaded) GUILayout.Label("No level loaded.");

            else 
            {
                // Show level name
                GUILayout.Label(LevelManager.Instance.LoadedLevel.Name + (LevelManager.Instance.Dirty ? "*" : ""));

                // Draw undo/redo buttons
                var undoRedoWidth = toolbarWidth * TOOLBAR_UNDO_REDO_PERCENT;
                DrawEditorButton("Undo", LevelManager.Instance.Undo, GUILayout.Width(undoRedoWidth));
                DrawEditorButton("Redo", LevelManager.Instance.Redo, GUILayout.Width(undoRedoWidth));

                // Draw draw grid toggle
                var drawGridToggleWidth = toolbarWidth * TOOLBAR_SHOW_GRID_PERCENT;
                var drawGrid = LevelManager.Instance.GridEnabled;
                var newDrawGrid = GUILayout.Toggle(drawGrid, "Draw Grid", GUILayout.Width(drawGridToggleWidth));
                if (drawGrid != newDrawGrid) 
                {
                    if (newDrawGrid) LevelManager.Instance.EnableGrid();
                    else LevelManager.Instance.DisableGrid();
                }

                // Draw snap to grid toggle
                var snapToGridToggleWidth = toolbarWidth * TOOLBAR_SNAP_TO_GRID_PERCENT;
                var snapToGrid = LevelManager.Instance.SnapToGrid;
                var newSnapToGrid = GUILayout.Toggle(snapToGrid, "Snap To Grid", GUILayout.Width(snapToGridToggleWidth));
                if (snapToGrid != newSnapToGrid) 
                {
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
        /// Draws the side panel in the scene view.
        /// </summary>
        void DrawSceneViewSidePanelGUI() 
        {
            // Calculate side panel rect
            sidePanelRect = new Rect(4, 64, SIDEPANEL_WIDTH, SIDEPANEL_HEIGHT);
            GUILayout.BeginArea(sidePanelRect, sceneViewGUISkin.window);
            GUILayout.BeginVertical();

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
        void DrawUndoStackGUI() 
        {
            // Calculate rect
            undoStackRect = new Rect(
                Screen.width - ACTION_STACK_WIDTH,
                ACTION_STACK_Y_OFFSET,
                ACTION_STACK_WIDTH,
                ACTION_STACK_HEIGHT);

            // Draw labels
            GUILayout.BeginArea(undoStackRect);
            GUILayout.Label("Undo Stack");
            foreach (var undo in LevelManager.Instance.UndoStack)
                GUILayout.Label(undo.ToShortString());
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the redo stack.
        /// </summary>
        void DrawRedoStackGUI() 
        {
            // Calculate rect
            redoStackRect = new Rect(
                Screen.width - ACTION_STACK_WIDTH,
                ACTION_STACK_HEIGHT + ACTION_STACK_Y_OFFSET,
                ACTION_STACK_WIDTH, ACTION_STACK_HEIGHT);

            // Draw labels
            GUILayout.BeginArea(redoStackRect);
            GUILayout.Label("Redo Stack");
            foreach (var redo in LevelManager.Instance.RedoStack)
                GUILayout.Label(redo.ToShortString());
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the object attribute editor.
        /// </summary>
        void DrawObjectEditorGUI(int windowID) 
        {
            var selectedObject = LevelManager.Instance.SelectedObjects[0];
            var attribs = LevelManager.Instance.AttributesOfObject(selectedObject);

            // Rotation label
            GUILayout.Label("Rotation", EditorStyles.whiteLabel);
            float yRotation = EditorGUILayout.FloatField("Rotation", attribs.RotationY);

            if (yRotation != attribs.RotationY) 
            {
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
            allowNonUniformScale = GUILayout.Toggle(allowNonUniformScale, "");

            GUILayout.EndHorizontal();
            if (allowNonUniformScale) 
            {
                GUILayout.BeginVertical();

                // X scale
                var scaleX = EditorGUILayout.FloatField("Scale X", 
                    attribs.ScaleX, GUILayout.ExpandWidth(false));
                if (scaleX != attribs.ScaleX) 
                {
                    var scale = selectedObject.transform.localScale;
                    scale.x = scaleX;
                    LevelManager.Instance.SetObject3DScale(selectedObject, 
                        scale, LevelEditorAction.ActionType.Normal);
                }

                // Y scale
                var scaleY = EditorGUILayout.FloatField("Scale Y", 
                    attribs.ScaleY, GUILayout.ExpandWidth(false));
                if (scaleY != attribs.ScaleY) 
                {
                    var scale = selectedObject.transform.localScale;
                    scale.y = scaleY;
                    LevelManager.Instance.SetObject3DScale(selectedObject, 
                        scale, LevelEditorAction.ActionType.Normal);
                }

                // Z scale
                var scaleZ = EditorGUILayout.FloatField("Scale Z",
                    attribs.ScaleZ, GUILayout.ExpandWidth(false));
                if (scaleZ != attribs.ScaleZ) 
                {
                    var scale = selectedObject.transform.localScale;
                    scale.z = scaleZ;
                    LevelManager.Instance.SetObject3DScale(selectedObject, 
                        scale, LevelEditorAction.ActionType.Normal);
                }
                GUILayout.EndVertical();
            } 
            
            // Enforce uniform scale
            else 
            {
                var scale = EditorGUILayout.FloatField("Scale", 
                    attribs.ScaleX);
                if (scale != attribs.ScaleX) 
                {
                    attribs.ScaleX = scale;
                    attribs.ScaleY = scale;
                    attribs.ScaleZ = scale;
                    var scaleVec = new Vector3(scale, scale, scale);
                    LevelManager.Instance.SetObject3DScale(selectedObject, 
                        scaleVec, LevelEditorAction.ActionType.Normal);
                }
            }
        }

        /// <summary>
        /// Draws a simple editor button.
        /// </summary>
        void DrawEditorButton(string label, ButtonAction buttonAction, 
            params GUILayoutOption[] options) 
        {
            if (GUILayout.Button(label, options)) buttonAction();
        }

        #endregion
    }
}