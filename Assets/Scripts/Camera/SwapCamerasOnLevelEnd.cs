using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCamerasOnLevelEnd : MonoBehaviour {

    [SerializeField]
    private Transform endCamera;

	// Use this for initialization
	void Start () {
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, SwitchCameras); 
	}

    private void OnDestroy()
    {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, SwitchCameras);
        }
    }



    void SwitchCameras(params object[] parameters)
    {
        endCamera.gameObject.SetActive(true);
    }
}
