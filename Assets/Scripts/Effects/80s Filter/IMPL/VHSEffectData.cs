#region Using Statements

using UnityEngine;

#endregion

/// <inheritdoc />
/// <summary>
/// Provides the VHSEffectData for the VHSEffect shader.
/// </summary>
[CreateAssetMenu(fileName = "VHSEffect", menuName = "Hathos/80s Filter/VHS Effect")]
public class VHSEffectData : EightiesFilterData
{
    #region Fields (Unity Serialization)

    /// <summary>
    /// The width of the VHS scroll bar.
    /// </summary>
    [Header("VHS Effect")]
    [SerializeField]
    [Range(0, 1)]
    private float width = 0.1F;

    /// <summary>
    /// The speed at which the scroll bar moves.
    /// </summary>
    [SerializeField]
    private float speed = 0.1F;

    #endregion

    #region Interface (EightiesFilterData)

    /// <inheritdoc />
    public override void SetValues(Material material)
    {
        material.SetFloat("_RealTime", Time.realtimeSinceStartup);
        material.SetFloat("_Width", width);
        material.SetFloat("_Speed", speed);
    }

    #endregion
}