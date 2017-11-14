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

    private RenderTexture[] textures;
    private Material renderMaterial;
    private Renderer myRenderer;
    private MaterialPropertyBlock propBlock;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Assert.IsNotNull(renderSplat);

        textures = new RenderTexture[] {
                new RenderTexture(renderTextureWidth, renderTextureHeight, 0),
                new RenderTexture(renderTextureWidth, renderTextureHeight, 0)
            };
        renderMaterial = new Material(renderSplat);
        propBlock = new MaterialPropertyBlock();

        myRenderer = GetComponent<Renderer>();
    }

    #endregion

    #region Public Interface
    public void DrawSplat(Texture2D splat, Vector3 worldPos, Vector3 normal, Camera renderCam)
    {
        //set shader variables
        renderMaterial.SetTexture("_Previous", textures[1]);
        renderMaterial.SetVector("_SplatLocation", worldPos);
        renderMaterial.SetVector("_OriginalNormal", normal);
        renderMaterial.SetTexture("_Decal", splat);

        //set up command buffer
        CommandBuffer myCommandBuffer = new CommandBuffer();
        myCommandBuffer.SetRenderTarget(textures[0]);
        myCommandBuffer.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        myCommandBuffer.DrawRenderer(myRenderer, renderMaterial);
        renderCam.AddCommandBuffer(CameraEvent.AfterEverything, myCommandBuffer);

        //render splat
        renderCam.Render();
        renderCam.RemoveAllCommandBuffers();
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
