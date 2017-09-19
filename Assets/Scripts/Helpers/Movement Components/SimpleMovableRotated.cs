using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovableRotated : SimpleMovable {

    [SerializeField] float minTorque, maxTorque;

    public override void AddDecayingForce(Vector3 force, float decay) {
        float torque = Random.Range(minTorque, maxTorque);
        Vector3 randomTorque = Random.onUnitSphere * torque;
        rigbod.AddTorque(randomTorque);
        base.AddDecayingForce(force, decay);
    }
}
