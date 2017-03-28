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

    private List<Vector3> startPositions = new List<Vector3>();

    private void OnEnable(){
        ExplodeLimbs();        
    }

    private void Awake()
    {
        for (int i = 0; i < limbs.Length; i++) {
            startPositions.Add(limbs[i].transform.localPosition);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        explosionParticleSystem.GetCollisionEvents(other, collEvents);
        Vector3 loc = collEvents[0].intersection;
        gp.Paint(loc);
    }

    private void ExplodeLimbs() {
        for (int i = 0; i < limbs.Length; i++)
        {
            //add a small random force
            GameObject go = limbs[i];
            go.transform.localPosition = startPositions[i];
            Rigidbody rb = go.GetComponent<Rigidbody>();
            Vector3 explodeDirection = Random.onUnitSphere;
            explodeDirection.y = Random.Range(2.5f, 4f);
            explodeDirection.Normalize();

            rb.AddForceAtPosition(explodeDirection* limbExplodeForce, go.transform.position + Random.onUnitSphere);            
        }
    }
    
}
