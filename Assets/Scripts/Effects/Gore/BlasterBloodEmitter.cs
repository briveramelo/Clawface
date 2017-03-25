//Garin
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlasterBloodEmitter : MonoBehaviour {

    [SerializeField]
    ParticleSystem blasterBloodParticleSystem;
    [SerializeField]
    float splatterLifetime = 2.5f;

    List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();

    //private float minDownScale = 0.25f;

    //private float maxDownScale = 0.5f;

    private float splashRange = 1.5f;

    void OnEnable() {
        Invoke("Deactivate", 1f);
    }    

    private void OnParticleCollision(GameObject other)
    {
        blasterBloodParticleSystem.GetCollisionEvents(other, collEvents);
        Vector3 loc = collEvents[0].intersection;
        Paint(loc, Color.red, 1);    
    }

    private void Paint(Vector3 loc, Color c, int drops)
    {
        RaycastHit hit;

        // Generate multiple decals in once
        int n = 0;
        while (n < drops)
        {
            Vector3 dir = transform.TransformDirection(UnityEngine.Random.onUnitSphere * splashRange);

            // Avoid raycast backward as we're in a 2D space
            if (dir.z < 0) dir.z = UnityEngine.Random.Range(0f, 1f);

            // Raycast around the position to splash everwhere we can
            if (Physics.Raycast(loc, dir, out hit, splashRange))
            {
                PaintDecal(hit, c);
                n++;
            }
        }
    }

    private void PaintDecal(RaycastHit hit, Color c)
    {
        //eff z fighting
        Vector3 modifiedHitPoint = hit.point;
        modifiedHitPoint.y += .05f;

        //get an object from pool
        GameObject bloodObject = ObjectPool.Instance.GetObject(PoolObjectType.BloodDecal);
        if (bloodObject)
        {
            bloodObject.transform.position = modifiedHitPoint;
            bloodObject.transform.rotation = Quaternion.FromToRotation(Vector3.back, hit.normal);

            Vector3 angs = bloodObject.transform.rotation.eulerAngles;
            bloodObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            Vector3 newNormal = bloodObject.transform.forward;

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
            bloodObject.transform.RotateAround(hit.point, newNormal, rater);

            ObjectPool.Instance.StartCoroutine(SetInactiveAfterTime(bloodObject, splatterLifetime));
        }
    }
    
    IEnumerator SetInactiveAfterTime(GameObject i_toSet, float i_delay)
    {
        yield return new WaitForSeconds(i_delay);

        i_toSet.SetActive(false);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
