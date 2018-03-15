using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModMan;
public abstract class PLEItem : MonoBehaviour {

    public Sprite iconPreview;
    protected Color startColor;
    protected Color StartColor {
        get {
            if (startColor.IsAboutEqual(Color.clear)) {
                startColor = Renderers[0].material.GetColor(ColorTint);
            }
            return startColor;
        }
    }
    public bool IsSelected { get; private set; }
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
        
    }    

    protected abstract string ColorTint { get; }

    public virtual void Select(Color selectionColor) {
        IsSelected = true;
        MatPropBlock.SetColor(ColorTint, selectionColor);
        Renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    public virtual void Deselect() {
        IsSelected = false;
        MatPropBlock.SetColor(ColorTint, StartColor);
        Renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    public virtual void TryHighlight(Color highlightColor) {
        if (!IsSelected) {
            MatPropBlock.SetColor(ColorTint, highlightColor);
            Renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
        }
    }
    public virtual void TryUnHighlight() {
        if (!IsSelected) {
            Deselect();
        }
    }
}
