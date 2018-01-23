using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEmitter : GameObjectEmitter {

    [SerializeField] float shaderFlopDistanceMin = 0.05f;
    [SerializeField] float shaderFlopDistanceMax = 0.05f;
    [SerializeField] float shaderFlopRateMin = 4.7f;
    [SerializeField] float shaderFlopRateMax = 4.7f;
    [SerializeField] float shaderFlopSpeedMin = 20.0f;
    [SerializeField] float shaderFlopSpeedMax = 20.0f;
    [SerializeField] float popIntervalMin = 0.5f;
    [SerializeField] float popIntervalMax = 1.0f;
    [SerializeField] float popForceMin = 1.0f;
    [SerializeField] float popForceMax = 2.0f;
    [SerializeField] float popRotationalForceMin = 30.0f;
    [SerializeField] float popRotationalForceMax = 60.0f;
    [SerializeField] float popOriginOffsetMax = 2.0f;

    protected override GameObject Emit()
    {
        GameObject instance = base.Emit();

        Fish fish = instance.GetComponent<Fish>();
        float flopDist = Random.Range(shaderFlopDistanceMin, shaderFlopDistanceMax);
        float flopRate = Random.Range(shaderFlopRateMin, shaderFlopRateMax);
        float flopSpeed = Random.Range(shaderFlopSpeedMin, shaderFlopSpeedMax);
        fish.Init(duration, scaleCurve,
            popIntervalMin, popIntervalMax, popForceMin, popForceMax, 
            popOriginOffsetMax, popRotationalForceMin, popRotationalForceMax,
            flopDist, flopRate, flopSpeed);

        return instance;
    }
}
