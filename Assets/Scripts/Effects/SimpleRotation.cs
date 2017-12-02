using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{

    [SerializeField] float rotationSpeed = 1.0f;

    Quaternion originalRotation;

    float t = 0.0f;

    private void Awake()
    {
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 rotation = new Vector3(0.0f, t * Mathf.Rad2Deg, 0.0f);

        transform.rotation = originalRotation * Quaternion.Euler(rotation);

        t = (t + Time.deltaTime * rotationSpeed) % (2.0f * Mathf.PI);
	}
}
