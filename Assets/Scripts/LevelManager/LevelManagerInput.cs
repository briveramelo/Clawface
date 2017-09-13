// LevelManagerInput.cs
// Author: Aaron

using ModMan;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Holds functionality for mouse input to the LM.
    /// </summary>
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Private Fields

        /// <summary>
        /// Current 3D cursor position.
        /// </summary>
        Vector3 cursorPosition = Vector3.zero;

        /// <summary>
        /// Is the mouse currently over UI?
        /// This must be managed externally, as LM does not know what
        /// UI elements exist.
        /// </summary>
        bool mouseOverUI = false;

        /// <summary>
        /// World position at mouse click.
        /// </summary>
        Vector3 mouseDownWorldPos;

        /// <summary>
        /// Screen position at mouse click.
        /// </summary>
        Vector2 mouseDownScreenPos;

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns the position of the 3D cursor (read-only).
        /// </summary>
        public Vector3 CursorPosition
        {
            get { return cursorPosition; }
            set { cursorPosition = value; }
        }

        /// <summary>
        /// Gets/sets whether or not the mouse is currently over UI.
        /// </summary>
        public bool MouseOverUI
        {
            get { return mouseOverUI; }
            set { mouseOverUI = value; }
        }

        /// <summary>
        /// Gets/sets the screen position of the mouse on click.
        /// </summary>
        public Vector2 MouseDownScreenPos
        {
            get { return mouseDownScreenPos; }
            set { mouseDownScreenPos = value; }
        }

        /// <summary>
        /// Gets/sets the world position of the mouse on click.
        /// </summary>
        public Vector3 MouseDownWorldPos
        {
            get { return mouseDownWorldPos; }
            set { mouseDownWorldPos = value; }
        }

        /// <summary>
        /// Handles a left mouse button click.
        /// </summary>
        public void HandleLeftDown(Event e)
        {
            mouseDownScreenPos = e.mousePosition;
            var ray = editor.PointerRay;
            float dist;
            if (Instance.EditingPlane.Raycast(ray, out dist))
            {
                mouseDownWorldPos = ray.GetPoint(dist);
            }
        }

        /// <summary>
        /// Handles a left mouse button up event.
        /// </summary>
        public void HandleLeftUp(Event e, bool isDragging, Camera camera)
        {
            switch (currentTool)
            {
                case Tool.Select:
                    if (LevelLoaded)
                    {
                        SelectObjects(hoveredObjects);
                    } else DeselectObjects();
                    break;

                case Tool.Place:
                    if (HasSelectedObjectForPlacement &&
                        CanPlaceAnotherCurrentObject())
                    {
                        //if (_snapToGrid) SnapCursor();
                        CreateCurrentSelectedObjectAtCursor();
                        e.Use();
                    }
                    break;

                case Tool.Erase:
                    if (hoveredObjects.Count > 0)
                        DeleteObject(hoveredObjects[0],
                            LevelEditorAction.ActionType.Normal);
                    break;

                case Tool.Move:
                    // Not currently moving
                    if (!IsMovingObject)
                    {
                        if (hoveredObjects.Count > 0)
                            StartMovingObject(hoveredObjects[0]);
                    }

                    // Currently moving
                    else
                    {
                        StopMovingObject();
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles a mouse movement event.
        /// </summary>
        public void HandleMouseMove(Event e, bool isDragging)
        {
            var ray = editor.PointerRay;
            float distance;
            if (editingPlane.Raycast(ray, out distance))
                SetCursorPosition(ray.GetPoint(distance));

            if (currentTool != Tool.Place)
            {
                if (isDragging)
                {
                    hoveredObjects = ObjectsInSelection();
                } else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        ObjectSpawner spawner = 
                            hit.collider.gameObject.
                            GetComponentInAncestors<ObjectSpawner>();
                        if (spawner != null)
                            HoverObject(spawner.gameObject);
                    } else hoveredObjects.Clear();
                }

            }
        }

        /// <summary>
        /// Sets the cursor position, snapping if necessary.
        /// </summary>
        public void SetCursorPosition(Vector3 newPos)
        {
            cursorPosition = newPos;
            if (snapToGrid) SnapCursor();
            if (assetPreview != null)
            {
                assetPreview.transform.position = cursorPosition;
                assetPreview.transform.rotation =
                    Quaternion.Euler(0f, currentPlacementYRotation, 0f);
            }
        }

        #endregion
    }
}