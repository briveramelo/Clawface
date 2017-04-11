using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorePainter : MonoBehaviour {
    
    [SerializeField] float splatterLifetime = 2.5f;

    public float minDownScale = 0.25f;
    public float maxDownScale = 0.5f;
    public float splashRange = 1.5f;

    void OnEnable() {
        StartCoroutine(SetInactiveAfterTime(gameObject, 4f));
    }


    public void Paint(Vector3 i_location)
    {
        RaycastHit hit;

        Vector3 dir = transform.TransformDirection(UnityEngine.Random.onUnitSphere * splashRange);

        //avoid raycast backward
        if(dir.z < 0)
        {
            dir.z = UnityEngine.Random.Range(0f, 1f);
        }

        //raycast around location to splash
        if(Physics.Raycast(i_location , dir, out hit, splashRange))
        {
            PlaceDecal(hit);
        }
    }

    private void PlaceDecal(RaycastHit i_hit)
    {
        //eff z fighting, place things up a bit
        Vector3 modifiedHitPoint = i_hit.point;
        modifiedHitPoint += .05f *  i_hit.normal;

        //get decal from pool
        GameObject decal = ObjectPool.Instance.GetObject(PoolObjectType.BloodDecal);

        //if we have a decal to draw from the pool
        if (decal)
        {
            decal.transform.position = modifiedHitPoint;
            decal.transform.rotation = Quaternion.FromToRotation(Vector3.back, i_hit.normal);

            //ScaleDownBlood(decal);
            

            //random rotation
            int rater = UnityEngine.Random.Range(0, 359);
            decal.transform.RotateAround(i_hit.point, i_hit.normal, rater);

            StartCoroutine(SetInactiveAfterTime(decal, splatterLifetime));
        }
    }

    private void ScaleDownBlood(GameObject decal) {
        //Random scale
        float scaler = UnityEngine.Random.Range(minDownScale, maxDownScale);

        Vector3 newScaleDown = new Vector3(
            decal.transform.localScale.x * scaler,
            decal.transform.localScale.y * scaler,
            decal.transform.localScale.z
        );

        decal.transform.localScale = newScaleDown;
    }

    IEnumerator SetInactiveAfterTime(GameObject i_toSet, float i_delay)
    {
        yield return new WaitForSeconds(i_delay);
        i_toSet.SetActive(false);        
    }

}
