#region Using Statements

using UnityEngine;

#endregion

/// <inheritdoc />
/// <summary>
/// Provides the ScanlinesData for the Scanlines shader.
/// </summary>
[CreateAssetMenu(fileName = "ScanlinesData", menuName = "Hathos/80s Filter/Scanlines")]
public class ScanlinesData : EightiesFilterData
{
    #region Fields (Unity Serialization)

    /// <summary>
    /// The width of the scanlines in UV Space.
    /// </summary>
    [Header("Scanlines")]
    [SerializeField]
    [Range(0, 1)]
    private float width = 0.05F;

    /// <summary>
    /// The frequency of the scanlines in UV Space.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float frequency = 0.01F;

    /// <summary>
    /// The color to tint the Scanlines.
    /// </summary>
    [SerializeField]
    private Color tint = new Color(0.2F, 0.2F, 0.2F);

    /// <summary>
    /// How heavily to apply the tint.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float weight = 0.2F;

    #endregion

    #region Interface (EightiesFilterData)

    /// <inheritdoc />
    public override void SetValues(Material material)
    {
        material.SetFloat("_Width", width);
        material.SetFloat("_Frequency", frequency);
        material.SetColor("_Tint", tint);
        material.SetFloat("_Weight", weight);
    }

    #endregion
}