using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : EmittedGameObject {

    float popIntervalMin;
    float popIntervalMax;
    float popForceMin;
    float popForceMax;
    float popOriginOffsetMax;
    float popRotationalForceMin;
    float popRotationalForceMax;
    
    float popTimer = 0.0f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        popTimer = Random.Range(popIntervalMin, popIntervalMax);
        originalScale = transform.localScale;
    }

    public void Init(float duration, AnimationCurve scaleCurve,
        float popIntervalMin, float popIntervalMax, float popForceMin, float popForceMax,
        float popOriginOffsetMax, float popRotationalForceMin, float popRotationalForceMax,
        float shaderFlopDist, float shaderFlopRate, float shaderFlopSpeed)
    {
        Init(duration, scaleCurve);

        this.popIntervalMin = popIntervalMin;
        this.popIntervalMax = popIntervalMax;
        this.popForceMin = popForceMin;
        this.popForceMax = popForceMax;
        this.popOriginOffsetMax = popOriginOffsetMax;
        this.popRotationalForceMin = popRotationalForceMin;
        this.popRotationalForceMax = popRotationalForceMax;

        // Init shader stuff
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        block.SetFloat("_FloppingDistance", shaderFlopDist);
        block.SetFloat("_FloppingRate", shaderFlopRate);
        block.SetFloat("_FloppingSpeed", shaderFlopSpeed);

        renderer.SetPropertyBlock(block);
    }

    protected override void Update ()
    {
        base.Update();

        if (lifeTimer < duration)
        {
            popTimer -= Time.deltaTime;
            if (popTimer <= 0.0f)
            {
                Pop();
                popTimer = Random.Range(popIntervalMin, popIntervalMax);
            }
        }
    }

    void Pop()
    {
        Vector3 force = new Vector3 (0.0f, 1.0f, 0.0f) * Random.Range(popForceMin, popForceMax);
        Vector3 forceOffset = new Vector3
            (
            Random.Range(-popOriginOffsetMax, popOriginOffsetMax),
            0.0f,
            Random.Range(-popOriginOffsetMax, popOriginOffsetMax)
            );
        rb.AddForceAtPosition(force/Time.fixedDeltaTime, transform.position + forceOffset, ForceMode.Acceleration);

        Vector3 torque = Random.insideUnitSphere * Random.Range(popRotationalForceMin, popRotationalForceMax);
        rb.AddTorque(torque, ForceMode.Acceleration);
    }
}
