using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class GoreManager : Singleton<GoreManager> {

    #region Unity Serialization

    [Header ("Custom Cameras")]
    [SerializeField]
    private Camera uvSpaceCamera;

    [Header ("Splat Stuffs")]
    [SerializeField]
    private Texture2D[] splats;
    [SerializeField]
    private float sphereRadius = 1F;

    #if UNITY_EDITOR

    [SerializeField]
    private bool debugSplats = false;

    #endif

    #endregion

    #region Fields (Private)
    #if UNITY_EDITOR
    private GameObject debugSpheres;
    #endif

    private bool shouldRenderSplats = false;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(splats.Length > 0);
        uvSpaceCamera.aspect = 1;

        #if UNITY_EDITOR
        
        debugSpheres = new GameObject("Debug Spheres");
        debugSpheres.transform.SetParent(gameObject.transform);

        #endif
    }

    private void LateUpdate()
    {
        // Render Splats
        if (shouldRenderSplats)
        {
            uvSpaceCamera.Render();
            uvSpaceCamera.RemoveAllCommandBuffers();
            shouldRenderSplats = false;
        }
    }

#if UNITY_EDITOR
    private void OnLevelWasLoaded(int level)
    {
        Destroy(debugSpheres);
        debugSpheres = new GameObject("Debug Spheres");
        debugSpheres.transform.SetParent(gameObject.transform);
    }
    #endif

    public void QueueSplat(Vector3 worldPos) {

        Collider[] collided = Physics.OverlapSphere(worldPos, sphereRadius);
        
        if (collided.Length != 0)
        {
            #if UNITY_EDITOR
            if (debugSplats)
            {
                GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitSphere.transform.SetParent(debugSpheres.transform);
                hitSphere.transform.position = worldPos;
                hitSphere.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
                hitSphere.GetComponent<Collider>().enabled = false;
            }
            #endif

            shouldRenderSplats = true;
            Texture2D randomSplat = splats[Random.Range(0, splats.Length - 1)];
            foreach (Collider collider in collided)
            {
                GameObject obj = collider.gameObject;
                Splattable canSplat = obj.GetComponent<Splattable>();
                if (canSplat)
                {
                    // we're not using the normal yet
                    var buffer = canSplat.QueueSplat(randomSplat, worldPos, new Vector3(1, 0, 0));
                    uvSpaceCamera.AddCommandBuffer(CameraEvent.AfterEverything, buffer);
                }
            }
        }
    }
    
}
