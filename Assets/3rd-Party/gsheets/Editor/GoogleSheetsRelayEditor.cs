using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GSheetsMiner)), CanEditMultipleObjects]
public class GSheetsMinerEditor : Editor {

    GSheetsMiner googleSheetsTarget;
    bool clickedGetSheetData;
    private void OnEnable() {
        googleSheetsTarget = target as GSheetsMiner;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        GUILayout.Space(10);
        GUILayout.Label("URL");
        EditorGUI.BeginDisabledGroup(true);
        GUILayout.TextArea(googleSheetsTarget.AllFieldsFilled ? googleSheetsTarget.Url : "Complete all fields");
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(10);
        base.OnInspectorGUI();
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!googleSheetsTarget.AllFieldsFilled);
        clickedGetSheetData = GUILayout.Button(new GUIContent("Print Sheet Data"));
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(10);
        if (clickedGetSheetData) {
            Func<string, string> getJson = (json) => { Debug.Log(json); return json; };
            Debug.Log("clicked");
            googleSheetsTarget.GetSheetData(getJson);
        }
        serializedObject.ApplyModifiedProperties();
    }
}