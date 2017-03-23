using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreExplosion : MonoBehaviour {

    [SerializeField]
    ParticleSystem explosionParticleSystem;

    [SerializeField]
    float splatterLifetime = 2.5f;

    List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();

    private float splashRange = 1.5f;

    [SerializeField]
    private GameObject[] limbs;

    [SerializeField]
    private float limbExplodeForce = 200f;

    [SerializeField]
    GorePainter gp;

    private void Awake()
    {
        for(int i = 0; i < limbs.Length; i++)
        {
            //add a small random force
            GameObject go = limbs[i];
            Rigidbody rb = go.GetComponent<Rigidbody>();
            int dir = Random.Range(0, 2);
            if(dir == 1)
            {
                rb.AddForce(go.transform.forward * limbExplodeForce);
            }
            else
            {
                rb.AddForce(-go.transform.forward * limbExplodeForce);
            }
            
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        explosionParticleSystem.GetCollisionEvents(other, collEvents);
        Vector3 loc = collEvents[0].intersection;
        gp.Paint(loc);
    }
    
}
