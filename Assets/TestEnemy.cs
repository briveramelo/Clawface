using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable, IMovable {
    public void AddExternalForce(Vector3 force)
    {
        Debug.Log("Applied force: " + force);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Took damage: " + damage);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
