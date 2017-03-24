using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour {

    [SerializeField]
    ParticleSystem ps;
    [SerializeField]
    float splatterLifetime = 2.5f;
    [SerializeField]
    GorePainter gp;

    List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();
    public float minDownScale = 0.25f;
    public float maxDownScale = 0.5f;
    public float splashRange = 1.5f;

    private void OnParticleCollision(GameObject other)
    {
        ps.GetCollisionEvents(other, collEvents);
        Vector3 location = collEvents[0].intersection;
        gp.Paint(location);
    }

}
