using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCamerasOnLevelEnd : MonoBehaviour {

    [SerializeField]
    private Transform endCamera;

    [SerializeField]
    private Transform keiraMesh;

    [SerializeField]
    private MoveState moveState;

    [SerializeField]
    private Rigidbody playerRigidbody;

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
        keiraMesh.LookAt(endCamera);
        moveState.enabled = false;
        playerRigidbody.velocity = Vector3.zero;
        // keiraMesh.localRotation = Quaternion.Euler(0f, keiraMesh.transform.rotation.y, 0f);
    }
}
