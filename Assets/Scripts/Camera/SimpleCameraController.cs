// SimpleCameraController.cs
// Author: Aaron

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SimpleCameraController : MonoBehaviour
{
    const float DECAY = 2.0f;
    const float R_DECAY = 3.0f;
    const float ACCEL = 10.0f;
    const float R_ACCEL = 1500.0f;
    const float MAX_P_VELOCITY = 15.0f;
    const float MAX_R_VELOCITY = 90.0f;

    Vector3 velocity = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    Vector3 rVelocity = Vector3.zero;

    Vector2 lastMousePos = Vector2.zero;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rotation = transform.rotation.eulerAngles;
    }

    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape)) Application.Quit();

        // Translation
        velocity *= 1.0f - (DECAY * Time.deltaTime);

        Vector3 dVelocity = Vector3.zero;

        dVelocity.x += Input.GetKey (KeyCode.D) ? ACCEL * Time.deltaTime : 0.0f;
        dVelocity.x -= Input.GetKey (KeyCode.A) ? ACCEL * Time.deltaTime : 0.0f;
        dVelocity.y += Input.GetKey (KeyCode.Space) ? ACCEL * Time.deltaTime : 0.0f;
        dVelocity.y -= Input.GetKey (KeyCode.LeftControl) ? ACCEL * Time.deltaTime : 0.0f;
        dVelocity.z += Input.GetKey (KeyCode.W) ? ACCEL * Time.deltaTime : 0.0f;
        dVelocity.z -= Input.GetKey (KeyCode.S) ? ACCEL * Time.deltaTime : 0.0f;

        if (Input.GetKey(KeyCode.LeftShift)) dVelocity *= 2.0f;

        velocity = Vector3.ClampMagnitude (velocity + dVelocity, MAX_P_VELOCITY);

        transform.position += (
            transform.right * velocity.x + 
            transform.up * velocity.y +
            transform.forward * velocity.z) * Time.deltaTime;

        // Rotation
        rVelocity *= 1.0f - (R_DECAY * Time.deltaTime);

        Vector2 transformedMousePos = new Vector2 (
            Input.GetAxis ("Mouse X"), 
            Input.GetAxis ("Mouse Y")
        );

        rVelocity.y += transformedMousePos.x * R_ACCEL * Time.deltaTime;
        rVelocity.x += -transformedMousePos.y * R_ACCEL * Time.deltaTime;

        rVelocity = Vector3.ClampMagnitude (rVelocity, MAX_R_VELOCITY);

        rotation += rVelocity * Time.deltaTime;
        rotation.x = Mathf.Clamp (rotation.x, -90.0f, 90.0f);
        transform.rotation = Quaternion.Euler (rotation);

        lastMousePos = transformedMousePos;
    }
}
