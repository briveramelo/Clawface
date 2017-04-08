// AssetPreview.cs

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class for the 3D asset preview.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AssetPreview : SingletonMonoBehaviour<AssetPreview> {

    #region Vars

    /// <summary>
    /// The child preview models.
    /// </summary>
    [SerializeField]
    List<GameObject> _children = new List<GameObject>();

    int _numChildren = 0;

    #endregion
    #region Unity Callbacks

    new void Awake() {

        // Destroy old instances
        if (Instance != null)
            LevelManager.Instance.DestroyLoadedObject (Instance.gameObject);

        base.Awake();
    }

    #endregion
    #region Methods

    /// <summary>
    /// Shows the asset preview.
    /// </summary>
    public void Show () {
        for (int i = 0; i < _children.Count; i++)
            _children[i].GetComponent<MeshRenderer>().enabled = i < _numChildren;
    }

    /// <summary>
    /// Hides the asset preview.
    /// </summary>
    public void Hide () {
        foreach (var child in _children)
            child.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Changes the asset preview to match an object.
    /// </summary>
	public void SetPreviewObject (GameObject template) {
        if (template == null) throw new System.NullReferenceException ("Null template!");
        MeshFilter[] templateChildren = template.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < templateChildren.Length; i++) {

            // Add previews if necessary
            if ((i+1) >= _children.Count) {
                var previewChild = new GameObject (("Preview" + i), typeof (MeshFilter), typeof(MeshRenderer));
                previewChild.transform.SetParent (transform);
                previewChild.GetComponent<MeshRenderer>().sharedMaterial = LevelManager.Instance.PreviewMaterial;
                _children.Add (previewChild);
            }

            // Copy meshes/transforms of children
            _children[i].GetComponent<MeshFilter>().sharedMesh = templateChildren[i].sharedMesh;
            _children[i].transform.localPosition = templateChildren[i].transform.localPosition;
            _children[i].transform.localRotation = templateChildren[i].transform.localRotation;
            _children[i].transform.localScale = templateChildren[i].transform.localScale;
        }

        _numChildren = templateChildren.Length;
    }

    #endregion
}
