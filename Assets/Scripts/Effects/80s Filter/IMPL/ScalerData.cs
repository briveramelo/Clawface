﻿#region Using Statements

using UnityEngine;

#endregion

/// <inheritdoc />
/// <summary>
/// Provides the ScalerData for the Scaler shader.
/// </summary>
[CreateAssetMenu(fileName = "ScalerData", menuName = "Hathos/80s Filter/Scaler")]
public class ScalerData : EightiesFilterData
{
    #region Fields (Unity Serialization)

    /// <summary>
    /// The scale of the final image.
    /// </summary>
    [Header("Scaler")]
    [SerializeField]
    private float scale = 1F;

    #endregion

    #region Interface (EightiesFilterData)

    /// <inheritdoc />
    public override void SetValues(Material material)
    {
        material.SetFloat("_Scale", scale);
    }

    #endregion
}