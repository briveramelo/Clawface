using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMovieTexture : MonoBehaviour {

    [SerializeField]
    MovieTexture _movie;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().materials[1].mainTexture = _movie;
        ((MovieTexture)GetComponent<Renderer>().materials[1].mainTexture).Play();
	}
	
}
