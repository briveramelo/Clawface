using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FontSizer)), CanEditMultipleObjects]
public class FontSizerEditor : Editor {

    private void OnEnable() {
        (target as FontSizer).TrySetTextFromTextComponent();
        (target as FontSizer).Apply();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        (target as FontSizer).Apply();
    }
}
