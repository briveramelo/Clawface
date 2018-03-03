// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModCinematicCamera : MonoBehaviour {

    #region Serialized
    [SerializeField]
    private Transform cameraPosition1;

    [SerializeField]
    private Transform cameraPosition2;

    [SerializeField]
    private float timeToMoveBackToOriginalPosition;
    #endregion

    #region Privates
    private Vector3 savedCameraPosition;
    private Vector3 savedCameraRotation;
    private Transform player;
    private Vector3 savedCameraDistance;

    private CameraLock cameraLock;

    private bool lookAtPlayer;
    private bool canTweenAgain;

    private float tweenTimer;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        cameraLock = Camera.main.gameObject.GetComponent<CameraLock>();
        player = transform;
    }

    // Use this for initialization
	void Start () {
        canTweenAgain = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (lookAtPlayer)
        {
            cameraLock.transform.LookAt(player);
            LeanTween.cancel(cameraLock.gameObject);
            tweenTimer -= Time.deltaTime;
            MoveToSavedPosition();
        }

        
	}
    #endregion

    #region Public Methods
    public void StartTween()
    {
        savedCameraDistance = player.position - cameraLock.transform.position;

        LeanTween.cancel(cameraLock.gameObject);
        canTweenAgain = false;
        savedCameraPosition = cameraLock.transform.position;
        savedCameraRotation = cameraLock.transform.rotation.eulerAngles;
        cameraLock.UnlockCamera();
        cameraLock.transform.position = cameraPosition1.position;
        cameraLock.transform.rotation = cameraPosition1.rotation;

        

        LeanTween.move(cameraLock.gameObject, cameraPosition2, 1f).setEaseInOutQuad().setOnComplete(MoveToSavedPosition);
        LeanTween.rotate(cameraLock.gameObject, cameraPosition2.rotation.eulerAngles, 1f).setOnComplete(MoveToSavedPosition).setEaseInOutQuad();

        tweenTimer = timeToMoveBackToOriginalPosition;

    }
    #endregion

    #region Private Methods
    private void MoveToSavedPosition()
    {
        lookAtPlayer = true;
        LeanTween.move(cameraLock.gameObject, player.position - savedCameraDistance, tweenTimer).setOnComplete(EndTween);
        // LeanTween.rotate(cameraLock.gameObject, savedCameraRotation, 1f).setOnComplete(cameraLock.LockCamera).setEaseInOutQuad();
    }

    private void EndTween()
    {
        lookAtPlayer = false;
        // cameraLock.SetDistance(player.position - cameraLock.transform.position);
        cameraLock.SetAngle(cameraLock.transform.rotation);
        cameraLock.LockCamera();
    }

    #endregion

}
