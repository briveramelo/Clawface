using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseColorPaletteBase : MonoBehaviour
{
	[SerializeField] protected ColorPalette palette;
    [SerializeField] protected ColorPalette.ColorType type;
    [SerializeField] protected UpdateTime updateTime;

    protected Color TargetColor 
    {
        get
        {
            return type == ColorPalette.ColorType.Primary ? palette.PrimaryColor : palette.SecondaryColor;
        }
    }

    public abstract void UpdateColor ();

    private void Awake()
    {
        if (updateTime == UpdateTime.Awake)
            UpdateColor();
    }

    private void Start()
    {
        if (updateTime == UpdateTime.Start)
            UpdateColor();
    }

    private void OnEnable()
    {
        if (updateTime == UpdateTime.OnEnable)
            UpdateColor();
    }

    public enum UpdateTime
    {
        Awake,
        Start,
        OnEnable
    }
}
