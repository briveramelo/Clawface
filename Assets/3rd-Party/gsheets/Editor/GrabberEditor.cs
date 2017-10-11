using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public abstract class DataGrabberEditor<X, T, V, U> : Editor where X : DataGrabber<T, U> where T : GSheetData where V : GSheetScriptable<T> where U : GSheetsJSONParser<T>, new() {

    DataGrabber<T, U> grabberTarget;
    GSheetsMiner gSheetsMinerTarget;
    bool clickedGetData;
    private void OnEnable() {
        grabberTarget = target as DataGrabber<T, U>;
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
            CreateScriptableObject();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(10);
        serializedObject.ApplyModifiedProperties();
    }

    void CreateScriptableObject() {
        V asset = ScriptableObject.CreateInstance<V>();                
        string sampleFileName = typeof(V).ToString();
        string pathName = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}{1}{2}", "Assets/ScriptableObjects/", sampleFileName, ".asset"));
        AssetDatabase.CreateAsset(asset, pathName);
        asset.dataList = grabberTarget.myDataList;
        AssetDatabase.SaveAssets();
    }
}
//T GSheet Data
//V GSHeet Scriptable
//U GSheetsJSONParser
//X Grabber<T,V,U>

public static class SOUtilities {
    public static void CreateAsset<T>() where T : ScriptableObject {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "") {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "") {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}