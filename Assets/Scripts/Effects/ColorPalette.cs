using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Color Palette", menuName="Color Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] Color colorPrimary;
    [SerializeField] Color colorSecondary;

    public Color PrimaryColor { get { return colorPrimary; } }
    public Color SecondaryColor { get { return colorSecondary; } }

    public enum ColorType
    {
        Primary,
        Secondary
    }
}
