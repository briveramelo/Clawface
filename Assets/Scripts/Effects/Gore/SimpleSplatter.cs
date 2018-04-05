#region using Statements

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#endregion

/// <summary>
/// A simple Splatter.  Renders at most ONE splat per frame.
/// </summary>
public class SimpleSplatter : Splatter
{
    #region Constructors (Public)

    /// <summary>
    /// Constructs this Simple Splatter.
    /// </summary>
    /// <param name="renderer"></param>
    public SimpleSplatter(Renderer renderer) : base(GoreManager.Instance.SimpleSplatShader, renderer)
    {
        // Nothing to do.
    }

    #endregion

    #region Interface (Splatter)

    /// </inheritdoc>
    public override CommandBuffer TryRenderSplats(Queue<QueuedSplat> splatsToRender, out List<QueuedSplat> renderedSplats)
    {
        renderedSplats = new List<QueuedSplat>();
        QueuedSplat splat = splatsToRender.Dequeue();
        renderedSplats.Add(splat);

        // Set Shader Variables
        SplatSO data = splat.splatData;
        SplatSO.Frame frame = data.Frames[splat.frame];

        splatMat.SetTexture("_Previous", splatMap);
        splatMat.SetTexture("_PaintMask", paintMask);
        splatMat.SetTexture("_Mask", frame.mask);
        splatMat.SetTexture("_Normal", frame.normal);
        splatMat.SetVector("_Anchor", data.Anchor);
        splatMat.SetVector("_Rotation", data.CalculateRotation(splat.projectileDir));
        splatMat.SetVector("_Position", splat.worldPos);

        // Set Up Command Buffer
        CommandBuffer splatBuffer = new CommandBuffer();
        splatBuffer.GetTemporaryRT(Shader.PropertyToID("_TEMPORARY"), dims.Width, dims.Height);
        splatBuffer.SetRenderTarget(Shader.PropertyToID("_TEMPORARY"));
        splatBuffer.DrawRenderer(renderer, splatMat);
        splatBuffer.Blit(Shader.PropertyToID("_TEMPORARY"), splatMap);
        return splatBuffer;
    }

    #endregion
}