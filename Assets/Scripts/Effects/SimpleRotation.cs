using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] RotationAxis axis = RotationAxis.WorldY;


    [SerializeField] float rotationSpeed = 1.0f;

    Quaternion originalRotation;

    Vector3 localX, localY, localZ;

    float t = 0.0f;

    private void Awake()
    {
        originalRotation = transform.rotation;
        localX = transform.right;
        localY = transform.up;
        localZ = transform.forward;
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 axisVector = Vector3.up;
        switch (axis)
        {
            case RotationAxis.LocalX:
                axisVector = localX;
                break;
            case RotationAxis.LocalY:
                axisVector = localY;
                break;
            case RotationAxis.LocalZ:
                axisVector = localZ;
                break;
            case RotationAxis.WorldX:
                axisVector = Vector3.right;
                break;
            case RotationAxis.WorldY:
                axisVector = Vector3.up;
                break;
            case RotationAxis.WorldZ:
                axisVector = Vector3.forward;
                break;
        }

        Vector3 rotation = (t * Mathf.Rad2Deg) * axisVector;

        transform.rotation = originalRotation * Quaternion.Euler(rotation);

        t = (t + Time.deltaTime * rotationSpeed) % (2.0f * Mathf.PI);
	}

    enum RotationAxis
    {
        LocalX,
        LocalY,
        LocalZ,
        WorldX,
        WorldY,
        WorldZ
    }
}
