using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class Splattable : MonoBehaviour {

    #region Public Fields

    public RenderTexture SplatMap
    {
        get
        {
            return textures[1];
        }
    }


    #endregion

    #region Unity Serialization

    [SerializeField] private Texture2D[] splats; //set via the inspector
    [SerializeField] private Shader renderSplat; //get render splat shader via the inspector

    #endregion

    #region Private fields

    private RenderTexture[] textures;
    private Material splatMaterial;
    private Renderer myRenderer;
    private CommandBuffer myCommandBuffer;
    private Camera renderCam;
    
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Assert.IsNotNull(renderSplat);
        Assert.IsTrue(splats.Length > 0);

        textures = new RenderTexture[2];
        splatMaterial = new Material(renderSplat);

        renderCam = gameObject.AddComponent<Camera>();
        renderCam.clearFlags = CameraClearFlags.SolidColor;
        renderCam.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        renderCam.orthographic = true;
        renderCam.nearClipPlane = 0.0f;
        renderCam.farClipPlane = 1.0f;
        renderCam.orthographicSize = 1.0f;
        renderCam.aspect = 1.0f;
        renderCam.useOcclusionCulling = false;
        renderCam.enabled = false;

        myRenderer = GetComponent<Renderer>();
        myCommandBuffer = new CommandBuffer();
        renderCam.AddCommandBuffer(CameraEvent.AfterEverything, myCommandBuffer);
    }

    #endregion

    #region Public Interface
    public void DrawSplat(Vector3 worldPos, Vector3 normal)
    {
        //grab random decal
        Texture2D randomlySelected = splats[Random.Range(0, splats.Length - 1)];

        //set shader variables
        splatMaterial.SetTexture("_Previous", textures[1]);
        splatMaterial.SetVector("_SplatLocation", worldPos);
        splatMaterial.SetVector("_OriginalNormal", normal);
        splatMaterial.SetTexture("_Decal", randomlySelected);

        //set up command buffer
        myCommandBuffer.SetRenderTarget(textures[0]);
        myCommandBuffer.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        myCommandBuffer.DrawRenderer(myRenderer, splatMaterial);

        //render splat
        renderCam.Render();
        SwapBuffers();  
    }

    #endregion

    #region Private Interface

    private void SwapBuffers()
    {
        RenderTexture temp = textures[0];
        textures[0] = textures[1];
        textures[1] = temp;
    }

    #endregion
}
