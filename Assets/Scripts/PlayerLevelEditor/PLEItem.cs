using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PLEItem : MonoBehaviour {

    [SerializeField] protected Color selectedColor;
    protected Color startColor;
    protected List<Renderer> Renderers {
        get {
            if (renderers == null) {
                renderers = GetComponentsInChildren<Renderer>().ToList();
            }
            return renderers;
        }
    }
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
        startColor = Renderers[0].material.GetColor(ColorTint);
    }    

    protected abstract string ColorTint { get; }

    public virtual void Select() {
        MatPropBlock.SetColor(ColorTint, selectedColor);
        Renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    public virtual void Deselect() {
        MatPropBlock.SetColor(ColorTint, startColor);
        Renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    
}
