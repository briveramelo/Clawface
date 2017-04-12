// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public enum CameraMode
{
    OVERHEAD = 0,
    SIDESCROLL = 1,
    THIRDPERSON = 2
}
public enum CameraState {
    SmoothDamp=0,
    Lerp=1
}

public class CameraLock : MonoBehaviour {

    public static CameraMode cameraMode;

    [SerializeField] private Transform objectToLockTo;
    [SerializeField] private Vector3 distance;
    [SerializeField] private Vector3 angle;
    [SerializeField] private bool isLocked;
    [SerializeField] private float smoothTime;
    private CameraState camState;
    // Use this for initialization
    void Start () {
		if (distance == Vector3.zero && angle == Vector3.zero)
        {
            cameraMode = CameraMode.OVERHEAD;
            distance = objectToLockTo.position - transform.position;
            angle = transform.rotation.eulerAngles;
        }
        camState = CameraState.SmoothDamp;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isLocked && objectToLockTo != null)
        {
            Vector3 maxSpeed = Vector3.zero;
            if (camState==CameraState.SmoothDamp) {
                transform.position = Vector3.SmoothDamp(transform.position, objectToLockTo.position - distance, ref maxSpeed, smoothTime);
            }
            else if(camState==CameraState.Lerp) {
                transform.position = Vector3.Lerp(transform.position, objectToLockTo.position - distance, .1f);
            }
            transform.rotation = Quaternion.Euler(angle);
        }
	}

    public void Shake(float shakeTime) {
        string shakeString = "CameraLock";
        Timing.KillCoroutines(shakeString);
        Timing.RunCoroutine(ShakeCam(shakeTime), Segment.LateUpdate, shakeString);
    }

    private IEnumerator<float> ShakeCam(float shakeTime) {
        camState=CameraState.Lerp;
        float timeRemaining=shakeTime;
        float shakeMaxMagnitude=.1f;        
        float shakeMagnitude=shakeMaxMagnitude;
        while (timeRemaining>0f) {
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, 0.05f);
            Vector3 shakeDirection = Random.onUnitSphere;
            shakeDirection-=Vector3.Project(shakeDirection, transform.forward);
            shakeDirection.Normalize();
            shakeDirection *= shakeMagnitude;
            transform.position+=shakeDirection;
            timeRemaining -=Time.deltaTime;
            yield return 0f;
        }
        camState=CameraState.SmoothDamp;
    }

    public void LockCamera()
    {
        isLocked = true;
    }

    public void UnlockCamera()
    {
        isLocked = false;
    }

    public void SetDistance(Vector3 _distance)
    {
        distance = _distance;
    }

    public void SetAngle(Vector3 _eulerAngles)
    {
        angle = _eulerAngles;
    }

    public void SetAngle(Quaternion _quaternionAngles)
    {
        angle = _quaternionAngles.eulerAngles;
    }

    public Vector3 GetDistance()
    {
        return distance;
    }

    public Vector3 GetAngle()
    {
        return angle;
    }
}
