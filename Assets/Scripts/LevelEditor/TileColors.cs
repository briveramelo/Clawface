using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class TileColors : MonoBehaviour {

    [SerializeField] Color green, blue, pink, purple, yellow, red;
    static List<ColorType> colorTypes = new List<ColorType>();

    void Awake() {
        Green = green;
        Blue = blue;
        Pink = pink;
        Purple = purple;
        Yellow = yellow;
        Red = red;

        colorTypes = new List<ColorType>() {
            new ColorType(Green, GREEN),
            new ColorType(Blue, BLUE),
            new ColorType(Pink, PINK),
            new ColorType(Purple, PURPLE),
            new ColorType(Yellow, YELLOW),
            new ColorType(Red, RED),
        };
    }


    public static Color Green { get; private set; }
    public static Color Blue { get; private set; }
    public static Color Pink { get; private set; }
    public static Color Purple { get; private set; }
    public static Color Yellow { get; private set; }
    public static Color Red { get; private set; }

    public const int GREEN = 0;
    public const int BLUE = 1;
    public const int PINK = 2;
    public const int PURPLE = 3;
    public const int YELLOW = 4;
    public const int RED = 5;

    public static Color GetColor(int tileType) {
        ColorType colorType = colorTypes.Find(type => type.type == tileType);
        if (colorType!=null) {
            return colorType.color;
        }
        return Green;
    }
    public static int GetType(Color color) {
        ColorType colorType = colorTypes.Find(type => type.color.IsAboutEqual(color));
        if (colorType != null) {
            return colorType.type;
        }
        return GREEN;
    }
}

[System.Serializable]
public class ColorType {
    public ColorType(Color color, int type) {
        this.color = color;
        this.type = type;
    }
    public int type;
    public Color color;
}