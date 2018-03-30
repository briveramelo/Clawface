using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModMan;
using MEC;
public abstract class PLEItem : EventSubscriber {

    [SerializeField] AbsAnim jiggleAnim;
    public Sprite iconPreview;
    private Vector3 startScale;
    protected Color startColor;
    protected Color StartColor {
        get {
            if (startColor.IsAboutEqual(Color.clear)) {
                startColor = Renderers[0].material.GetColor(ColorTint);
            }
            return startColor;
        }
    }
    public GridTile tile;
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

    protected abstract string ColorTint { get; }



    protected override void Awake() {
        base.Awake();
        startScale = transform.localScale;
        jiggleAnim.OnUpdate = ScaleItem;
    }

    void ScaleItem(float val) {
        transform.localScale = startScale + startScale * val;
    }

    public virtual void Select(Color selectionColor) {
        Timing.KillCoroutines(coroutineName);
        jiggleAnim.Animate(coroutineName);
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
