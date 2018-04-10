using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIStandards))]
public class UIStandardsEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Apply")) {
            (target as UIStandards).ApplyFontSizes();
        }
    }
}
