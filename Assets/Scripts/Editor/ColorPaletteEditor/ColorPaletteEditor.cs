using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColorPaletteEditor : EditorWindow
{
    const string COLOR_PALETTES_PATH = "ColorPalettes";

    static ColorPaletteEditor Instance;

    Vector2 scrollPos = Vector2.zero;

    ColorPalette[] palettes;

    [MenuItem("Window/Color Palette Editor")]
	static void Init ()
    {
        Instance = GetWindow<ColorPaletteEditor>("Color Palette Editor");
        Instance.Show();
        Instance.LoadColorPalettes();
    }

    private void OnEnable()
    {
        LoadColorPalettes();
    }

    private void OnFocus()
    {
        LoadColorPalettes();
    }

    void LoadColorPalettes ()
    {
        string path = COLOR_PALETTES_PATH;
        palettes = Resources.LoadAll<ColorPalette>(path);
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("Color Palette Editor", EditorStyles.largeLabel);
        EditorGUILayout.Separator();

        if (palettes == null || palettes.Length == 0)
        {
            EditorGUILayout.LabelField("No color palettes found.", EditorStyles.boldLabel);
        }

        else
        {
            foreach (ColorPalette colorPalette in palettes)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField(colorPalette.name, EditorStyles.boldLabel);

                SerializedObject obj = new SerializedObject(colorPalette);
                SerializedProperty pri = obj.FindProperty("colorPrimary");
                SerializedProperty sec = obj.FindProperty("colorSecondary");

                EditorGUILayout.PropertyField(pri);
                EditorGUILayout.PropertyField(sec);

                obj.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
