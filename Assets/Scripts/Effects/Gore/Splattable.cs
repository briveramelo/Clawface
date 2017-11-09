using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class Splattable : MonoBehaviour {

    #region Unity Serialization
    
    [Header ("Splat Stuffs")]
    [SerializeField] private Shader renderSplat; //get render splat shader via the inspector

    [Header ("Render Texture Configuration")]
    [SerializeField] private int renderTextureWidth = 512;
    [SerializeField] private int renderTextureHeight = 512;

    #endregion

    #region Private fields
    
    private RenderTexture splatMap;
    private Material renderMaterial;
    private Renderer myRenderer;
    private MaterialPropertyBlock propBlock;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Assert.IsNotNull(renderSplat);

        splatMap = new RenderTexture(renderTextureWidth, renderTextureHeight, 0);
        renderMaterial = new Material(renderSplat);

        propBlock = new MaterialPropertyBlock();
        myRenderer = GetComponent<Renderer>();

        myRenderer.GetPropertyBlock(propBlock);
        propBlock.SetTexture("_SplatMap", splatMap);
        myRenderer.SetPropertyBlock(propBlock);
    }

    #endregion

    #region Public Interface
    public void DrawSplat(Texture2D splat, Vector3 worldPos, Vector3 normal, Camera renderCam)
    {
        //set shader variables
        renderMaterial.SetTexture("_Previous", splatMap);
        renderMaterial.SetVector("_SplatLocation", worldPos);
        renderMaterial.SetVector("_OriginalNormal", normal);
        renderMaterial.SetTexture("_Decal", splat);

        //set up command buffer
        CommandBuffer myCommandBuffer = new CommandBuffer();
        myCommandBuffer.GetTemporaryRT(Shader.PropertyToID("_TEMPORARY"), renderTextureWidth, renderTextureHeight);
        myCommandBuffer.SetRenderTarget(Shader.PropertyToID("_TEMPORARY"));
        myCommandBuffer.DrawRenderer(myRenderer, renderMaterial);
        myCommandBuffer.Blit(Shader.PropertyToID("_TEMPORARY"), splatMap);
        renderCam.AddCommandBuffer(CameraEvent.AfterEverything, myCommandBuffer);

        //render splat
        renderCam.Render();
        renderCam.RemoveAllCommandBuffers();
    }

    #endregion
}
