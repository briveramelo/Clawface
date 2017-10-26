using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//world position
//normal at world position
//decal using to draw the blood (single sprite from the black and white sheet)
//previous rendertexture

public class GoreManager : Singleton<GoreManager> {

    #region Unity Serialization

    [SerializeField]
    private Camera uvSpaceCamera;

    #endregion

    private int mask;

    protected override void Awake()
    {
        base.Awake();
        mask = 1 << 11; //GROUND LAYER
    }

    public void SpawnSplat(Vector3 worldPos) {

        Collider[] collided = Physics.OverlapSphere(worldPos, 1F, mask);

        #if UNITY_EDITOR
        if (collided.Length != 0)
        {
            GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitSphere.transform.position = worldPos;
            hitSphere.GetComponent<Collider>().enabled = false;
        }
        #endif

        foreach (Collider collider in collided)
        {
            GameObject obj = collider.gameObject;
            Splattable canSplat = obj.GetComponent<Splattable>();
            if (canSplat)
            {
                canSplat.DrawSplat(worldPos, new Vector3(1, 0, 0), uvSpaceCamera); // we're not actually using the normal yet
            }
        }
    }
    
}
