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

    public Transform objectToLockTo;
    public Vector3 distance;
    public Vector3 angle;

    public bool isLocked;

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
	void Update () {
        if (isLocked)
        {
            transform.position = objectToLockTo.position - distance;
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
}
