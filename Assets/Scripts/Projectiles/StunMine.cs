﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMine : MonoBehaviour {

    [SerializeField]
    private float damage;

    [SerializeField]
    private VFXStunMineExplosion explosionEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Strings.ENEMY)
        {
            explosionEffect.Emit();
            other.GetComponent<IDamageable>().TakeDamage(damage);
            other.GetComponent<IStunnable>().Stun();
            gameObject.SetActive(false);
        }
    }
}
