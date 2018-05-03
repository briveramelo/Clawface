#region Using Statements

using UnityEngine;

#endregion

/// <summary>
/// An abstract class for providing shader data to EightiesFilterOperations.
/// </summary>
public abstract class EightiesFilterData : ScriptableObject
{
    #region Accessors (Public)

    /// <summary>
    /// Gets the Shader associated with this data.
    /// </summary>
    public Shader Shader
    {
        get
        {
            return shader;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [Header("Common")]
    [SerializeField]
    private Shader shader;

    #endregion

    #region Interface (Public)

    /// <summary>
    /// Applies all data points provided by this data object to the provided material.
    /// </summary>
    /// <param name="material">The material to set shader uniforms on.</param>
    public abstract void SetValues(Material material);

    #endregion
}