using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;


public abstract class GrabberEditor<X, T, V, U> : Editor where X: Grabber<T, V, U> where T : GSheetData where V: GSheetScriptable<T> where U : GSheetsJSONParser<T>, new() {

    Grabber<T, V, U> grabberTarget;
    GSheetsMiner gSheetsMinerTarget;
    bool clickedGetData;
    private void OnEnable() {
        grabberTarget = target as Grabber<T, V, U>;
        gSheetsMinerTarget = grabberTarget.GetComponent<GSheetsMiner>();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!gSheetsMinerTarget.AllFieldsFilled);
            if (GUILayout.Button(new GUIContent("Get Data"))) {
                grabberTarget.RequestData();
            }
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Create Scriptable Object"))) {
                grabberTarget.CreateScriptableObject();
            }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(10);
        serializedObject.ApplyModifiedProperties();
    }
}
//T GSheet Data
//V GSHeet Scriptable
//U GSheetsJSONParser
//X Grabber<T,V,U>