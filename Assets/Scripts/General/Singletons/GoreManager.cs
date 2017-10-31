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

    [SerializeField]
    private float sphereRadius = 1F;

    #if UNITY_EDITOR

    [SerializeField]
    private bool debugSplats = false;

    #endif

    #endregion

    #region Fields (Private)

    private int mask;
    #if UNITY_EDITOR

    GameObject debugSpheres;

    #endif

    #endregion

    protected override void Awake()
    {
        base.Awake();
        mask = 1 << 11; //GROUND LAYER
        uvSpaceCamera.aspect = 1;

        #if UNITY_EDITOR
        
        debugSpheres = new GameObject("Debug Spheres");
        debugSpheres.transform.SetParent(gameObject.transform);

        #endif
    }

    #if UNITY_EDITOR
    private void OnLevelWasLoaded(int level)
    {
        Destroy(debugSpheres);
        debugSpheres = new GameObject("Debug Spheres");
        debugSpheres.transform.SetParent(gameObject.transform);
    }
    #endif

    public void SpawnSplat(Vector3 worldPos) {

        Collider[] collided = Physics.OverlapSphere(worldPos, sphereRadius, mask);

        #if UNITY_EDITOR
        if (debugSplats && collided.Length != 0)
        {
            GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitSphere.transform.SetParent(debugSpheres.transform);
            hitSphere.transform.position = worldPos;
            hitSphere.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
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
