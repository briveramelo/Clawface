using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingBotPatrolState : GrapplingBotState {

    float angularSpeed;
    public override void OnEnter() {
        angularSpeed = myStats.moveSpeed * 2 * Mathf.PI * Vector3.Distance(controller.transform.position, properties.rotationCenter.position);        
    }
    public override void Update() {
        properties.rotationCenter.Rotate(Vector3.up, angularSpeed);
        velBody.LookAt(properties.rotationCenter);
    }
    public override void OnExit() {

    }
}
