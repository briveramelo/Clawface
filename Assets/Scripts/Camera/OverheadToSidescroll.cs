// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadToSidescroll : MonoBehaviour {

    [SerializeField]
    private BoxCollider sidescrollArea;
    [SerializeField]
    private BoxCollider overheadArea;
    [SerializeField]
    private Transform overheadCameraPosition;
    [SerializeField]
    private Transform sideScrollCameraPosition;
    [SerializeField]
    private Transform sideScrollLockToAxis;

    [SerializeField] private float timeToTween;

    private PlayerMovement joystickMovement;
    private CameraLock cameraLock;

    private float tweenTimer;

    private void Awake()
    {
        joystickMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        cameraLock = Camera.main.gameObject.GetComponent<CameraLock>();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        tweenTimer += Time.deltaTime;

        iTween.Stop(Camera.main.gameObject);
        iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "tweenMoveTo", "time", timeToTween - tweenTimer, "position", joystickMovement.transform.position - cameraLock.GetDistance(), "oncomplete", "LockCamera"));
        iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "tweenRotateTo", "time", timeToTween - tweenTimer, "rotation", cameraLock.GetAngle()));

        /*
        if (LeanTween.isTweening(Camera.main.gameObject) && tweenTimer < timeToTween)
        {
            LeanTween.cancel(Camera.main.gameObject);
            LeanTween.move(Camera.main.gameObject, joystickMovement.transform.position - cameraLock.distance, timeToTween - tweenTimer).setOnComplete(LockCamera);
            LeanTween.rotate(Camera.main.gameObject, cameraLock.angle, timeToTween - tweenTimer);
        }
        */
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (CameraLock.cameraMode != CameraMode.SIDESCROLL)
        {
            if (sideScrollLockToAxis != null)
            {
                joystickMovement.transform.position = new Vector3(sideScrollLockToAxis.position.x, joystickMovement.transform.position.y, sideScrollLockToAxis.position.z);
            }
            iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "sidescrollMoveTo", "time", timeToTween, "position", sideScrollCameraPosition.position, "oncomplete", "LockCamera"));
            iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "sidescrollRotateTo", "time", timeToTween, "rotation", sideScrollCameraPosition.eulerAngles));
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.SetSidescrolling(true);
            cameraLock.SetDistance(sidescrollArea.transform.position - sideScrollCameraPosition.position);
            cameraLock.SetAngle(sideScrollCameraPosition.eulerAngles);
            CameraLock.cameraMode = CameraMode.SIDESCROLL;
        }
        else if (CameraLock.cameraMode != CameraMode.OVERHEAD)
        {
            iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "overheadMoveTo", "time", timeToTween, "position", overheadCameraPosition.position, "oncomplete", "LockCamera"));
            iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "overheadRotateTo", "time", timeToTween, "rotation", overheadCameraPosition.eulerAngles));
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.SetSidescrolling(false);
            cameraLock.SetDistance(joystickMovement.transform.position - overheadCameraPosition.position);
            cameraLock.SetAngle(overheadCameraPosition.eulerAngles);
            CameraLock.cameraMode = CameraMode.OVERHEAD;
        }
    }
    */


    public void SidescrollTriggerEnter()
    {
        if (CameraLock.cameraMode != CameraMode.SIDESCROLL)
        {
            if (sideScrollLockToAxis != null)
            {
                joystickMovement.transform.position = new Vector3(sideScrollLockToAxis.position.x, joystickMovement.transform.position.y, sideScrollLockToAxis.position.z);
            }
            cameraLock.UnlockCamera();

            iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "sidescrollMoveTo", "time", timeToTween, "position", sideScrollCameraPosition.position, "oncomplete", "LockCamera"));
            iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "sidescrollRotateTo", "time", timeToTween, "rotation", sideScrollCameraPosition.eulerAngles));
            tweenTimer = 0f;
            
            joystickMovement.SetSidescrolling(true);
            cameraLock.SetDistance(sidescrollArea.transform.position - sideScrollCameraPosition.position);
            cameraLock.SetAngle(sideScrollCameraPosition.eulerAngles);
            CameraLock.cameraMode = CameraMode.SIDESCROLL;


            /*
            joystickMovement.transform.position = new Vector3(sideScrollLockToAxis.position.x, joystickMovement.transform.position.y, sideScrollLockToAxis.position.z);
            LeanTween.move(Camera.main.gameObject, sideScrollCameraPosition.position, timeToTween).setOnComplete(LockCamera);
            LeanTween.rotate(Camera.main.gameObject, sideScrollCameraPosition.eulerAngles, timeToTween);
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.isSidescrolling = true;
            cameraLock.distance = sidescrollArea.transform.position - sideScrollCameraPosition.position;
            cameraLock.angle = sideScrollCameraPosition.eulerAngles;
            CameraLock.cameraMode = CameraMode.SIDESCROLL;
            */
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
            iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "overheadMoveTo", "time", timeToTween, "position", overheadCameraPosition.position, "oncomplete", "LockCamera"));
            iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "overheadRotateTo", "time", timeToTween, "rotation", overheadCameraPosition.eulerAngles));
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.SetSidescrolling(false);
            cameraLock.SetDistance(joystickMovement.transform.position - overheadCameraPosition.position);
            cameraLock.SetAngle(overheadCameraPosition.eulerAngles);
            CameraLock.cameraMode = CameraMode.OVERHEAD;

            /*
            LeanTween.move(Camera.main.gameObject, overheadCameraPosition.position, timeToTween).setOnComplete(LockCamera);
            LeanTween.rotate(Camera.main.gameObject, overheadCameraPosition.eulerAngles, timeToTween);
            tweenTimer = 0f;
            cameraLock.UnlockCamera();
            joystickMovement.isSidescrolling = false;
            cameraLock.distance = joystickMovement.transform.position - overheadCameraPosition.position;
            cameraLock.angle = overheadCameraPosition.eulerAngles;
            CameraLock.cameraMode = CameraMode.OVERHEAD;
            */
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
    }

    public void UnlockCamera()
    {
        cameraLock.UnlockCamera();
    }

}
