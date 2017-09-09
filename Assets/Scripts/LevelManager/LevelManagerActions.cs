// LevelManagerActions.cs
// Author: Aaron

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Holds functionality for actions done through LM.
    /// </summary>
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Public Fields

        /// <summary>
        /// Invoked when an object is selected.
        /// </summary>
        [HideInInspector]
        public UnityEvent onSelectObject = new UnityEvent();

        /// <summary>
        /// Invoked when an object is deselected.
        /// </summary>
        [HideInInspector]
        public UnityEvent onDeselectObject = new UnityEvent();

        #endregion
        #region Private Fields

        /// <summary>
        /// Current state of the editor tool.
        /// </summary>
        Tool currentTool = Tool.Select;

        /// <summary>
        /// Are there unsaved changes?
        /// </summary>
        bool dirty = false;

        /// <summary>
        /// ID of object being moved.
        /// </summary>
        GameObject movingObject;

        /// <summary>
        /// Is object placement allowed at the current location?
        /// </summary>
        bool placementAllowed = true;

        /// <summary>
        /// Original coordinates of object being moved;
        /// </summary>
        Vector3 movingObjectOriginalCoords;

        /// <summary>
        /// Index of the currently selected object.
        /// </summary>
        int selectedObjectIndexForPlacement;

        /// <summary>
        /// Snap mode of the selected object.
        /// </summary>
        ObjectDatabase.SnapMode selectedObjectSnapMode;

        /// <summary>
        /// Current editing y position.
        /// </summary>
        int currentYPosition = 0;

        /// <summary>
        /// Current editing y rotation.
        /// </summary>
        float currentPlacementYRotation = 0f;

        /// <summary>
        /// Stack of undo actions.
        /// </summary>
        Stack<LevelEditorAction> undoStack = new Stack<LevelEditorAction>();

        /// <summary>
        /// Stack of redo actions.
        /// </summary>
        Stack<LevelEditorAction> redoStack = new Stack<LevelEditorAction>();

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns true if unsaved changes exist (read-only).
        /// </summary>
        public bool Dirty { get { return dirty; } }

        /// <summary>
        /// Returns true if an object is currently selected for placement 
        /// (read-only).
        /// </summary>
        public bool HasSelectedObjectForPlacement
        {
            get { return selectedObjectIndexForPlacement != -1; }
        }

        /// <summary>
        /// Returns the current editing y-value (read-only).
        /// </summary>
        public int CurrentYValue { get { return currentYPosition; } }

        /// <summary>
        /// Returns the current tool (read-only).
        /// </summary>
        public Tool CurrentTool { get { return currentTool; } }

        /// <summary>
        /// Returns the undo stack (read-only).
        /// </summary>
        public Stack<LevelEditorAction> UndoStack { get { return undoStack; } }

        /// <summary>
        /// Returns the redo stack (read-only).
        /// </summary>
        public Stack<LevelEditorAction> RedoStack { get { return redoStack; } }

        /// <summary>
        /// Returns true if an object is being moved (read-only).
        /// </summary>
        public bool IsMovingObject { get { return movingObject != null; } }

        /// <summary>
        /// Sets the dirty status of the LM.
        /// </summary>
        public void SetDirty(bool dirty) { this.dirty = dirty; }

        /// <summary>
        /// Selects the given tool.
        /// </summary>
        public void SelectTool(Tool tool)
        {
            currentTool = tool;
            if (tool != Tool.Place) DisablePreview();
        }

        /// <summary>
        /// Resets the currently selected tool (to placement).
        /// </summary>
        public void ResetTool()
        {
            selectedObjectIndexForPlacement = -1;
            DisablePreview();
            currentTool = Tool.Select;
        }

        /// <summary>
        /// Moves the editing plane up by one tile.
        /// </summary>
        public void IncrementY()
        {
            if (currentYPosition < Level.LEVEL_HEIGHT - TILE_UNIT_WIDTH)
            {
                currentYPosition++;
                UpdateHeight();
            }
        }

        /// <summary>
        /// Moves the editing plane down by one tile.
        /// </summary>
        public void DecrementY()
        {
            if (currentYPosition > 0)
            {
                currentYPosition--;
                UpdateHeight();
            }
        }

        /// <summary>
        /// Updates the editing plane according to the current floor and 
        /// y-value.
        /// </summary>
        public void UpdateHeight()
        {
            editingPlane.SetNormalAndPosition(Vector3.up,
                new Vector3(0f, currentYPosition, 0f));
        }

        /// <summary>
        /// Creates one of the currently selected objects at the cursor.
        /// </summary>
        public void CreateCurrentSelectedObjectAtCursor()
        {
            CreateObject((byte)selectedObjectIndexForPlacement,
                cursorPosition, LevelEditorAction.ActionType.Normal);
        }

        /// <summary>
        /// Crates an object with the given index.
        /// </summary>
        public void CreateObject(byte index, Vector3 position,
            LevelEditorAction.ActionType actionType)
        {
            // Warn if invalid index
            if (index < 0 || index >= byte.MaxValue)
                throw new IndexOutOfRangeException("Invalid index! " + index);

            // Add object to level file
            loadedLevel.AddObject((int)index, position,
                currentPlacementYRotation);

            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log(currentPlacementYRotation);

            // Create spawner
            GameObject spawner = CreateSpawner(index);
            spawner.transform.SetParent(loadedLevelObject.transform);
            spawner.transform.position = position;
            spawner.transform.localRotation = Quaternion.Euler(0f, 
                currentPlacementYRotation, 0f);
            loadedSpawners.Add(spawner.GetComponent<ObjectSpawner>());
            objectCounts[(int)index]++;
            dirty = true;

            // Update undo/redo stack
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(new CreateObjectAction(index, spawner));
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(new DeleteObjectAction(spawner, index));
                    break;
            }
        }

        /// <summary>
        /// Deletes the given object.
        /// </summary>
        public void DeleteObject(GameObject obj, 
            LevelEditorAction.ActionType actionType)
        {
            // Get database index of object
            byte deletedObjectIndex = (byte)ObjectIndexOfObject(obj);
            int levelIndex = LevelIndexOfObject(obj);

            // Update undo/redo stack
            var deleteAction = new DeleteObjectAction(obj, deletedObjectIndex);
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(deleteAction);
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(
                        new CreateObjectAction(deletedObjectIndex, obj));
                    break;
            }

            // Delete object from level
            loadedLevel.DeleteObject(levelIndex);

            // Delete spawner
            objectCounts[deletedObjectIndex]--;
            loadedSpawners.Remove(obj.GetComponent<ObjectSpawner>());
            DestroyLoadedObject(obj);
            dirty = true;
        }

        /// <summary>
        /// Moves an object.
        /// </summary>
        public void MoveObject(GameObject obj, Vector3 oldPos, Vector3 newPos,
            LevelEditorAction.ActionType actionType)
        {
            var moveAction = new MoveObjectAction(obj, oldPos, newPos);
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(moveAction);
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(new MoveObjectAction(obj, newPos, oldPos));
                    break;
            }

            //var attribs = AttributesOfObject(obj);
            SetObjectPosition(obj, newPos, actionType);
        }

        /// <summary>
        /// Sets the position of an object.
        /// </summary>
        public void SetObjectPosition(GameObject obj, Vector3 pos,
            LevelEditorAction.ActionType actionType)
        {
            var attribs = AttributesOfObject(obj);
            attribs.SetPosition(pos);
            obj.transform.position = pos;
        }

        /// <summary>
        /// Sets an object's Euler rotation.
        /// </summary>
        public void SetObjectEulerRotation(GameObject obj, Vector3 rot,
            LevelEditorAction.ActionType actionType)
        {
            var attribs = AttributesOfObject(obj);
            attribs.SetEulerRotation(rot);
            obj.transform.localRotation = Quaternion.Euler(rot);
        }

        /// <summary>
        /// Sets an object's 3D scale.
        /// </summary>
        public void SetObject3DScale(GameObject obj, Vector3 scale,
            LevelEditorAction.ActionType actionType)
        {
            var attribs = AttributesOfObject(obj);
            attribs.Set3DScale(scale);
            obj.transform.localScale = scale;
        }

        /// <summary>
        /// Recrods a change to a standard attribute.
        /// </summary>
        public void RecordAttributeChange(GameObject obj,
            ChangeObjectNormalAttributeAction.AttributeChanged attrib,
            Vector3 oldValue, Vector3 newValue, 
            LevelEditorAction.ActionType actionType)
        {
            switch (actionType)
            {
                case LevelEditorAction.ActionType.Normal:
                case LevelEditorAction.ActionType.Redo:
                    undoStack.Push(
                        new ChangeObjectNormalAttributeAction(
                            obj, attrib, newValue));
                    break;

                case LevelEditorAction.ActionType.Undo:
                    redoStack.Push(
                        new ChangeObjectNormalAttributeAction(
                            obj, attrib, oldValue));
                    break;
            }
        }

        /// <summary>
        /// Undoes the most recent action.
        /// </summary>
        public void Undo()
        {
            if (undoStack.Count <= 0) return;

            var undoAction = undoStack.Pop();
            undoAction.Undo();
        }

        /// <summary>
        /// Redoes the most recent action.
        /// </summary>
        public void Redo()
        {
            if (redoStack.Count <= 0) return;

            var redoAction = redoStack.Pop();
            redoAction.Redo();
        }

        /// <summary>
        /// Starts moving an object.
        /// </summary>
        public void StartMovingObject(GameObject obj)
        {
            ShowAssetPreview(obj);
            movingObjectOriginalCoords = obj.transform.position;
            movingObject = obj;
            //DeleteObject(obj, ActionType.None);
        }

        /// <summary>
        /// Stops moving an object.
        /// </summary>
        public void StopMovingObject()
        {
            if (placementAllowed)
            {
                MoveObject(movingObject, movingObjectOriginalCoords,
                    cursorPosition, LevelEditorAction.ActionType.Normal);
                ResetMovingObject();
            }
        }

        /// <summary>
        /// Resets the object being moved.
        /// </summary>
        public void ResetMovingObject()
        {
            movingObject = null;
            DisablePreview();
        }

        /// <summary>
        /// Creates a spawner for the given template.
        /// </summary>
        public GameObject CreateSpawner(GameObject template)
        {
            GameObject spawner = new GameObject(template.name +
                " Spawner", typeof(ObjectSpawner));
            spawner.GetComponent<ObjectSpawner>().SetTemplate(template);
            return spawner;
        }

        /// <summary>
        /// Returns a new instance of the object with the given index.
        /// </summary>
        public GameObject CreateSpawner(byte index)
        {
            var template = ObjectDatabaseManager.Instance.GetObject(index);
            return CreateSpawner(template);
        }

        #endregion
    }
}