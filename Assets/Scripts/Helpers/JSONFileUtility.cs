using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class JSONFileUtility {

	public static void SaveToJSONFile (object obj, string path) {
        var json = JsonUtility.ToJson(obj);
        File.WriteAllText(path, json);
    }

    public static T LoadFromJSONFile<T> (string path) {
        var json = File.ReadAllText (path);
        return JsonUtility.FromJson<T>(json);
    }
}
