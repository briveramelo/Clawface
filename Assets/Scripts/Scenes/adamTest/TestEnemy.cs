using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable {

    private Rigidbody rigid;    

    public void TakeDamage(float damage)
    {
        Debug.Log("Took damage: " + damage);
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
}
