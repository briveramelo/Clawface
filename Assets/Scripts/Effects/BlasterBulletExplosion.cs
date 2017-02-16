using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBulletExplosion : MonoBehaviour {

    [SerializeField] private VFXBlasterProjectileImpact effect;

    private void OnEnable()
    {
        effect.Emit();
        Invoke("Die", 1f);
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
	
}
