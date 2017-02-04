using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTransitionTrigger : MonoBehaviour {

    [SerializeField]
    string layerName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Strings.PLAYER)
        {
            CameraLayerController.instance.SwitchLayer(1 << LayerMask.NameToLayer(layerName));
        }
    }
}
