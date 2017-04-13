using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class GorePainter : MonoBehaviour {
    
    [SerializeField] float splatterLifetime;
    [SerializeField] int maxOverlappingDecals;
    [SerializeField] private float minDownScale;
    [SerializeField] private float maxDownScale;
    [SerializeField] private float splashRange;

    void OnEnable() {
        gameObject.DeActivate(4f);
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

        //raycast around location to splash, but not on already placed blood
        int ground = LayerMasker.GetLayerMask(Layers.Ground);
        int blood = LayerMasker.GetLayerMask(Layers.Blood);
        if(Physics.Raycast(i_location , dir, out hit, splashRange, ground) && 
            Physics.RaycastNonAlloc(new Ray(i_location , dir), null, splashRange, blood)<maxOverlappingDecals){
                        
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

}
