using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable, IMovable {

    private Rigidbody rigid;

    void IMovable.AddExternalForce(Vector3 force, float decay)
    {
        Debug.Log("Applied force: " + force);
        force = new Vector3(force.x, 0f, force.z);
        rigid.isKinematic = false;
        rigid.AddForce(force);

    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Took damage: " + damage);
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
}
