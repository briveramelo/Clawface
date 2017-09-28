using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;


public abstract class GrabberEditor<T, U, V> : Editor where T: Grabber<U,V> where U : GSheetData where V : GSheetsJSONParser<U>, new() {

    Grabber<U, V> grabberTarget;
    GSheetsMiner gSheetsMinerTarget;
    bool clickedGetData;
    private void OnEnable() {
        grabberTarget = target as Grabber<U, V>;
        gSheetsMinerTarget = grabberTarget.GetComponent<GSheetsMiner>();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!gSheetsMinerTarget.AllFieldsFilled);
        clickedGetData = GUILayout.Button(new GUIContent("Get Data"));
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(10);
        if (clickedGetData) {
            grabberTarget.RequestData();
            //grabberTarget.
        }
        serializedObject.ApplyModifiedProperties();
    }
}