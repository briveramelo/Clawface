// ObjectSpawner.cs

using System.Collections.Generic;
using UnityEngine;
using ModMan;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Class to spawn an object on level start.
/// </summary>
public class ObjectSpawner : MonoBehaviour {

    #region Vars

    /// <summary>
    /// Prefab to spawn.
    /// </summary>
    [SerializeField]
    GameObject _template;

    /// <summary>
    /// Preview children.
    /// </summary>
    [SerializeField]
    List<GameObject> _children = new List<GameObject>();

    /// <summary>
    /// Spawned instance of template.
    /// </summary>
    GameObject _instance;

    #endregion
    #region Properties

    /// <summary>
    /// Returns the template used in this spawner (read-only).
    /// </summary>
    public GameObject Template { get { return _template; } }

    #endregion
    #region Methods

    /// <summary>
    /// Sets the template of this spawner.
    /// </summary>
    public void SetTemplate (GameObject template) {
        if (template == null)
            throw new System.NullReferenceException ("Null template given to spawner!");

        _template = template;
        //transform.localScale = template.transform.localScale;

        // Show preview
        var meshFilters = _template.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++) {
            

            GameObject child;

            // Add new children if necessary
            if ((i+1) >= _children.Count) {
                child = new GameObject ("Preview" + i, typeof (MeshFilter), typeof (MeshRenderer));
                child.GetComponent<MeshRenderer>().sharedMaterial = LevelManager.Instance.PreviewMaterial;
                child.CopyColliderFromGameObject (meshFilters[i].gameObject);
                child.transform.SetParent (transform);
                _children.Add(child);
            }
            else child = _children[i];

            // Copy mesh/transform of children
            child.transform.localPosition = meshFilters[i].transform.localPosition;
            child.transform.localRotation = meshFilters[i].transform.localRotation;
            child.transform.localScale = meshFilters[i].transform.localScale;
            child.GetComponent<MeshFilter>().sharedMesh = meshFilters[i].sharedMesh;
        }
    }

    /// <summary>
    /// Spawns an instance of the template and hides the spawner.
    /// </summary>
    public void Play() {
        _instance = SpawnObject();

        if (_instance == null) {
            Debug.LogError ("Failed to spawn object!", gameObject);
            return;
        }

        _instance.transform.SetParent (LevelManager.Instance.transform);
        _instance.transform.position = _children[0].transform.position;
        _instance.transform.localRotation = _children[0].transform.localRotation;
        _instance.transform.localScale = _children[0].transform.localScale;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Deletes the instance and reenables the spawner.
    /// </summary>
    public void ResetSpawner() {
        DeleteObject (_instance);
        gameObject.SetActive(true);
    } 

    /// <summary>
    /// Instanctiates the template.
    /// </summary>
    public GameObject SpawnObject () {
        if (_template == null)
            throw new System.NullReferenceException ("No prefab defined!");

        #if UNITY_EDITOR
        if (Application.isEditor) return (GameObject)PrefabUtility.InstantiatePrefab (_template);
        else
        #endif
            return Instantiate (_template);
    }

    /// <summary>
    /// Deletes an object.
    /// </summary>
    public void DeleteObject (GameObject toDelete) {
        if (Application.isEditor) DestroyImmediate (toDelete);
        else Destroy (toDelete);
    }

    #endregion
}
