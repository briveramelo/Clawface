using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMineExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<VFXStunMineExplosion>().Emit();
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
