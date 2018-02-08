using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColorPaletteEditor : EditorWindow
{
    static ColorPaletteEditor Instance;

    [MenuItem("Window/Color Palette Editor")]
	static void Init ()
    {
        Instance = GetWindow<ColorPaletteEditor>("Color Palette Editor");
        Instance.Show();
    }

    private void OnGUI()
    {
        //EditorGUILayout.ColorField ("Enemy Outline Color");
    }
}
