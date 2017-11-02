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
    SmoothDamping=0,
    Shaking=1
}

public class CameraLock : MonoBehaviour {

    public static CameraMode cameraMode;

    [SerializeField] private Transform objectToLockTo;
    [SerializeField] private Vector3 distance;
    [SerializeField] private Vector3 angle;
    [SerializeField] private bool isLocked;
    [SerializeField] private float smoothTime;
    [SerializeField] private float shakeFactor = 0.25f;
    [SerializeField] private float shakeTime = 0.4f;

    private CameraState camState=CameraState.SmoothDamping;
    // Use this for initialization
    void Start () {
		if (distance == Vector3.zero && angle == Vector3.zero){
            cameraMode = CameraMode.OVERHEAD;
            distance = objectToLockTo.position - transform.position;
            angle = transform.rotation.eulerAngles;
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (isLocked && objectToLockTo != null){
            Vector3 maxSpeed = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, objectToLockTo.position - distance, ref maxSpeed, smoothTime);
            transform.rotation = Quaternion.Euler(angle);
        }
	}

    public void Shake() {
        string shakeString = "CameraLock";
        Timing.KillCoroutines(shakeString);
        Timing.RunCoroutine(ShakeCam(shakeTime,shakeFactor), Segment.LateUpdate, shakeString);
    }

    private IEnumerator<float> ShakeCam(float shakeTime, float maxShakeMagnitude) {
        camState=CameraState.Shaking;
        float timeRemaining=shakeTime;
        float shakeMagnitude=maxShakeMagnitude;
        int camSide=1;
        while (timeRemaining>0f) {
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, 0.05f);
            Vector3 shakeDirection = Random.onUnitSphere;
            shakeDirection-=Vector3.Project(shakeDirection, transform.forward);
            shakeDirection.x=Mathf.Abs(shakeDirection.x) * camSide;
            shakeDirection.Normalize();
            shakeDirection *= shakeMagnitude;
            transform.position+=shakeDirection;
            camSide*=-1;
            timeRemaining -=Time.deltaTime;
            yield return 0f;
        }
        camState=CameraState.SmoothDamping;
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
