using UnityEngine;

[ExecuteInEditMode]
public class EightiesFilter : MonoBehaviour {

    #region Fields (Unity Serialization)

    [Header ("SET THIS TO ENABLE FILTERING (SHOULD BE '80sFilter.shader'!)")]
    [SerializeField]
    private Material material;

    #endregion

    #region Interface (Unity Lifecycle)

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetTexture("_SourceImage", source);
            Graphics.Blit(source, destination, material);
        } else
        {
            Graphics.Blit(source, destination);
        }
    }

    #endregion
}
