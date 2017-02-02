using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayerTests : MonoBehaviour {

    Camera mainCamera;
    LayerMask currentLayer;

    [SerializeField]
    string startingLayer;

	// Use this for initialization
	void Start () {
        mainCamera = GetComponent<Camera>();
        currentLayer = 1 << LayerMask.NameToLayer(startingLayer);
    }
	
	// Update is called once per frame
	void Update () {        
    }

    public void SwitchLayer(LayerMask mask)
    {
        mainCamera.cullingMask = mainCamera.cullingMask & ~currentLayer | mask;
        currentLayer = mask;
    }
}