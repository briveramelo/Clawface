using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayerController : MonoBehaviour {

    Camera mainCamera;
    LayerMask currentLayer;
    public static CameraLayerController instance;

    [SerializeField]
    string startingLayer;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

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
        if (currentLayer != mask)
        {
            mainCamera.cullingMask = mainCamera.cullingMask & ~currentLayer | mask;
            currentLayer = mask;
        }
    }
}