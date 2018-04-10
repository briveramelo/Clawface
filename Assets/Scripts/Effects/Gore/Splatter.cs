#region Using Statements

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#endregion

/// <summary>
/// An abstract Splatter.  Used to encapsulate the act of splatting blood onto objects.
/// </summary>
public abstract class Splatter
{
    #region Accessors (Public)

    /// <summary>
    /// Gets the SplatMap if it's still valid.
    /// </summary>
    public RenderTexture SplatMap
    {
        get
        {
            if (splatMap.IsCreated())
            {
                return splatMap;
            } else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the Paint Mask.
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

    #region Fields (Protected)

    /// <summary>
    /// The SplatMat Material used to paint splats.
    /// </summary>
    protected readonly Material splatMat;

    /// <summary>
    /// The renderer we're using to splat with.
    /// </summary>
    protected readonly Renderer renderer;

    /// <summary>
    /// The dims of our splat map.
    /// </summary>
    protected readonly Dims dims;

    /// <summary>
    /// Our splat map.  This is where splats will go.
    /// </summary>
    protected readonly RenderTexture splatMap;

    /// <summary>
    /// The paint mask which is used to limit where a splatter can splat things.
    /// </summary>
    protected Texture2D paintMask;

    #endregion

    #region Constructors (Public)

    /// <summary>
    /// Constructs this Splatter.
    /// </summary>
    /// <param name="splatShader"></param>
    /// <param name="renderer"></param>
    public Splatter (Shader splatShader, Renderer renderer)
    {
        splatMat = new Material(splatShader);
        this.renderer = renderer;
        dims = GetRenderDims();
        splatMap = new RenderTexture(dims.Width, dims.Height, 0, RenderTextureFormat.ARGB32);
        splatMap.Create();

        // Clear the RenderTexture.. just in case
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = splatMap;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        RenderTexture.active = old;
    }

    #endregion

    #region Interface (Public)

    /// <summary>
    /// Tries to render splats to a texture by returning a CommandBuffer that can render the splats.
    /// </summary>
    /// <param name="splatsToRender"></param>
    /// <param name="renderedSplats"></param>
    /// <returns></returns>
    public abstract CommandBuffer TryRenderSplats(Queue<QueuedSplat> splatsToRender, out List<QueuedSplat> renderedSplats);

    #endregion

    #region Interface (Private)

    /// <summary>
    /// Gets the render dims to use.
    /// </summary>
    /// <returns></returns>
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

    #region Types (Public)

    /// <summary>
    /// This struct is used to track data being rendered by the Splattable.
    /// We have to queue this data up and render it in batches since there
    /// could potentially be multiple sets of data being rendered to same tile
    /// every frame.
    /// </summary>
    public struct QueuedSplat
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

    #region Types (Protected)

    /// <summary>
    /// The render dims defining the size of our splat map.
    /// </summary>
    protected struct Dims
    {
        #region Accessors (Public)

        /// <summary>
        /// Gets the width of the splat map.
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// Gets the height of the splat map.
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
        }

        #endregion

        #region Fields (Private)

        /// <summary>
        /// The width of the splat map.
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the splat map.
        /// </summary>
        private int height;

        #endregion

        #region Constructors (Public)

        /// <summary>
        /// Constructs this Dims object with the provided parameters.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Dims(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        #endregion
    }

    #endregion
}