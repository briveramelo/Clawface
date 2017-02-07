using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        Destroy(gameObject, 2f);	
	}	
}
