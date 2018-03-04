using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UseColorPaletteBase), true)]
public class UseColorPaletteBaseEditor : Editor
{
    UseColorPaletteBase _target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _target = target as UseColorPaletteBase;

        if (GUILayout.Button("Update"))
            _target.UpdateColor();
    }
}
