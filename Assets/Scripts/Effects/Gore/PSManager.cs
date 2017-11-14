using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour {

    #region Serialized Unity Fields
    [SerializeField] ParticleSystem ps;
    [SerializeField] float splatterLifetime = 2.5f;
    [SerializeField] GorePainter gp;
    #endregion

    #region Public Fields
    public float minDownScale = 0.25f;
    public float maxDownScale = 0.5f;
    public float splashRange = 1.5f;
    #endregion

    #region Private Fields
    private List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();
    #endregion

    private void OnParticleCollision(GameObject other)
    {
        ps.GetCollisionEvents(other, collEvents);
        Vector3 location = collEvents[0].intersection;
        GoreManager.Instance.SpawnSplat(location);
        gp.Paint(location);
    }

}
