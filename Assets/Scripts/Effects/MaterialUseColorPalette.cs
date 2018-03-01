using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialUseColorPalette : UseColorPaletteBase
{
    [SerializeField] string colorName;
    [SerializeField] int materialIndex;
    
    public override void UpdateColor ()
    {
        Renderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = GetComponent<SkinnedMeshRenderer>();

        Material mat = meshRenderer.sharedMaterials[materialIndex];
        mat.SetColor (colorName, TargetColor);
    }
}
