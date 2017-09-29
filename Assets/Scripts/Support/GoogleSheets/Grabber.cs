using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(GSheetsDeliveryBoy),typeof(GSheetsMiner))]
public abstract class Grabber<T, V, U> : MonoBehaviour where T: GSheetData where V: GSheetScriptable<T> where U : GSheetsJSONParser<T>, new() {

    [SerializeField] GSheetsDeliveryBoy deliveryBoy;
    public List<T> myDataList;

    public void RequestData() {
        deliveryBoy.Request<T>(OnGetData);
    }

    List<T> OnGetData(string json) {
        myDataList = GSheetsDeliveryBoy.Get<T, U>(json);
        return myDataList;
    }

    public void CreateScriptableObject() {                        
        V asset = ScriptableObject.CreateInstance<V>();
        string sampleFileName = typeof(V).ToString();
        string pathName = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}{1}{2}","Assets/ScriptableObjects/", sampleFileName, ".asset"));
        AssetDatabase.CreateAsset(asset, pathName);
        asset.dataList = myDataList;
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
        //EditorUtility.FocusProjectWindow();

        
    }
}


[System.Serializable]
public class GSheetData {

}

[System.Serializable]
public class GSheetScriptable<T> : ScriptableObject where T:GSheetData {
    public List<T> dataList;
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