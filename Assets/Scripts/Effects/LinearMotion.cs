using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMotion : MonoBehaviour {

	[SerializeField] float amplitude = 5.0f;
    [SerializeField] float speedMultiplier = 1.0f;

    Vector3 originalPosition;

    float t = 0.0f;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        transform.position = originalPosition + new Vector3 (
            Mathf.Sin (t), 0f, 0f
            ) * amplitude;

        t = (t + Time.deltaTime * speedMultiplier) % (2.0f * Mathf.PI);
    }
}
