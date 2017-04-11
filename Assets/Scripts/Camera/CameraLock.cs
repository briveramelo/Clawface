// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    OVERHEAD = 0,
    SIDESCROLL = 1,
    THIRDPERSON = 2
}

public class CameraLock : MonoBehaviour {

    public static CameraMode cameraMode;

    [SerializeField] private Transform objectToLockTo;
    [SerializeField] private Vector3 distance;
    [SerializeField] private Vector3 angle;
    [SerializeField] private bool isLocked;
    [SerializeField]
    private float smoothTime;

    // Use this for initialization
    void Start () {
		if (distance == Vector3.zero && angle == Vector3.zero)
        {
            cameraMode = CameraMode.OVERHEAD;
            distance = objectToLockTo.position - transform.position;
            angle = transform.rotation.eulerAngles;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isLocked && objectToLockTo != null)
        {
            Vector3 maxSpeed = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, objectToLockTo.position - distance, ref maxSpeed, smoothTime);
            transform.rotation = Quaternion.Euler(angle);
        }
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
