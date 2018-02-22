using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PLEItem : MonoBehaviour {

    protected MeshRenderer myMeshRenderer;
    protected MaterialPropertyBlock matPropBlock;
    const string AlbedoTint = "_AlbedoTint";
    private void Awake() {
        myMeshRenderer = GetComponent<MeshRenderer>();
        matPropBlock = new MaterialPropertyBlock();
        myMeshRenderer.GetPropertyBlock(matPropBlock);
    }

    public virtual void Select() {
        matPropBlock.SetColor(AlbedoTint, Color.blue);        
        myMeshRenderer.SetPropertyBlock(matPropBlock);
    }
    public virtual void Deselect() {
        matPropBlock.SetColor(AlbedoTint, Color.white);
        myMeshRenderer.SetPropertyBlock(matPropBlock);
    }
    
}
