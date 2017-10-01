// PlayerSpawner.cs
// ©2017 Aaron Desin

using ModMan;

using System;
using System.IO;

using UnityEngine;

/// <summary>
/// Automatically gets and spawns the newest player prefab.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    #region Private Fields

    const string PLAYER_PREFAB_PATH = "/Prefabs/Player/";
    const string RESOURCES_FOLDER_PATH = "/Resources/";
    const string PLAYER_GROUP_NAME = "Keira_GroupV";
    const string PREFAB_EXT = ".prefab";

    // Comes from the player prefab
    static Vector3 SPAWN_OFFSET = new Vector3 (-14f, 20f, -7f);

    GameObject player;

    new Camera camera;

    #endregion
    #region Unity Lifecycle

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        camera.enabled = false;

        SpawnPlayer();
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Spawns the player prefab at the position of this spawner.
    /// </summary>
    void SpawnPlayer ()
    {
        // Get the path of the newest prefab
        string playerPrefabPath = GetNewestPrefabPath();
        
        // Load the prefab
        GameObject playerPrefab = Resources.Load<GameObject>(playerPrefabPath);        
        if (playerPrefab == null)
            throw new NullReferenceException (
                string.Format ("Failed to load prefab at \"{0}\"!", 
                playerPrefabPath));

        // Instantiate the prefab and bring it to spawner location
        if (Application.isPlaying)
        {
            player = Instantiate (playerPrefab);
            Debug.Log ("Spawned player (player)");
        }

        #if UNITY_EDITOR
        else if (Application.isEditor)
        {
            player = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(playerPrefab);
            Debug.Log ("Spawned player (editor)");
        }
        #endif

        player.transform.SetParent (transform);
        player.transform.position = transform.position + SPAWN_OFFSET;
    }

    /// <summary>
    /// Returns the path of the newest prefab version (relative to the 
    /// resources folder).
    /// </summary>
    string GetNewestPrefabPath ()
    {
        // Get absolute path
        string absolutePath = string.Format ("{0}{1}{2}", 
            Application.dataPath, 
            PLAYER_PREFAB_PATH,
            RESOURCES_FOLDER_PATH);

        // List all files in directory
        string[] allPrefabFiles = Directory.GetFiles(absolutePath);

        // Search for highest versioned
        float highestVersion = Mathf.NegativeInfinity;
        string highestVersionPath = default(string);
        foreach (string filePath in allPrefabFiles)
        {
            // Check if file is a .prefab file
            string ext = Path.GetExtension (filePath);
            if (ext != PREFAB_EXT) continue;

            // Get file name
            string fileName = Path.GetFileNameWithoutExtension (filePath);

            // Check if file name has the correct naming convention
            int nameIndex = fileName.IndexOf (PLAYER_GROUP_NAME);
            if (nameIndex < 0) continue;

            // Isolate the version number
            string versionText = fileName.Remove(nameIndex, 
                PLAYER_GROUP_NAME.Length);

            try
            {
                // Attempt to parse to float, and check if it is newest
                float version = float.Parse (versionText);
                if (version > highestVersion)
                {
                    version = highestVersion;
                    highestVersionPath = filePath;
                }
            } 
            
            catch (FormatException)
            {
                Debug.LogError (string.Format (
                    "Failed to parse version number at {0}!", filePath));
                continue;
            }
        }

        if (highestVersionPath == default(string))
        {
            Debug.LogError ("Failed to get newest player prefab!");
            return null;
        }

        // Change path to be relative to Resources folder
        string stringToRemove = string.Format ("{0}{1}{2}", 
            Application.dataPath, PLAYER_PREFAB_PATH, RESOURCES_FOLDER_PATH);
        highestVersionPath = highestVersionPath.Remove (0, 
            stringToRemove.Length);

        // Trim file extension
        highestVersionPath = highestVersionPath.Remove (
            highestVersionPath.Length - PREFAB_EXT.Length, 
            PREFAB_EXT.Length);

        return highestVersionPath;
    }

    #endregion
}
