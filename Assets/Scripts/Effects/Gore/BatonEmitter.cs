using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonEmitter : MonoBehaviour {
    [SerializeField]
    ParticleSystem batonBloodParticleSystem;
    [SerializeField]
    float splatterLifetime = 2.5f;

    List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();

    public float MinScale = 0.75f;

    public float MaxScale = 3f;

    public float SplashRange = 1.5f;

    private void OnParticleCollision(GameObject other)
    {

        batonBloodParticleSystem.GetCollisionEvents(other, collEvents);

        Vector3 loc = collEvents[0].intersection;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
