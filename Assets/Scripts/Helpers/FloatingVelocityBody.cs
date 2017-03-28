using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingVelocityBody : VelocityBody {

    void Awake() {
        isGrounded = false;
    }

    protected override void Update() { }
}
