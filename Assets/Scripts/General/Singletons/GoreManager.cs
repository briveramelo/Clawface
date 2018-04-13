﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class GoreManager : Singleton<GoreManager> {

    #region Accessors (Public)

    public bool GoreEnabled
    {
        get
        {
            return SettingsManager.Instance.GoreDetail > 0;
        }
    }

    public Shader AdvancedSplatShader
    {
        get
        {
            return advancedSplatShader;
        }
    }

    public Shader SimpleSplatShader
    {
        get
        {
            return simpleSplatShader;
        }
    }

    public int NumSplatsToRender
    {
        get
        {
            return numSplatsToRender;
        }
    }

    public Vector2 FrameDim
    {
        get
        {
            return frameDim;
        }
    }

    #endregion

    #region Unity Serialization

    [Header ("Custom Cameras")]
    [SerializeField]
    private Camera uvSpaceCamera;

    [Header ("Splat Queuing Stuffs")]
    [SerializeField]
    private SplatSO[] splats;
    [SerializeField]
    private float sphereRadius = 1F;
    [SerializeField]
    private float castDistance = 1.5F;

    [Header("Splat Rendering Stuffs")]
    [SerializeField]
    private Shader advancedSplatShader = null;

    [SerializeField]
    private Shader simpleSplatShader = null;

    [SerializeField]
    private int numSplatsToRender = 10;

    [SerializeField]
    private Vector2 frameDim = new Vector2(256, 256);   

    [Header("Editor Only")]
    [SerializeField]
    private bool debugSplats = false;

    #endregion

    #region Fields (Private)

    #if UNITY_EDITOR
    private GameObject debugMarkers;
    #endif

    private bool shouldRenderSplats = false;

    #endregion

    #region Interface (Unity Lifecycle)

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

    #endregion

    #region Interface (Public)

    public void AddBloodBuffer(CommandBuffer buffer)
    {
        uvSpaceCamera.AddCommandBuffer(CameraEvent.AfterEverything, buffer);
        shouldRenderSplats = true;
    }

    public void EmitDirectionalBlood(DamagePack pack)
    {
        // Determine Position to emit blood from:
        Vector3 position = pack.damaged.owner.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 5F,
            LayerMasker.GetLayerMask(Layers.Ground)))
        {
            position = hit.point;
        }
        else
        {
            // We'll set the 'y' to zero which should be near the floor...
            position.y = 0;
        }

        // Obtain lateral impact direction
        Vector3 impactDir = pack.damager.impactDirection;
        Vector2 projectileDir = new Vector2(impactDir.x, impactDir.z);

        // Queue Up Directional Splat
        QueueDirectionalSplat(position, projectileDir);
    }

    #endregion

    #region Interface (Private)

    private void QueueDirectionalSplat(Vector3 worldPos, Vector2 projectileDir) {
        
        Vector3 raycastDir = new Vector3(projectileDir.x, 0, projectileDir.y);
        RaycastHit[] collided = Physics.SphereCastAll(worldPos, sphereRadius, raycastDir, castDistance);

        if (collided.Length != 0)
        {
            #if UNITY_EDITOR
            if (debugSplats)
            {
                GameObject hitSphere = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                hitSphere.transform.SetParent(debugMarkers.transform);
                hitSphere.transform.up = -raycastDir;
                hitSphere.transform.position = worldPos + raycastDir / 2F;
                hitSphere.transform.localScale = new Vector3(sphereRadius,
                    castDistance, sphereRadius);
                hitSphere.GetComponent<Collider>().enabled = false;
            }
#endif

            if (GoreEnabled) {
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
    }

    #endregion
}
