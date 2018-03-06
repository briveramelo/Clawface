// PlayerSpawner.cs
// ©2017 Aaron Desin, Garin Richards

// 11/21: Paths have been moved to the Strings class.

using ModMan;

using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

/// <summary>
/// Automatically gets and spawns the newest player prefab.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    #region Private Fields

    // Comes from the player prefab
    private static Vector3 SPAWN_OFFSET = new Vector3 (-14f, 20f, -7f);

    private GameObject playerPrefabGO;
    private GameObject playerUIGO;

    //cause i want "camera" dammit
    new Camera camera;

    #endregion
    #region Unity Lifecycle

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        if (camera) {
            camera.enabled = false;
        }

        SpawnPlayerPrefab();

        gameObject.SetActive (false);
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Spawns the player prefab at the position of this spawner.
    /// </summary>
    void SpawnPlayerPrefab ()
    {
        // Get the path of the newest prefab
        string playerPrefabPath = GetNewestPlayerPrefabPath();
        string playerUIPrefabPath = GetNewestPlayerUIPrefabPath();
        
        // Load the prefab
        GameObject playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
        if (playerPrefab == null)
        {
            throw new NullReferenceException(
                string.Format("Failed to load prefab at \"{0}\"!",
                    playerPrefabPath));
        }
        GameObject playerUIPrefab = Resources.Load<GameObject>(playerUIPrefabPath);
        if (playerUIPrefab == null)
        {
            throw new NullReferenceException(
                string.Format("Failed to load prefab at \"{0}\"!",
                    playerUIPrefabPath));
        }

        // Instantiate the prefab and bring it to spawner location
        if (Application.isPlaying)
        {
            playerPrefabGO = Instantiate (playerPrefab);
            playerUIGO = Instantiate(playerUIPrefab, playerPrefabGO.transform);
        }

        #if UNITY_EDITOR
        else if (Application.isEditor)
        {
            playerPrefabGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(playerPrefab);
            playerUIGO = (GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(playerUIPrefab);
            playerUIGO.transform.SetParent(playerPrefabGO.transform);
        }
        #endif

        playerPrefabGO.transform.position = transform.position + SPAWN_OFFSET;
    }

    string GetNewestPlayerUIPrefabPath()
    {
        VersionedPlayerUIPrefab[] allPrefabs =
            Resources.LoadAll<VersionedPlayerUIPrefab>(Strings.Paths.PLAYER_UI_PREFAB_RESOURCES_PATH);
        List<GameObject> results = new List<GameObject>();

        float highestVersion = Mathf.NegativeInfinity;
        string highestVersionName = "";
        foreach (VersionedPlayerUIPrefab prefab in allPrefabs)
        {
            string name = prefab.gameObject.name;
            string versionText = name.Remove(0,
                Strings.Paths.PLAYER_UI_PREFAB_NAME.Length);

            try
            {
                // Attempt to parse to float, and check if it is newest
                float version = float.Parse(versionText);
                if (version > highestVersion)
                {
                    highestVersion = version;
                    highestVersionName = name;
                }
            }

            catch (FormatException)
            {
                Debug.LogError(string.Format(
                    "Failed to parse version number at {0}!", name));
                continue;
            }
        }

        return string.Format("{0}{1}", Strings.Paths.PLAYER_UI_PREFAB_RESOURCES_PATH, highestVersionName);
        
    }

    /// <summary>
    /// Returns the path of the newest prefab version (relative to the 
    /// resources folder).
    /// </summary>
    string GetNewestPlayerPrefabPath ()
    {
        VersionedPlayerPrefab[] allPrefabs = 
            Resources.LoadAll<VersionedPlayerPrefab>(Strings.Paths.PLAYER_PREFAB_RESOURCES_PATH);
        List<GameObject> results = new List<GameObject>();

        float highestVersion = Mathf.NegativeInfinity;
        string highestVersionName = "";
        foreach (VersionedPlayerPrefab prefab in allPrefabs)
        {
            string name = prefab.gameObject.name;
            int DELETEME = Strings.Paths.PLAYER_PREFAB_NAME.Length;
            string versionText = name.Remove(0, 
                Strings.Paths.PLAYER_PREFAB_NAME.Length);

            try
            {
                // Attempt to parse to float, and check if it is newest
                float version = float.Parse (versionText);
                if (version > highestVersion)
                {
                    highestVersion = version;
                    highestVersionName = name;
                }
            } 
            
            catch (FormatException)
            {
                Debug.LogError (string.Format (
                    "Failed to parse version number at {0}!", name));
                continue;
            }
        }

        return string.Format("{0}{1}", Strings.Paths.PLAYER_PREFAB_RESOURCES_PATH, highestVersionName);
    }

    #endregion


    public GameObject GetplayerPrefabGO()
    {
        return playerPrefabGO;
    }
}
