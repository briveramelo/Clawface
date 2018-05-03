#region Using Statements

using UnityEngine;

#endregion

/// <inheritdoc />
/// <summary>
/// Provides the FilmGrainData for the FilmGrain shader.
/// </summary>
[CreateAssetMenu (fileName = "FilmGrainData", menuName = "Hathos/80s Filter/Film Grain")]
public class FilmGrainData : EightiesFilterData {

    #region Accessors (EightiesFilterData)

    /// <inheritdoc />
    public override string ShaderName
    {
        get
        {
            return "Hathos/Hidden/80sFilter/FilmGrain";
        }
    }
    
    #endregion
    
    #region Fields (Unity Serialization)

    /// <summary>
    ///  The tint of the film grain.
    /// </summary>
    [SerializeField]
    private Color tint = new Color(0.2F, 0.2F, 0.2F);

    /// <summary>
    /// How heavily to apply the film grain.
    /// </summary>
    [SerializeField]
    [Range (0, 1)]
    private float weight = 0.2F;

    /// <summary>
    /// A scaling parameter for sampling the noise.
    /// </summary>
    [SerializeField]
    private float scale = 10;
    
    #endregion
    
    #region Interface (EightiesFilterData)

    /// <inheritdoc />
    public override void SetValues(Material material)
    {
        material.SetFloat("_RealTime", Time.realtimeSinceStartup);
        material.SetColor("_Tint", tint);
        material.SetFloat("_Weight", weight);
        material.SetFloat("_Scale", scale);
    }
    
    #endregion
}
