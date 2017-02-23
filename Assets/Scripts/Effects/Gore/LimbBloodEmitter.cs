//Garin
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LimbBloodEmitter : MonoBehaviour {

    [SerializeField]
    ParticleSystem limbBloodParticleSystem;
    [SerializeField]
    float splatterLifetime = 2.5f;

    List<ParticleCollisionEvent> collEvents = new List<ParticleCollisionEvent>();

    public float MinScale = 0.75f;

    public float MaxScale = 3f;

    public float SplashRange = 1.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateBloodGeyser();
        }
    }
    private void OnParticleCollision(GameObject  other)
    {

        limbBloodParticleSystem.GetCollisionEvents(other, collEvents);

        Vector3 loc = collEvents[0].intersection;
  
        Paint(loc, Color.red, 1);
        
        
    }

    private void ActivateBloodGeyser()
    {
        limbBloodParticleSystem.Play(false);
    }

    public void Paint(Vector3 location, Color color, int drops, float scaleBonus = 1f)
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
                PaintDecal(hit, color, scaleBonus);
                n++;
            }
        }
    }

    private void PaintDecal(RaycastHit hit, Color color, float scaleBonus)
    {
        //fuck z fighting
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
            float scaler = UnityEngine.Random.Range(MinScale, MaxScale) * scaleBonus;

            Vector3 newScaleDown = new Vector3(
                bloodObject.transform.localScale.x * 1 / scaler,
                bloodObject.transform.localScale.y * 1 / scaler,
                bloodObject.transform.localScale.z
            );

            bloodObject.transform.localScale = newScaleDown;

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
