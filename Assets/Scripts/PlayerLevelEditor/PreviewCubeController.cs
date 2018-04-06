using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCubeController : MonoBehaviour {

    #region Private Fields

    private Material cloneMat;
    private Color startColor;
    #endregion

    #region Public Fields

    public bool selected = false;

    #endregion
    private const string BlockColor = "_Color";

    #region Unity Lifecycle
    
    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        cloneMat = new Material(meshRenderer.material);
        cloneMat.CopyPropertiesFromMaterial(meshRenderer.material);
        meshRenderer.material = cloneMat;
        startColor = cloneMat.color;
    }

    #endregion

    #region Public Interface

    public void SetColor(Color i_Color) {
        cloneMat.SetColor(BlockColor, i_Color);
    }

    public void ResetColor() {
        cloneMat.SetColor(BlockColor, startColor);
    }

    #endregion


}
