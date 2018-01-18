using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class Splattable : MonoBehaviour
{
    #region Accessors

    /// <summary>
    /// Gets or Sets the PaintMask.
    /// (This will primarily be done via the Dynamic system.)
    /// </summary>
    public Texture2D PaintMask
    {
        get
        {
            return paintMask;
        }
        set
        {
            paintMask = value;
        }
    }

    #endregion

    #region Unity Serialization

    [Header ("Splat Stuffs")]
    [SerializeField] private Texture2D paintMask = Texture2D.whiteTexture;
    [SerializeField] private Texture2D renderMask = Texture2D.whiteTexture;

	[Header ("Render Texture Configuration")]
	[SerializeField] private int renderTextureWidth = 512;
	[SerializeField] private int renderTextureHeight = 512;

    [Header ("Splat Rendering Configuration (You probably shouldn't edit these!)")]
    [Tooltip ("This SHOULD BE SET TO SOMETHING.")]
    [SerializeField] private Shader renderSplat = null;
    [SerializeField] private int numSplatsToRender = 10;
    [SerializeField] private Vector2 frameDim = new Vector2(256, 256);
    [SerializeField] private int FPS = 12;

    #endregion

    #region Private fields

    private RenderTexture splatMap;
	private Material paintingMaterial;
	private Renderer myRenderer;
	private MaterialPropertyBlock propBlock;

    /// <summary>
    /// This queue will contain splats that need to be rendered to the
    /// splattable.
    /// </summary>
    private Queue<QueuedSplat> splatsToRender;

	#endregion

	#region Unity Lifecycle

	private void Awake ()
	{
        // Assertions
		Assert.IsNotNull (renderSplat);

        // Set Up Render Data
		splatMap = new RenderTexture (renderTextureWidth, renderTextureHeight, 0, RenderTextureFormat.ARGB32);
		splatMap.Create ();
		paintingMaterial = new Material (renderSplat);

		propBlock = new MaterialPropertyBlock ();
		myRenderer = GetComponent<Renderer> ();

		myRenderer.GetPropertyBlock (propBlock);
		propBlock.SetTexture ("_SplatMap", splatMap);
        propBlock.SetTexture ("_RenderMask", renderMask);
		myRenderer.SetPropertyBlock (propBlock);

        // Set Up Queuing System
        splatsToRender = new Queue<QueuedSplat>();
	}

    private void Update ()
    {
        // If we have some splat data to render, create a command
        // buffer for it and give it to the GoreManager to do the
        // rendering.
        if (splatsToRender.Count > 0)
        {
            CommandBuffer buffer = CreateCommandBuffer();
            GoreManager.Instance.RenderSplats(buffer);
        }
    } 

    #endregion

    #region Public Interface

    /// <summary>
    /// Queues a Splat up to be rendered.
    /// </summary>
    /// <param name="splatData">The SplatSO describing the splat.</param>
    /// <param name="worldPos">The world position to anchor the splat at.</param>
    /// <param name="projectileDir">The direction to splat in.</param>
    public void QueueNewSplat (SplatSO splatData, Vector3 worldPos, Vector2 projectileDir)
	{
        QueuedSplat splat = new QueuedSplat();
        splat.splatData = splatData;
        splat.frame = 0;
        splat.worldPos = worldPos;
        splat.projectileDir = projectileDir;

        splatsToRender.Enqueue(splat);
	}

    #endregion

    #region Interface (Private)

    /// <summary>
    /// Creates a CommandBuffer and primes the material to do rendering.
    /// This should ONLY be called ONCE per FRAME.
    /// </summary>
    /// <returns>A command buffer that will render splats.</returns>
    private CommandBuffer CreateCommandBuffer()
    {
        List<Texture2D> masks = new List<Texture2D>();
        List<Texture2D> normals = new List<Texture2D>();
        List<Vector4> anchors = new List<Vector4>(); // must be vec4 in order to set array
        List<Vector4> rotations = new List<Vector4>(); // must be vec4 in order to set array
        List<Vector4> positions = new List<Vector4>(); // must be vec4 in order to set array
        int count = 0;

        while (splatsToRender.Count > 0 && count < numSplatsToRender)
        {
            // Queue Up Data
            QueuedSplat splat = splatsToRender.Dequeue();
            SplatSO data = splat.splatData;
            SplatSO.Frame frame = data.Frames[splat.frame];
            masks.Add(frame.mask);
            normals.Add(frame.normal);
            anchors.Add(data.Anchor);
            rotations.Add(data.CalculateRotation(splat.projectileDir));
            positions.Add(splat.worldPos);
            count++;

            // Check to see if this splat will have more frames
            // and queue it up with a "delay".
            if (data.HasMoreFrames(splat.frame + 1))
            {
                splat.frame++;
                StartCoroutine(DelayFrame(splat));
            }
        }

        // Set Shader Variables
        // TODO if possible: Convert renderMaterial to a common material and make these [PerRendererData]
        paintingMaterial.SetTexture("_Previous", splatMap);
        paintingMaterial.SetTexture("_PaintMask", paintMask);
        paintingMaterial.SetTexture("_Masks", CreateTextureArray(masks));
        paintingMaterial.SetTexture("_Normals", CreateTextureArray(normals));
        paintingMaterial.SetVectorArray("_Anchors", anchors);
        paintingMaterial.SetVectorArray("_Rotations", rotations);
        paintingMaterial.SetVectorArray("_Positions", positions);
        paintingMaterial.SetInt("_Count", count);

        // Set Up Command Buffer
        CommandBuffer splatBuffer = new CommandBuffer ();
        splatBuffer.GetTemporaryRT(Shader.PropertyToID("_TEMPORARY"), renderTextureWidth, renderTextureHeight);
        splatBuffer.SetRenderTarget(Shader.PropertyToID("_TEMPORARY"));
        splatBuffer.DrawRenderer(myRenderer, paintingMaterial);
        splatBuffer.Blit(Shader.PropertyToID("_TEMPORARY"), splatMap);
        return splatBuffer;
    }

    /// <summary>
    /// Creates a Texture2DArray from a list of 2D textures.
    /// </summary>
    /// <param name="textures">The List of Texture2D objects to pack in an array.</param>
    /// <returns>A Texture2DArray containing the textures in the argument list.</returns>
    private Texture2DArray CreateTextureArray(List<Texture2D> textures)
    {
        Texture2DArray array = new Texture2DArray( Mathf.FloorToInt(frameDim.x), Mathf.FloorToInt(frameDim.y),
            textures.Count, TextureFormat.ARGB32, false);
        for (int index = 0; index < textures.Count; index++)
        {
            Graphics.CopyTexture(textures[index], 0, array, index);
        }
        return array;
    }

    /// <summary>
    /// Coroutine used to delay adding a frame back into the "toRender" queue.
    /// </summary>
    /// <param name="toQueue">The splat to delay adding back.</param>
    /// <returns>A Coroutine enumerator.</returns>
    private IEnumerator DelayFrame(QueuedSplat toQueue)
    {
        yield return new WaitForSeconds(1F / FPS);
        splatsToRender.Enqueue(toQueue);
    }

    #endregion

    #region Types (Private)

    /// <summary>
    /// This struct is used to track data being rendered by the Splattable.
    /// We have to queue this data up and render it in batches since there
    /// could potentially be multiple sets of data being rendered to same tile
    /// every frame.
    /// </summary>
    private struct QueuedSplat
    {
        #region Fields (Public)

        /// <summary>
        /// The data defining the splat to render.
        /// </summary>
        public SplatSO splatData;

        /// <summary>
        /// The frame to render.
        /// </summary>
        public int frame;

        /// <summary>
        /// The position to render the splat at.
        /// </summary>
        public Vector3 worldPos;

        /// <summary>
        /// The direction (normalized velocity) of the projectile that caused this splat.
        /// </summary>
        public Vector2 projectileDir;

        #endregion
    }

    #endregion
}
