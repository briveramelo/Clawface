using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBulletExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<VFXBlasterProjectileImpact>().Emit();
        Invoke("KillYourself", 1f);
	}

    void KillYourself()
    {
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
