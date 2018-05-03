﻿#region Using Statements

using UnityEngine;

#endregion

/// <inheritdoc />
/// <summary>
/// Provides the BarrelDistortionData for the BarrelDistortion shader.
/// </summary>
[CreateAssetMenu(fileName = "BarrelDistortionData",
    menuName = "Hathos/80s Filter/Barrel Distortion")]
public class BarrelDistortionData : EightiesFilterData
{
    #region Fields (Unity Serialization)

    /// <summary>
    /// The amount of distortion to apply.
    /// </summary>
    [Header("Barrel Distortion")]
    [SerializeField]
    [Range(0, 1)]
    private float distortion = 0.015F;

    #endregion

    #region Interface (EightiesFilterData)

    /// <inheritdoc />
    public override void SetValues(Material material)
    {
        material.SetFloat("_Distortion", distortion);
    }

    #endregion
}