//Garin
//Attach this to a limb to spawn a blood splatter when the 
//limb hits a surface.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbChunkPainter : MonoBehaviour {

    [SerializeField]
    float splatterLifetime = 10f;
    
    float minDownScale = 1f;
    float maxDownScale = 2f;
    private Collider[] dummyColliders;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Paint(contact.point, Color.red);
        dummyColliders=new Collider[3];
    }


    private void Paint(Vector3 loc, Color c)
    {
        RaycastHit hit;

        Vector3 dir = transform.TransformDirection(UnityEngine.Random.onUnitSphere * .1f);

        // Avoid raycast backward as we're in a 2D space
        if (dir.z < 0) dir.z = UnityEngine.Random.Range(0f, 1f);

        // Raycast around the position to splash everwhere we can
        int ground = LayerMasker.GetLayerMask(Layers.Ground);
        int blood = LayerMasker.GetLayerMask(Layers.Blood);
        
        if(Physics.Raycast(loc , dir, out hit, 1.5f, ground) && 
            Physics.OverlapSphereNonAlloc(hit.point, 0.2f, null, blood)<3){
                        
            PaintDecal(hit, c);
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

            Random scale;
            float scaler = UnityEngine.Random.Range(minDownScale, maxDownScale);

            Vector3 newScaleDown = new Vector3(
                bloodObject.transform.localScale.x * scaler,
                bloodObject.transform.localScale.y * scaler,
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
