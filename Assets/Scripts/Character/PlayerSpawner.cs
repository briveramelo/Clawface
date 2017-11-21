// PlayerSpawner.cs
// ©2017 Aaron Desin

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

    private const string PLAYER_PREFAB_PATH = "/Prefabs/Player/";
    private const string PLAYER_RESOURCES_FOLDER_PATH = "/Resources/Player/";
    private const string PLAYER_GROUP_NAME = "Keira_GroupV";
    private const string PREFAB_EXT = ".prefab";

    private const string PLAYER_UI_PREFAB_PATH = "/Prefabs/UI/";
    private const string PLAYER_UI_RESOUCES_FOLDER_PATH = "Resources/PlayerUI/";
    private const string PLAYER_UI_NAME = "PlayerHUDV";


    // Comes from the player prefab
    private static Vector3 SPAWN_OFFSET = new Vector3 (-14f, 20f, -7f);

    private GameObject player;

    new Camera camera;

    #endregion
    #region Unity Lifecycle

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        camera.enabled = false;

        SpawnPlayer();

        gameObject.SetActive (false);
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Spawns the player prefab at the position of this spawner.
    /// </summary>
    void SpawnPlayer ()
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
            player = Instantiate (playerPrefab);
            //Debug.Log ("Spawned player (player)");
        }

        #if UNITY_EDITOR
        else if (Application.isEditor)
        {
            player = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(playerPrefab);
            //Debug.Log ("Spawned player (editor)");
        }
        #endif

        //player.transform.SetParent (transform);
        player.transform.position = transform.position + SPAWN_OFFSET;
    }

    string GetNewestPlayerUIPrefabPath()
    {
        VersionedPlayerUIPrefab[] allPrefabs =
            Resources.LoadAll<VersionedPlayerUIPrefab>("PlayerUI/");
        List<GameObject> results = new List<GameObject>();

        float highestVersion = Mathf.NegativeInfinity;
        string highestVersionName = "";
        foreach (VersionedPlayerUIPrefab prefab in allPrefabs)
        {
            string name = prefab.gameObject.name;
            string versionText = name.Remove(0,
                PLAYER_GROUP_NAME.Length);

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

        return string.Format("{0}{1}", "Player/", highestVersionName);
        
    }

    /// <summary>
    /// Returns the path of the newest prefab version (relative to the 
    /// resources folder).
    /// </summary>
    string GetNewestPlayerPrefabPath ()
    {
        VersionedPlayerPrefab[] allPrefabs = 
            Resources.LoadAll<VersionedPlayerPrefab>("Player/");
        List<GameObject> results = new List<GameObject>();

        float highestVersion = Mathf.NegativeInfinity;
        string highestVersionName = "";
        foreach (VersionedPlayerPrefab prefab in allPrefabs)
        {
            string name = prefab.gameObject.name;
            string versionText = name.Remove(0, 
                PLAYER_GROUP_NAME.Length);

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

        return string.Format("{0}{1}", "Player/", highestVersionName);
    }

    #endregion
}
