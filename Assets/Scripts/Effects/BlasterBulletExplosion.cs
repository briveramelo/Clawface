using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

public class BlasterBulletExplosion : MonoBehaviour {

    [SerializeField] private VFXOneOff effect;

    private void OnEnable()
    {
        effect.Play();
        Invoke("Die", 1f);
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
	
}
