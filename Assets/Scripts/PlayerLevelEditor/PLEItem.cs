using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PLEItem : MonoBehaviour {

    [SerializeField] MeshRenderer meshRenderer;
    protected MeshRenderer myMeshRenderer;
    protected MaterialPropertyBlock matPropBlock;
    protected MaterialPropertyBlock MatPropBlock {
        get {
            if (matPropBlock==null) {
                matPropBlock = new MaterialPropertyBlock();
            }
            return matPropBlock;
        }
    }    

    const string AlbedoTint = "_AlbedoTint";

    public virtual void Select() {
        MatPropBlock.SetColor(AlbedoTint, Color.blue);
        meshRenderer.SetPropertyBlock(MatPropBlock);
    }
    public virtual void Deselect() {
        MatPropBlock.SetColor(AlbedoTint, Color.white);
        meshRenderer.SetPropertyBlock(MatPropBlock);
    }
    
}
