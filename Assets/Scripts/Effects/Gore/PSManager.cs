using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour
{

    #region Serialized Unity Fields

    [SerializeField]
    private float splatterLifetime = 2.5f;
    [SerializeField]
    private GorePainter gp;

    #endregion

    #region Private Fields

    private List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem ps;

    #endregion

    #region Unity LifeCycle

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    #endregion

    private void OnParticleCollision(GameObject other)
    {
        ps.GetCollisionEvents(other, collEvents);
        gp.Paint(collEvents[0].intersection);
    }

}