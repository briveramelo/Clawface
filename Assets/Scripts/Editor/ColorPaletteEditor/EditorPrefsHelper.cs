using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorPrefsHelper
{
    const string COLOR_R_EXTENSION = "_R";
    const string COLOR_G_EXTENSION = "_G";
    const string COLOR_B_EXTENSION = "_B";
    const string COLOR_A_EXTENSION = "_A";

    public static bool HasColor (string colorKey)
    {
        return EditorPrefs.HasKey (colorKey + COLOR_R_EXTENSION) &&
               EditorPrefs.HasKey (colorKey + COLOR_G_EXTENSION) &&
               EditorPrefs.HasKey (colorKey + COLOR_B_EXTENSION) &&
               EditorPrefs.HasKey (colorKey + COLOR_A_EXTENSION);
    }

    public static void SetColor (string colorKey, Color color)
    {
        EditorPrefs.SetFloat (colorKey + COLOR_R_EXTENSION, color.r);
        EditorPrefs.SetFloat (colorKey + COLOR_G_EXTENSION, color.g);
        EditorPrefs.SetFloat (colorKey + COLOR_B_EXTENSION, color.b);
        EditorPrefs.SetFloat (colorKey + COLOR_A_EXTENSION, color.a);
    }

    public static Color GetColor (string colorKey)
    {
        return new Color (
            EditorPrefs.GetFloat(colorKey + COLOR_R_EXTENSION),
            EditorPrefs.GetFloat(colorKey + COLOR_G_EXTENSION),
            EditorPrefs.GetFloat(colorKey + COLOR_B_EXTENSION),
            EditorPrefs.GetFloat(colorKey + COLOR_A_EXTENSION)
            );
    }
}
