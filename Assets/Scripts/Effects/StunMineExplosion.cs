using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMineExplosion : MonoBehaviour {

    [SerializeField] private VFXStunMineExplosion explosionEffect;

    // Use this for initialization
    private void OnEnable () {
        explosionEffect.Emit();
        Invoke("Die", 1f);
	}

    private void Die()
    {
        gameObject.SetActive(false);
    }
	
}
