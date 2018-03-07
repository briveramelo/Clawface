using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PLEItem : MonoBehaviour {

    [SerializeField] protected Color selectedColor;
    protected Color startColor;
    protected List<Renderer> renderers;
    protected MaterialPropertyBlock matPropBlock;
    protected MaterialPropertyBlock MatPropBlock {
        get {
            if (matPropBlock==null) {
                matPropBlock = new MaterialPropertyBlock();
            }
            return matPropBlock;
        }
    }

    protected virtual void Start() {
        renderers = GetComponentsInChildren<Renderer>().ToList();
        startColor = renderers[0].material.GetColor(ColorTint);
    }    

    protected abstract string ColorTint { get; }

    public virtual void Select() {
        MatPropBlock.SetColor(ColorTint, selectedColor);
        renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    public virtual void Deselect() {
        MatPropBlock.SetColor(ColorTint, startColor);
        renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    
}
