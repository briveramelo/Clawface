using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovable : MonoBehaviour, IMovable {

    [SerializeField] protected Rigidbody rigbod;
    [SerializeField] protected float forceMultiplier;

    public virtual void AddDecayingForce(Vector3 force, float decay) {
        rigbod.AddForce(force * forceMultiplier);
    }

    public virtual Vector3 GetForward() {
        return transform.forward;
    }

    public virtual Quaternion GetRotation() {
        return transform.rotation;
    }

    public virtual bool IsGrounded() {
        return true;
    }

    public virtual void SetMovementMode(MovementMode mode) {
        
    }

    public virtual void StopVerticalMovement() {
        rigbod.velocity = new Vector3(rigbod.velocity.x, 0f, rigbod.velocity.z);        
    }
}
