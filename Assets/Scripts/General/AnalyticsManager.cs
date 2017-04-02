﻿// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : Singleton<AnalyticsManager> {

    #region Public fields
    

    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Stats playerStats;

    [SerializeField]
    private ModManager manager;

    [SerializeField]
    private float timeBetweenTicks;
    #endregion



    #region Private Fields
    private Dictionary<string, object> playerDeathDictionary;
    private Dictionary<string, object> quitGameDictionary;

    private float timer;
    private float playerHealthTotal;
    private int playerHealthTicks;

    private int deaths;

    #endregion


    #region Unity Lifecycle


    // Use this for initialization
    void Start () {
        quitGameDictionary = new Dictionary<string, object>();
        quitGameDictionary.Add("armL", "null");
        quitGameDictionary.Add("armR", "null");
        quitGameDictionary.Add("legs", "null");
        quitGameDictionary.Add("averageHP", 0);
        quitGameDictionary.Add("currentHP", 0);
        quitGameDictionary.Add("deaths", 0);

        playerDeathDictionary = new Dictionary<string, object>();
        playerDeathDictionary.Add("armL", "null");
        playerDeathDictionary.Add("armR", "null");
        playerDeathDictionary.Add("legs", "null");
        playerDeathDictionary.Add("averageHP", 0);
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= timeBetweenTicks)
        {
            playerHealthTicks++;
            timer = 0;

            UpdatePlayerHealthTotal();
        }
	}

    

    private void OnApplicationQuit()
    {
        Debug.Log("Sending data on application quit!");
        FormatPlayerModsInDictionary(ref quitGameDictionary);
        quitGameDictionary["averageHP"] = (int) (playerHealthTotal / playerHealthTicks);
        quitGameDictionary["currentHP"] = (int) playerStats.GetStat(StatType.Health);
        quitGameDictionary["deaths"] = deaths;

        Analytics.CustomEvent("quitGame", quitGameDictionary);
    }
    #endregion

    #region Public Methods
    public void FormatPlayerModsInDictionary(ref Dictionary<string, object> dict)
    {
        Dictionary<ModSpot, ModManager.ModSocket> modSocketDictionary = manager.GetModSpotDictionary();

        if (modSocketDictionary[ModSpot.ArmL].mod != null)
        {
            dict["armL"] = modSocketDictionary[ModSpot.ArmL].mod.getModType().ToString();
        }
        else
        {
            dict["armL"] = "null";
        }

        if (modSocketDictionary[ModSpot.ArmR].mod != null)
        {
            dict["armR"] = modSocketDictionary[ModSpot.ArmR].mod.getModType().ToString();
        }
        else
        {
            dict["armR"] = "null";
        }

        if (modSocketDictionary[ModSpot.Legs].mod != null)
        {
            dict["legs"] = modSocketDictionary[ModSpot.Legs].mod.getModType().ToString();
        }
        else
        {
            dict["legs"] = "null";
        }

    }

    public void PlayerDeath()
    {
        deaths++;
        playerDeathDictionary["averageHP"] = (int) (playerHealthTotal / playerHealthTicks);
        FormatPlayerModsInDictionary(ref playerDeathDictionary);

        Analytics.CustomEvent("playerDeath", playerDeathDictionary);


        playerHealthTotal = playerStats.GetStat(StatType.Health);
        playerHealthTicks = 1;
        
    }

    public void SetModManager(ModManager manager)
    {
        this.manager = manager;
    }

    public void SetPlayerStats(Stats stats)
    {
        playerStats = stats;
        playerHealthTotal = stats.GetStat(StatType.Health);
        playerHealthTicks = 1;
    }

    #endregion

    #region Private Methods
    private void UpdatePlayerHealthTotal()
    {
        playerHealthTotal += playerStats.GetStat(StatType.Health);
    }
    #endregion

    #region Private Structures
    #endregion

}