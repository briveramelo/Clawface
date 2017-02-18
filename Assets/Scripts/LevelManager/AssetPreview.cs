// AssetPreview.cs

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AssetPreview : SingletonMonoBehaviour<AssetPreview> {

    List<GameObject> _children = new List<GameObject>();

    new void Awake() {
        if (Instance != null)
            LevelManager.Instance.DestroyLoadedObject (Instance.gameObject);

        base.Awake();
    } 

    public void Show () {
        foreach (var child in _children)
            child.GetComponent<MeshRenderer>().enabled = true;
    }

    public void Hide () {
        foreach (var child in _children)
            child.GetComponent<MeshRenderer>().enabled = false;
    }

	public void SetPreviewObject (GameObject template) {
        MeshFilter[] templateChildren = template.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < templateChildren.Length; i++) {
            if ((i+1) >= _children.Count) {
                var previewChild = new GameObject (("Preview" + i), typeof (MeshFilter), typeof(MeshRenderer));
                previewChild.GetComponent<MeshRenderer>().sharedMaterial = LevelManager.Instance.PreviewMaterial;
                _children.Add (previewChild);
            }

            _children[i].GetComponent<MeshFilter>().sharedMesh = templateChildren[i].sharedMesh;
        }
    } 
}
