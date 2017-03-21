//Garin
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloodDripEmitter : MonoBehaviour {

    [SerializeField]
    ParticleSystem bloodDripParticleSystem;
    [SerializeField]
    float splatterLifetime = 2.5f;

    List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();

    public float minDownScale = 0.25f;

    public float maxDownScale = 0.5f;

    public float SplashRange = 1.5f;


    private void OnParticleCollision(GameObject other)
    {

        bloodDripParticleSystem.GetCollisionEvents(other, collEvents);

        Vector3 loc = collEvents[0].intersection;
        
        Paint(loc, Color.red, 1);
    }

    public void Paint(Vector3 location, Color color, int drops)
    {
        RaycastHit hit;

        // Generate multiple decals in once
        int n = 0;
        while (n < drops)
        {
            Vector3 dir = transform.TransformDirection(UnityEngine.Random.onUnitSphere * SplashRange);

            // Avoid raycast backward as we're in a 2D space
            if (dir.z < 0) dir.z = UnityEngine.Random.Range(0f, 1f);

            // Raycast around the position to splash everwhere we can
            if (Physics.Raycast(location, dir, out hit, SplashRange))
            {
                PaintDecal(hit, color);
                n++;
            }
        }
    }

    private void PaintDecal(RaycastHit hit, Color color)
    {
        //fuck z fighting
        Vector3 modifiedHitPoint = hit.point;
        modifiedHitPoint.y += .5f;

        //get an object from pool
        GameObject bloodObject = ObjectPool.Instance.GetObject(PoolObjectType.BloodDecal);
        if (bloodObject)
        {
            bloodObject.transform.position = modifiedHitPoint;
            bloodObject.transform.rotation = Quaternion.FromToRotation(Vector3.back, hit.normal);

            // Random scale
            //float scaler = UnityEngine.Random.Range(minDownScale, maxDownScale);

            //Vector3 newScaleDown = new Vector3(
            //    bloodObject.transform.localScale.x *  scaler,
            //    bloodObject.transform.localScale.y *  scaler,
            //    bloodObject.transform.localScale.z
            //);

            //bloodObject.transform.localScale = newScaleDown;

            // Random rotation effect
            int rater = UnityEngine.Random.Range(0, 359);
            bloodObject.transform.RotateAround(hit.point, hit.normal, rater);

            StartCoroutine(SetInactiveAfterTime(bloodObject, splatterLifetime));

        }
    }

    IEnumerator SetInactiveAfterTime(GameObject i_toSet, float i_delay)
    {
        yield return new WaitForSeconds(i_delay);

        i_toSet.SetActive(false);
    }


}
