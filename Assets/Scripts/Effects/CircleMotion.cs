using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMotion : MonoBehaviour
{
    [SerializeField] float radius = 5.0f;
    [SerializeField] float speedMultiplier = 1.0f;

    float t = 0.0f;

    Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        transform.position = originalPosition + new Vector3 (
            Mathf.Cos (t),
            0f, Mathf.Sin(t)
            ) * radius;

        t = (t + Time.deltaTime * speedMultiplier) % (2.0f * Mathf.PI);
    }
}
