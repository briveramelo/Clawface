using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCubeController : MonoBehaviour {

    #region Private Fields

    private Material cloneMat;

    #endregion

    #region Public Fields

    public bool selected = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        cloneMat = new Material(GetComponent<MeshRenderer>().material);
        cloneMat.CopyPropertiesFromMaterial(GetComponent<MeshRenderer>().material);
        GetComponent<MeshRenderer>().material = cloneMat;
    }

    #endregion

    #region Public Interface

    public void SetColor(Color i_Color)
    {

        cloneMat.SetColor("_TintColor",i_Color);

    }

    public void ResetColor()
    {
        cloneMat.SetColor("_TintColor", Color.white);
    }

    #endregion


}
