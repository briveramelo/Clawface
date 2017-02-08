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

    [SerializeField]
    private bool isTweening;

    private float tweenTimer;

    private bool startTweenAgain;

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

        //if (isTweening)
        //{
        //    if (startTweenAgain)
        //    {
        //        iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "tweenMoveTo", "time", timeToTween - tweenTimer, "position", joystickMovement.transform.position - cameraLock.GetDistance(), "oncomplete", "LockCamera"));
        //        iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "tweenRotateTo", "time", timeToTween - tweenTimer, "rotation", cameraLock.GetAngle()));
        //        startTweenAgain = false;
        //    }
        //    else
        //    {
        //        iTween.Stop(Camera.main.gameObject);
        //        startTweenAgain = true;
        //    }
        //}

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

            //cameraLock.UnlockCamera();

            //iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "sidescrollMoveTo", "time", timeToTween, "position", sideScrollCameraPosition.position, "oncomplete", "LockCamera"));
            //iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "sidescrollRotateTo", "time", timeToTween, "rotation", sideScrollCameraPosition.eulerAngles));
            //tweenTimer = 0f;

            //joystickMovement.SetSidescrolling(true);
            //cameraLock.SetDistance(sidescrollArea.transform.position - sideScrollCameraPosition.position);
            //cameraLock.SetAngle(sideScrollCameraPosition.eulerAngles);
            //isTweening = true;
            //CameraLock.cameraMode = CameraMode.SIDESCROLL;



            
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
            //iTween.MoveTo(Camera.main.gameObject, iTween.Hash("name", "overheadMoveTo", "time", timeToTween, "position", overheadCameraPosition.position, "oncomplete", "LockCamera"));
            //iTween.RotateTo(Camera.main.gameObject, iTween.Hash("name", "overheadRotateTo", "time", timeToTween, "rotation", overheadCameraPosition.eulerAngles));
            //tweenTimer = 0f;
            //cameraLock.UnlockCamera();
            //joystickMovement.SetSidescrolling(false);
            //cameraLock.SetDistance(joystickMovement.transform.position - overheadCameraPosition.position);
            //cameraLock.SetAngle(overheadCameraPosition.eulerAngles);
            //isTweening = true;
            //CameraLock.cameraMode = CameraMode.OVERHEAD;


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
