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
            if (GoreManager.Instance.GoreEnabled) {
                return splatter.PaintMask;
            } else {
                return null;
            }
        }
        set
        {
            if (GoreManager.Instance.GoreEnabled) {
                splatter.PaintMask = value;
                paintMask = value; // keep UI updated
            }
        }
    }
    public Texture2D RenderMask {
        get {
            return renderMask;
        }
        set {
            renderMask = value;
            UpdatePropertyBlock();
        }
    }

    #endregion

    #region Unity Serialization

    [Header ("Splat Stuffs")]
    [SerializeField] private Texture2D paintMask = null;
    [SerializeField] private Texture2D renderMask = null;

    #endregion

    #region Private fields

    private new Renderer renderer;
	private MaterialPropertyBlock propBlock;

    /// <summary>
    /// This queue will contain splats that need to be rendered to the
    /// splattable.
    /// </summary>
    private Queue<Splatter.QueuedSplat> splatsToRender;

    private Splatter splatter;
	#endregion

	#region Unity Lifecycle

	private void Awake ()
	{
        renderMask = renderMask != null ? renderMask : Texture2D.whiteTexture;
        renderer = GetComponent<Renderer>(); // this must be done BEFORE the splatter is created
        if (GoreManager.Instance.GoreEnabled)
        {
            // Set up Splatting
            paintMask = paintMask != null ? paintMask : Texture2D.whiteTexture;
            splatter = CreateSplatter(); // this must be done BEFORE the property block is updated
            PaintMask = paintMask;
            splatsToRender = new Queue<Splatter.QueuedSplat>();
        } else
        {
            enabled = false; // just turn self off.
        }

        // Set Up Property Block
        UpdatePropertyBlock();
    }

    private void Update ()
    {
        // If we have some splat data to render, create a command
        // buffer for it and give it to the GoreManager to do the
        // rendering.
        if (splatsToRender.Count > 0)
        {
            List<Splatter.QueuedSplat> renderedSplats;
            CommandBuffer buffer = splatter.TryRenderSplats(splatsToRender, out renderedSplats);
            if (renderedSplats.Count > 0)
            {
                GoreManager.Instance.AddBloodBuffer(buffer);
                foreach (Splatter.QueuedSplat splat in renderedSplats)
                {
                    if (splat.splatData.HasMoreFrames(splat.frame + 1))
                    {
                        StartCoroutine(DelayFrame(splat));
                    }
                }
            }
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
    public void QueueNewSplat(SplatSO splatData, Vector3 worldPos, Vector2 projectileDir)
    {
        Splatter.QueuedSplat splat = new Splatter.QueuedSplat();
        splat.splatData = splatData;
        splat.frame = 0;
        splat.worldPos = worldPos;
        splat.projectileDir = projectileDir;

        splatsToRender.Enqueue(splat);
    }

    #endregion
    
    #region Private Interface

    /// <summary>
    /// "Creates" a splatter for our use.
    /// </summary>
    /// <returns>A splatter for splatting le blood.</returns>
    private Splatter CreateSplatter()
    {
        if (SystemInfo.supports2DArrayTextures && SystemInfo.graphicsShaderLevel >= 35)
        {
            return new AdvancedSplatter(renderer);
        } else
        {
            return new SimpleSplatter(renderer);
        }
    }

    /// <summary>
    /// Coroutine used to delay adding a frame back into the "toRender" queue.
    /// </summary>
    /// <param name="toQueue">The splat to delay adding back.</param>
    /// <returns>A Coroutine enumerator.</returns>
    private IEnumerator DelayFrame(Splatter.QueuedSplat toQueue)
    {
        yield return new WaitForSeconds(1F / toQueue.splatData.FPS);
        toQueue.frame++; // increment frame
        splatsToRender.Enqueue(toQueue);
    }

    private void UpdatePropertyBlock() {
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        renderer.GetPropertyBlock(propBlock);
        if (GoreManager.Instance.GoreEnabled) {
            propBlock.SetTexture("_SplatMap", splatter.SplatMap);
        } else {
            propBlock.SetTexture("_SplatMap", Texture2D.blackTexture);
        }
        propBlock.SetTexture("_RenderMask", renderMask);
        renderer.SetPropertyBlock(propBlock);
    }

    #endregion
}
