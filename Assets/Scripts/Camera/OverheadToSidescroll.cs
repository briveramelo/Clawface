// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadToSidescroll : MonoBehaviour {

    #region Serialized
    
    [SerializeField] private BoxCollider sidescrollArea;
    [SerializeField] private BoxCollider overheadArea;
    [SerializeField] private Transform overheadCameraPosition;
    [SerializeField] private Transform sideScrollCameraPosition;
    [SerializeField] private Transform sideScrollLockToAxis;
    [SerializeField] private float timeToTween;
    [SerializeField] private bool isTweening;
    #endregion


    #region Private
    private MoveState joystickMovement;
    private CameraLock cameraLock;

    private float tweenTimer;
    private bool startTweenAgain;
    #endregion

    private void Awake()
    {
        joystickMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<MoveState>();
        cameraLock = Camera.main.gameObject.GetComponent<CameraLock>();
    }

    // Update is called once per frame
    void Update () {
        tweenTimer += Time.deltaTime;

        if (LeanTween.isTweening(Camera.main.gameObject) && tweenTimer < timeToTween)
        {
            LeanTween.cancel(Camera.main.gameObject);
            LeanTween.move(Camera.main.gameObject, joystickMovement.transform.position - cameraLock.GetDistance(), timeToTween - tweenTimer).setOnComplete(LockCamera);
            LeanTween.rotate(Camera.main.gameObject, cameraLock.GetAngle(), timeToTween - tweenTimer);
        }
    }

    public void SidescrollTriggerEnter()
    {
        if (CameraLock.cameraMode != CameraMode.SIDESCROLL)
        {
            if (sideScrollLockToAxis != null)
            {
                joystickMovement.transform.position = new Vector3(sideScrollLockToAxis.position.x, joystickMovement.transform.position.y, sideScrollLockToAxis.position.z);
            }
 
            LeanTween.move(Camera.main.gameObject, sideScrollCameraPosition.position, timeToTween).setOnComplete(LockCamera);
            LeanTween.rotate(Camera.main.gameObject, sideScrollCameraPosition.eulerAngles, timeToTween);
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.SetSidescrolling(true);
            cameraLock.SetDistance(sidescrollArea.transform.position - sideScrollCameraPosition.position);
            cameraLock.SetAngle(sideScrollCameraPosition.eulerAngles);
            CameraLock.cameraMode = CameraMode.SIDESCROLL;

        }
    }

    public void SidescrollTriggerStay()
    {

    }

    public void SidescrollTriggerExit()
    {

    }

    public void OverheadTriggerEnter()
    {
        if (CameraLock.cameraMode != CameraMode.OVERHEAD)
        {
            LeanTween.move(Camera.main.gameObject, overheadCameraPosition.position, timeToTween).setOnComplete(LockCamera);
            LeanTween.rotate(Camera.main.gameObject, overheadCameraPosition.eulerAngles, timeToTween);
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.SetSidescrolling(false);
            cameraLock.SetDistance(joystickMovement.transform.position - overheadCameraPosition.position);
            cameraLock.SetAngle(overheadCameraPosition.eulerAngles);
            CameraLock.cameraMode = CameraMode.OVERHEAD;

        }
    }

    public void OverheadTriggerStay()
    {

    }

    public void OverheadTriggerExit()
    {

    }

    public void LockCamera()
    {
        cameraLock.LockCamera();
        isTweening = false;
    }

    public void UnlockCamera()
    {
        cameraLock.UnlockCamera();
    }

}
