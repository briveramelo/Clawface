using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Color Palette", menuName="Color Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] Color colorPrimary;
    [SerializeField] Color colorSecondary;
    [SerializeField] Color colorTertiary;

    public Color GetColor (ColorType type)
    {
        switch (type)
        {
            case ColorType.Primary:
                return colorPrimary;
            case ColorType.Secondary:
                return colorSecondary;
            case ColorType.Tertiary:
                return colorTertiary;
            default:
                return default(Color);
        }
    }

    public Color PrimaryColor { get { return colorPrimary; } }
    public Color SecondaryColor { get { return colorSecondary; } }
    public Color TertiaryColor { get { return colorTertiary; } }

    public enum ColorType
    {
        Primary,
        Secondary,
        Tertiary
    }
}
