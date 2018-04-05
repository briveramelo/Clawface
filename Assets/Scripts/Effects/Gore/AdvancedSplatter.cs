#region Using Statements

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#endregion

/// <summary>
/// The "advanced" Splatter.  Uses Texture2DArrays to speed operation.
/// </summary>
public class AdvancedSplatter : Splatter
{
    #region Fields (Private)

    /// <summary>
    /// The masks to render with.  Cached for performance.
    /// </summary>
    private Texture2DArray masks;

    /// <summary>
    /// The normals to render with.  Cached for performance.
    /// </summary>
    private Texture2DArray normals;

    #endregion

    #region Constructors (Public)

    /// <summary>
    /// Constructs this Advanced Splatter.
    /// </summary>
    /// <param name="renderer"></param>
    public AdvancedSplatter(Renderer renderer) : base(GoreManager.Instance.AdvancedSplatShader, renderer)
    {
        Vector4[] empty = new Vector4[GoreManager.Instance.NumSplatsToRender];
        splatMat.SetVectorArray("_Anchors", empty);
        splatMat.SetVectorArray("_Rotations", empty);
        splatMat.SetVectorArray("_Positions", empty);
    }

    #endregion

    #region Interface (Splatter)

    /// </inheritdoc>
    public override CommandBuffer TryRenderSplats(Queue<QueuedSplat> splatsToRender, out List<QueuedSplat> renderedSplats)
    {
        List<Texture2D> masks = new List<Texture2D>();
        List<Texture2D> normals = new List<Texture2D>();
        List<Vector4> anchors = new List<Vector4>(); // must be vec4 in order to set array
        List<Vector4> rotations = new List<Vector4>(); // must be vec4 in order to set array
        List<Vector4> positions = new List<Vector4>(); // must be vec4 in order to set array

        renderedSplats = new List<QueuedSplat>();
        int count = 0;
        while (splatsToRender.Count > 0 && count < GoreManager.Instance.NumSplatsToRender)
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

            renderedSplats.Add(splat);
        }

        // Set Shader Variables
        splatMat.SetTexture("_Previous", splatMap);
        splatMat.SetTexture("_PaintMask", paintMask);
        splatMat.SetTexture("_Masks", CreateTextureArray(masks, ref this.masks));
        splatMat.SetTexture("_Normals", CreateTextureArray(normals, ref this.normals));
        splatMat.SetVectorArray("_Anchors", anchors);
        splatMat.SetVectorArray("_Rotations", rotations);
        splatMat.SetVectorArray("_Positions", positions);
        splatMat.SetInt("_Count", count);

        // Set Up Command Buffer
        CommandBuffer splatBuffer = new CommandBuffer();
        splatBuffer.GetTemporaryRT(Shader.PropertyToID("_TEMPORARY"), dims.Width, dims.Height);
        splatBuffer.SetRenderTarget(Shader.PropertyToID("_TEMPORARY"));
        splatBuffer.DrawRenderer(renderer, splatMat);
        splatBuffer.Blit(Shader.PropertyToID("_TEMPORARY"), splatMap);
        return splatBuffer;
    }

    #endregion

    #region Interface (Private)
    
    /// <summary>
    /// Creates a Texture2DArray from a list of 2D textures.
    /// </summary>
    /// <param name="textures">The List of Texture2D objects to pack in an array.</param>
    /// <returns>A Texture2DArray containing the textures in the argument list.</returns>
    private Texture2DArray CreateTextureArray(List<Texture2D> textures, ref Texture2DArray array)
    {
        GoreManager manager = GoreManager.Instance;

        if (array == null)
        {
            array = new Texture2DArray(Mathf.FloorToInt(manager.FrameDim.x), Mathf.FloorToInt(manager.FrameDim.y),
                manager.NumSplatsToRender, textures[0].format, true);
        }

        for (int index = 0; index < textures.Count; index++)
        {
            Graphics.CopyTexture(textures[index], 0, array, index);
        }
        return array;
    }

    #endregion
}