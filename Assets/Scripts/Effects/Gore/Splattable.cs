using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class Splattable : MonoBehaviour {

    #region Unity Serialization
    
    [Header ("Splat Stuffs")]
    [SerializeField] private Texture2D[] splats; //set via the inspector
    [SerializeField] private Shader renderSplat; //get render splat shader via the inspector

    [Header ("Render Texture Configuration")]
    [SerializeField] private int renderTextureWidth = 512;
    [SerializeField] private int renderTextureHeight = 512;

    #endregion

    #region Private fields

    private RenderTexture[] textures;
    private Material renderMaterial;
    private Renderer myRenderer;
    private MaterialPropertyBlock propBlock;
    private CommandBuffer myCommandBuffer;
    private Camera renderCam;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Assert.IsNotNull(renderSplat);
        Assert.IsTrue(splats.Length > 0);

        textures = new RenderTexture[] {
                new RenderTexture(renderTextureWidth, renderTextureHeight, 0),
                new RenderTexture(renderTextureWidth, renderTextureHeight, 0)
            };
        renderMaterial = new Material(renderSplat);
        propBlock = new MaterialPropertyBlock();

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
        renderMaterial.SetTexture("_Previous", textures[1]);
        renderMaterial.SetVector("_SplatLocation", worldPos);
        renderMaterial.SetVector("_OriginalNormal", normal);
        renderMaterial.SetTexture("_Decal", randomlySelected);

        //set up command buffer
        myCommandBuffer.SetRenderTarget(textures[0]);
        myCommandBuffer.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        myCommandBuffer.DrawRenderer(myRenderer, renderMaterial);

        //render splat
        renderCam.Render();
        SwapBuffers();
        myRenderer.GetPropertyBlock(propBlock);
        if (propBlock != null)
        {
            propBlock.SetTexture("_SplatMap", textures[1]);
            myRenderer.SetPropertyBlock(propBlock);
        }
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
