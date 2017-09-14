// LevelManagerSelection.cs
// Author: Aaron

using ModMan;

using System.Collections.Generic;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Holds functionality for LM object selection.
    /// </summary>
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Private Fields

        /// <summary>
        /// Currently hovered objects in editor.
        /// </summary>
        List<GameObject> hoveredObjects = new List<GameObject>();

        /// <summary>
        /// Currently selected object in editor;
        /// </summary>
        List<GameObject> selectedObjects = new List<GameObject>();

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns the list of objects that are currently hovered over by the
        /// pointer or selection rect (read-only).
        /// </summary>
        public List<GameObject> HoveredObjects { get { return hoveredObjects; } }

        /// <summary>
        /// Returns the objects that are currently selected (read-only).
        /// </summary>
        public List<GameObject> SelectedObjects { get { return selectedObjects; } }

        /// <summary>
        /// Returns true if an object is selected (read-only).
        /// </summary>
        public bool HasSelectedObjects { get { return selectedObjects.Count > 0; } }

        /// <summary>
        /// Sets an object (and only that object) to be hovered.
        /// </summary>
        public void HoverObject(GameObject obj)
        {
            hoveredObjects.Clear();
            hoveredObjects.Add(obj);
        }

        /// <summary>
        /// Returns a list of all GameObjects in the current selection.
        /// </summary>
        public List<GameObject> ObjectsInSelection()
        {
            Camera camera = editor.ActiveCamera;
            Rect selectionRect = editor.SelectionRect;
            List<GameObject> result = new List<GameObject>();
            foreach (var spawner in loadedSpawners)
            {
                Vector3 rawScreenPos =
                    camera.WorldToScreenPoint(spawner.transform.position);

                Vector2 screenPos = 
                    new Vector2(rawScreenPos.x, rawScreenPos.y);

                if (selectionRect.Contains(screenPos, true))
                    result.Add(spawner.gameObject);
            }
            return result;
        }


        /// <summary>
        /// Selects an object in the level.
        /// </summary>
        public void SelectObjects(List<GameObject> objects, bool clearFirst = true)
        {
            if (clearFirst) selectedObjects.Clear();
            foreach (var obj in objects)
            {
                // If selected object is not spawner, find through parents
                var spawner = obj.GetComponent<ObjectSpawner>();
                if (spawner == null) spawner = obj.GetComponentInAncestors<ObjectSpawner>();

                if (spawner != null)
                {
                    if (!selectedObjects.Contains(spawner.gameObject))
                    {
                        selectedObjects.Add(spawner.gameObject);
                        onSelectObject.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Deselects an object.
        /// </summary>
        public void DeselectObjects()
        {
            if (selectedObjects != null)
            {
                onDeselectObject.Invoke();
                selectedObjects.Clear();
            }
        }

        #endregion
    }
}