using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//world position
//normal at world position
//previous splat texture (via splattable)

public class GoreManager : Singleton<GoreManager> {
    
    public void SpawnSplat(Vector3 worldPos) {

        RaycastHit hit;

        if (Physics.SphereCast(worldPos, 1f, transform.forward, out hit, 1f))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.tag == Strings.Tags.FLOOR || hitObject.tag == Strings.Tags.WALL)
            {
                //Splattable s = hitObject.GetComponent<Splattable>();
                Splattable splattable = null;
                
                GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitSphere.transform.position = worldPos;
                GameObject decal = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodDecal);
                Material bloodMat = decal.GetComponent<MeshRenderer>().material;

                if (bloodMat)
                {
                    Texture bloodMask = bloodMat.GetTexture("_BloodMask");

#if UNITY_EDITOR
                    Debug.Log(hitObject.name + " W:" + bloodMask.width + " H:" + bloodMask.height);
#endif
                }
            }
        }
    }
    
}
