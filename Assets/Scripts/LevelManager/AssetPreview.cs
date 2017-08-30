// AssetPreview.cs
// Author: Aaron

using System.Collections.Generic;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Class for the 3D asset preview.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AssetPreview : EditorSingleton<AssetPreview>
    {
        #region Vars

        /// <summary>
        /// The child preview models.
        /// </summary>
        List<GameObject> children = new List<GameObject>();

        // Number of child preview models
        int numChildren = 0;

        #endregion
        #region Unity Callbacks

        new void Awake() 
        {
            // Destroy old instances
            if (Instance != null)
                LevelManager.Instance.DestroyLoadedObject(Instance.gameObject);

            base.Awake();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Shows the asset preview.
        /// </summary>
        public void Show() 
        {
            for (int i = 0; i < children.Count; i++)
                children[i].GetComponent<MeshRenderer>().enabled = 
                    i < numChildren;
        }

        /// <summary>
        /// Hides the asset preview.
        /// </summary>
        public void Hide() 
        {
            foreach (var child in children)
                child.GetComponent<MeshRenderer>().enabled = false;
        }

        /// <summary>
        /// Changes the asset preview to match an object.
        /// </summary>
        public void SetPreviewObject(GameObject template) 
        {
            if (template == null)
                throw new System.NullReferenceException("Null template!");

            MeshFilter[] templateChildren = 
                template.GetComponentsInChildren<MeshFilter>();

            for (int i = 0; i < templateChildren.Length; i++) 
            {
                // Add previews if necessary
                if ((i + 1) >= children.Count) 
                {
                    var previewChild = new GameObject(("Preview" + i), 
                        typeof(MeshFilter), typeof(MeshRenderer));

                    previewChild.transform.SetParent(transform);

                    var materials = new Material[8];
                    for (int m = 0; m < 8; m++) 
                    {
                       materials[m] = LevelManager.Instance.PreviewMaterial;
                    }

                    previewChild.GetComponent<MeshRenderer>().
                        sharedMaterials = materials;
                    
                    children.Add(previewChild);
                }

                // Copy meshes/transforms of children
                children[i].GetComponent<MeshFilter>().sharedMesh =
                    templateChildren[i].sharedMesh;

                children[i].transform.localPosition = 
                    templateChildren[i].transform.localPosition;

                children[i].transform.localRotation = 
                    templateChildren[i].transform.localRotation;

                children[i].transform.localScale = 
                    templateChildren[i].transform.localScale;
            }

            numChildren = templateChildren.Length;
        }

        #endregion
    }
}