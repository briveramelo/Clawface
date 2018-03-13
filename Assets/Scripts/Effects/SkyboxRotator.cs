using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour {

    [SerializeField] float rotationSpeed = 1.0f;
    Material skyboxInstance;
    float rotation = 0.0f;

    private void Awake()
    {
        skyboxInstance = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyboxInstance;
    }

    private void Update()
    {
        rotation = (rotation + Time.deltaTime * rotationSpeed) % 360.0f;

        skyboxInstance.SetFloat("_Rotation", rotation);
    }
}
