#region Using Statements

using UnityEngine;

#endregion

/// <inheritdoc />
/// <summary>
/// Provides the VignetteData for the Vignette shader.
/// </summary>
[CreateAssetMenu(fileName = "VignetteData", menuName = "Hathos/80s Filter/Vignette")]
public class VignetteData : EightiesFilterData
{
    #region Fields (Unity Serialization)

    /// <summary>
    /// The tint of the Vignette.
    /// </summary>
    [Header("Vignette - THIS CLASS IS CURRENTLY NOT USED!!!")]
    [SerializeField]
    private Color tint = Color.black;

    /// <summary>
    /// The range from the center of the screen where the vignette becomes black.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float distance = 0.8F;

    /// <summary>
    /// A dampening parameter for the vignette itself.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float softness = 0.45F;

    /// <summary>
    /// The strength with which to apply the vignette.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float strength = 0.5F;

    #endregion

    #region Interface (EightiesFilterData)

    /// <inheritdoc />
    public override void SetValues(Material material)
    {
        material.SetColor("_Tint", tint);
        material.SetFloat("_Distance", distance);
        material.SetFloat("_Softness", softness);
        material.SetFloat("_Strength", strength);
    }

    #endregion
}