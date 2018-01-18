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
    private SplatSO[] splats;
    [SerializeField]
    private float sphereRadius = 1F;
    [SerializeField]
    private float castDistance = 1.5F;

    #if UNITY_EDITOR

    [SerializeField]
    private bool debugSplats = false;

    #endif

    #endregion

    #region Fields (Private)

    #if UNITY_EDITOR
    private GameObject debugMarkers;
    #endif

    private bool shouldRenderSplats = false;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(splats.Length > 0);
        uvSpaceCamera.aspect = 1;

        #if UNITY_EDITOR
        
        debugMarkers = new GameObject("Debug Markers");
        debugMarkers.transform.SetParent(gameObject.transform);

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
        Destroy(debugMarkers);
        debugMarkers = new GameObject("Debug Markers");
        debugMarkers.transform.SetParent(gameObject.transform);
    }
    #endif

    public void QueueSplat(Vector3 worldPos, Vector2 projectileDir) {
        
        Vector3 raycastDir = new Vector3(projectileDir.x, 0, projectileDir.y);
        RaycastHit[] collided = Physics.SphereCastAll(worldPos, sphereRadius, raycastDir, castDistance);

        if (collided.Length != 0)
        {
            #if UNITY_EDITOR
            if (debugSplats)
            {
                GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                hitSphere.transform.SetParent(debugMarkers.transform);
                hitSphere.transform.up = -raycastDir;
                hitSphere.transform.position = worldPos + raycastDir / 2F;
                hitSphere.transform.localScale = new Vector3(sphereRadius, sphereRadius * 2 + castDistance,
                    sphereRadius);
                hitSphere.GetComponent<Collider>().enabled = false;
            }
            #endif

            shouldRenderSplats = true;
            
            SplatSO randomSplat = splats[Random.Range(0, splats.Length - 1)];             
            foreach (RaycastHit hit in collided) {
                Collider collider = hit.collider;
                GameObject obj = collider.gameObject;
                Splattable canSplat = obj.GetComponent<Splattable>();
                if (canSplat) {
                    canSplat.QueueNewSplat(randomSplat, worldPos, projectileDir);
                }
            }
        }
    }

    public void RenderSplats(CommandBuffer buffer)
    {
        uvSpaceCamera.AddCommandBuffer(CameraEvent.AfterEverything, buffer);
    }
}
