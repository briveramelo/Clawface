// JSONFileUtility.cs
// Author: Aaron

using System.IO;

using UnityEngine;

/// <summary>
/// Utility class for JSON file writing functionality.
/// </summary>
public static class JSONFileUtility
{
    #region Public Methods

    /// <summary>
    /// Saves the given object as a JSON file to the given path.
    /// </summary>
    public static void SaveToJSONFile (object obj, string path)
    {
        var json = JsonUtility.ToJson(obj);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Loads an object as JSON from the given path.
    /// </summary>
    public static T LoadFromJSONFile<T> (string path)
    {
        var json = File.ReadAllText (path);
        return JsonUtility.FromJson<T>(json);
    }

    #endregion
}
