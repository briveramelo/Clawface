using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class Splattable : MonoBehaviour
{

	#region Unity Serialization

	[Header ("Splat Stuffs")]
	[SerializeField] private Shader renderSplat;
	//get render splat shader via the inspector

	#endregion

	#region Private fields

	private RenderTexture splatMap;
	private Material renderMaterial;
	private Renderer myRenderer;
	private MaterialPropertyBlock propBlock;
    private Dims dims;

	#endregion

	#region Unity Lifecycle

	private void Awake ()
	{
		Assert.IsNotNull (renderSplat);

        dims = GetRenderDims();
		splatMap = new RenderTexture (dims.Width, dims.Height, 0, RenderTextureFormat.ARGB32);
		splatMap.Create ();
		renderMaterial = new Material (renderSplat);

		propBlock = new MaterialPropertyBlock ();
		myRenderer = GetComponent<Renderer> ();

		myRenderer.GetPropertyBlock (propBlock);
		propBlock.SetTexture ("_SplatMap", splatMap);
		myRenderer.SetPropertyBlock (propBlock);
	}

	#endregion

	#region Public Interface

	public CommandBuffer QueueSplat (Texture2D splat, Vector3 worldPos, Vector3 normal)
	{
        if (dims.Width == 0 || dims.Height == 0)
        {
            return null;
        }

		//set shader variables
		renderMaterial.SetTexture ("_Previous", splatMap);
		renderMaterial.SetVector ("_SplatLocation", worldPos);
		renderMaterial.SetVector ("_OriginalNormal", normal);
		renderMaterial.SetTexture ("_Decal", splat);

		//set up command buffer
		CommandBuffer splatBuffer = new CommandBuffer ();
		splatBuffer.GetTemporaryRT (Shader.PropertyToID ("_TEMPORARY"), dims.Width, dims.Height);
		splatBuffer.SetRenderTarget (Shader.PropertyToID ("_TEMPORARY"));
		splatBuffer.DrawRenderer (myRenderer, renderMaterial);
		splatBuffer.Blit (Shader.PropertyToID ("_TEMPORARY"), splatMap);
		return splatBuffer;
	}

    #endregion

    #region Private Interface

    private Dims GetRenderDims()
    {
        Dims dims;
        switch (SettingsManager.Instance.GoreDetail)
        {
            default:
            case 0:
                dims = new Dims(0, 0);
                break;
            case 1:
                dims = new Dims(32, 32);
                break;
            case 2:
                dims = new Dims(64, 64);
                break;
            case 3:
                dims = new Dims(128, 128);
                break;
            case 4:
                dims = new Dims(256, 256);
                break;
            case 5:
                dims = new Dims(512, 512);
                break;
        }
        return dims;
    }

    #endregion

    #region Types (Private)

    private struct Dims
    {
        #region Accessors (Public)

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        #endregion

        #region Fields (Private)

        private int width;
        private int height;

        #endregion

        #region Constructors (Public)

        public Dims(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        #endregion
    }

    #endregion
}
