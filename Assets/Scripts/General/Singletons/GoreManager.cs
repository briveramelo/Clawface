using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreManager : Singleton<GoreManager> {

    //List<Vector3> splatsInWorld = new List<Vector3>

    public void SpawnSplat(Vector3 worldPos) {

        RaycastHit hit;

        if (Physics.SphereCast(worldPos, 1f, transform.forward, out hit, 1f))
        {

            GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitSphere.transform.position = worldPos;
            GameObject decal = ObjectPool.Instance.GetObject(PoolObjectType.VFXBloodDecal);
            Material bloodMat = decal.GetComponent<MeshRenderer>().material;

            if (bloodMat)
            {
                Texture bloodMask = bloodMat.GetTexture("_BloodMask");

#if UNITY_EDITOR
                Debug.Log("W:\t" + bloodMask.width + " H:\t" + bloodMask.height);
#endif
            }
        }
    }
    
}
