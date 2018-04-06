using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModMan;
using MEC;

public interface IPLESelectable {
    bool IsSelected { get; }
    bool IsFocused { get; }
    void Focus();
    Transform GetTransform { get; }
}

public abstract class PLEItem : EventSubscriber, IPLESelectable {

    [SerializeField] AbsAnim jiggleAnim;
    public Sprite iconPreview;
    
    public GridTile tile;
    public bool IsSelected { get; private set; }
    public bool IsFocused { get; private set; }
    public Transform GetTransform { get { return transform; } }
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
    private MaterialPropertyBlock matPropBlock;
    public MaterialPropertyBlock MatPropBlock {
        get {
            if (matPropBlock == null) {
                matPropBlock = new MaterialPropertyBlock();
            }
            return matPropBlock;
        }
    }
    protected List<Renderer> Renderers {
        get {
            if (renderers == null) {
                renderers = GetComponentsInChildren<Renderer>().ToList();
            }
            return renderers;
        }
    }
    private List<Renderer> renderers;

    protected abstract string ColorTint { get; }

    protected override void Awake() {
        base.Awake();
        startScale = transform.localScale;
        jiggleAnim.OnUpdate = ScaleItem;
    }

    private void ScaleItem(float val) {
        transform.localScale = startScale + startScale * val;
    }

    public virtual void Select(Color selectionColor) {
        Timing.KillCoroutines(CoroutineName);
        jiggleAnim.Animate(CoroutineName);
        SFXManager.Instance.Play(SFXType.PLEPlaceObject, transform.position);
        IsSelected = true;
        MatPropBlock.SetColor(ColorTint, selectionColor);
        Renderers.ForEach(renderer => { renderer.SetPropertyBlock(MatPropBlock); });
    }
    public virtual void Deselect() {
        IsSelected = false;
        IsFocused = false;
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
    public virtual void Focus() {
        IsFocused = true;
    }
}