using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ObjectSpawner : MonoBehaviour {

    GameObject _template;

    GameObject _instance;

    public GameObject Template { get { return _template; } }

    public void SetTemplate (GameObject template) {
        _template = template;
        GetComponent<MeshFilter>().sharedMesh = _template.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshRenderer>().sharedMaterial = LevelManager.Instance.PreviewMaterial;
    }

    public void Play() {
        _instance = SpawnObject();
        _instance.transform.position = transform.position;
        _instance.transform.localRotation = transform.localRotation;
        _instance.transform.localScale = transform.localScale;
        gameObject.SetActive(false);
    }

    public void Reset() {
        DeleteObject (_instance);
        gameObject.SetActive(true);
    } 

    public GameObject SpawnObject () {
        if (Application.isEditor) return (GameObject)PrefabUtility.InstantiatePrefab (_template);
        else return Instantiate (_template);
    }

    public void DeleteObject (GameObject toDelete) {
        if (Application.isEditor) DestroyImmediate (toDelete);
        else Destroy (toDelete);
    }
}
