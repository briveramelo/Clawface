using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//world position
//normal at world position
//decal using to draw the blood (single sprite from the black and white sheet)
//previous rendertexture

public class GoreManager : Singleton<GoreManager> {

    private int mask;

    protected override void Awake()
    {
        base.Awake();
        mask = 1 << 11; //GROUND LAYER
    }

    public void SpawnSplat(Vector3 worldPos) {

        RaycastHit h;
        
        if (Physics.SphereCast(worldPos, 1f, transform.forward, out h, 1f, mask))
        {
            GameObject hitObject = h.transform.gameObject;

#if UNITY_EDITOR
            GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitSphere.transform.position = worldPos;
#endif
            Splattable canSplat = hitObject.GetComponent<Splattable>();

            if (canSplat)
            {
                canSplat.DrawSplat(h.point,h.normal);
            }
            
        }
    }
    
}
