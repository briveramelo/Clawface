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
        modifiedHitPoint.y += .5f;

        //get an object from pool
        GameObject bloodObject = ObjectPool.Instance.GetObject(PoolObjectType.BloodDecal);
        if (bloodObject)
        {
            bloodObject.SetActive(true);
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

            //Invoke("SetInActiveAfterTime", time);
            StartCoroutine(SetInactiveAfterTime(bloodObject, splatterLifetime));

        }
    }

    IEnumerator SetInactiveAfterTime(GameObject i_toSet, float i_Delay)
    {
        yield return new WaitForSeconds(i_Delay);

        i_toSet.SetActive(false);
    }
    
}
