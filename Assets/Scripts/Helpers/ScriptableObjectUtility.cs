//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public static class ScriptableObjectUtility {

//    public static void SaveScriptableObject<T> (this T asset, string assetPath, bool forceOverwrite=true) where T : ScriptableObject {
//        var existingAsset = AssetDatabase.LoadMainAssetAtPath (assetPath);
//        if (forceOverwrite) {
//            if (existingAsset) EditorUtility.CopySerialized (asset, existingAsset);
//            else AssetDatabase.CreateAsset (asset, assetPath);
//            AssetDatabase.SaveAssets();
//        }

//		AssetDatabase.Refresh();
//		EditorUtility.FocusProjectWindow();
//    }

//	public static string SaveScriptableObjectWithFilePanel<T> (this T asset, string filePanelMessage, string defaultName, string fileExtension) where T : ScriptableObject {
//		var assetPath = EditorUtility.SaveFilePanelInProject (filePanelMessage, defaultName, fileExtension, "");

//		if (assetPath == default(string) || assetPath == "") return default(string);

//		SaveScriptableObject<T> (asset, assetPath);

//        return assetPath;
//	}

//    public static T LoadScriptableObject<T> (string assetPath) where T : ScriptableObject {
//        T asset = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
//		if (asset == default(T)) {
//			Debug.LogError ("Failed to load " + typeof(T).ToString() + " asset at " + assetPath);
//			return default(T);
//		}

//		return asset;
//    }

//	public static T LoadScriptableObjectWithFilePanel<T> (string filePanelMessage, string initialFolderName, string fileExtension) where T : ScriptableObject {
//		var assetPath = EditorUtility.OpenFilePanel (filePanelMessage, initialFolderName, fileExtension);
//		if (assetPath == default(string) || assetPath == "") return default(T);

//		if (assetPath.StartsWith(Application.dataPath)) assetPath = "Assets" + assetPath.Substring (Application.dataPath.Length);

//		return LoadScriptableObject<T> (assetPath);
//	}

//    public static T LoadScriptableObjectWithFilePanel<T> (string filePanelMessage, string initialFolderName, string fileExtension, out string path) where T : ScriptableObject {
//        path = EditorUtility.OpenFilePanel (filePanelMessage, initialFolderName, fileExtension);
//		if (path == default(string) || path == "") return default(T);

//		if (path.StartsWith(Application.dataPath)) path = "Assets" + path.Substring (Application.dataPath.Length);

//		return LoadScriptableObject<T> (path);
//    }
//}
