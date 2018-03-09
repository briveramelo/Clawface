using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class UseColorPaletteBase : MonoBehaviour
{
	[SerializeField] protected ColorPalette palette;
    [SerializeField] protected ColorPalette.ColorType type;
    [SerializeField] protected UpdateTime updateTime;

    protected Color TargetColor 
    {
        get
        {
            switch (type) {
                case ColorPalette.ColorType.Primary:
                    return palette.PrimaryColor;
                case ColorPalette.ColorType.Secondary:
                    return palette.SecondaryColor;
                case ColorPalette.ColorType.Tertiary:
                    return palette.TertiaryColor;
                default:
                    Debug.LogError("That color type ain't supported, boye.");
                    return default(Color);
            }
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
