using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

public class SwapCamerasOnLevelEnd : MonoBehaviour {

    #region Serialized
    [SerializeField]
    private Transform endCamera;

    [SerializeField]
    private Transform keiraMesh;

    [SerializeField]
    private MoveState moveState;

    [SerializeField]
    private Rigidbody playerRigidbody;

    [SerializeField]
    private PlayerFaceController playerFace;
    #endregion

    #region Unity Lifetime
    // Use this for initialization
    void Start () {
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, SwitchCameras); 
	}

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCameras();
        }
        */
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, SwitchCameras);
        }
    }
    #endregion


    #region Private Methods
    void SwitchCameras(params object[] parameters)
    {
        endCamera.gameObject.SetActive(true);
        keiraMesh.LookAt(endCamera);
        moveState.enabled = false;
        playerRigidbody.velocity = Vector3.zero;
        playerFace.SetEmotion(PlayerFaceController.Emotion.Happy);
        // keiraMesh.localRotation = Quaternion.Euler(0f, keiraMesh.transform.rotation.y, 0f);
    }
    #endregion
}
