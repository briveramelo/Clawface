﻿// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : Singleton<AnalyticsManager>
{

    #region Public fields
    

    #endregion

    #region Serialized Unity Inspector fields

    [SerializeField]
    private bool sendData;

    [SerializeField]
    private float timeBetweenTicks;

    [SerializeField]
    private int currentWave;

    [SerializeField]
    private float currentLevelTime;

    [SerializeField]
    private float totalCurrentLevelTime;
    #endregion



    #region Private Fields
    private Stats playerStats;
    private ModManager manager;

    // Dictionary of stats that is recorded and sent on player death
    private Dictionary<string, object> playerDeathDictionary;

    // Dictionary of general stats that is sent on exiting the game
    private Dictionary<string, object> quitGameDictionary;

    // Dictionary of times (in seconds) of what mods were equipped during gameplay
    private Dictionary<string, object> modTimeDictionary;

    // Dictionary of times (in ratios of time mod equipped / total time playing) of what mods were equipped in any slot during gameplay
    private Dictionary<string, object> modRatioDictionary;

    // Dictionary of how many times buttons were pressed during gameplay, in average per minute
    private Dictionary<string, object> buttonPressesDictionary;

    // Dictionary of how much damage the player did with every mod in the game
    private Dictionary<string, object> modDamageDictionary;

    // Dictionary of how much damage enemies did with their mods
    private Dictionary<string, object> enemyModDamageDictionary;

    // Dictionary of how many times each mod was used in the left arm slot
    private Dictionary<string, object> modArmLPressesDictionary;

    // Dictionary of how many times each mod was used in the right arm slot
    private Dictionary<string, object> modArmRPressesDictionary;

    // Dictionary of how many times each mod was used in the legs slot
    private Dictionary<string, object> modLegsPressesDictionary;

    // Dictionary of how many times the player scored kills with each mod
    private Dictionary<string, object> modKillsDictionary;

    // Dictionary of level event triggers
    private Dictionary<string, object> levelDataDictionary;

    private float timer;
    private float playerHealthTotal;
    private int playerHealthTicks;

    private float totalTime;

    private int deaths;

    #endregion


    #region Unity Lifecycle
    
    private new void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, OnLevelStarted);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, OnPlayerKilled);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, OnLevelCompleted);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_QUIT, OnLevelQuit);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_RESTARTED, OnLevelRestart);
        }

        base.OnDestroy();   
    }


    // Use this for initialization
    void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, OnLevelStarted);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, OnPlayerKilled);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, OnLevelCompleted);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_QUIT, OnLevelQuit);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, OnLevelRestart);


        quitGameDictionary = new Dictionary<string, object>();
        quitGameDictionary.Add("armL", "null");
        quitGameDictionary.Add("armR", "null");
        quitGameDictionary.Add("legs", "null");
        quitGameDictionary.Add("averageHP", 0);
        quitGameDictionary.Add("currentHP", 0);
        quitGameDictionary.Add("deaths", 0);
        quitGameDictionary.Add("swaps", 0f);
        quitGameDictionary.Add("drops", 0f);
        quitGameDictionary.Add("sessionTimeMins", 0);

        playerDeathDictionary = new Dictionary<string, object>();
        playerDeathDictionary.Add("armL", "null");
        playerDeathDictionary.Add("armR", "null");
        playerDeathDictionary.Add("legs", "null");
        playerDeathDictionary.Add("averageHP", 0);


        modTimeDictionary = new Dictionary<string, object>();
        modRatioDictionary = new Dictionary<string, object>();
        modDamageDictionary = new Dictionary<string, object>();
        enemyModDamageDictionary = new Dictionary<string, object>();
        modArmLPressesDictionary = new Dictionary<string, object>();
        modArmRPressesDictionary = new Dictionary<string, object>();
        modLegsPressesDictionary = new Dictionary<string, object>();
        modKillsDictionary = new Dictionary<string, object>();

        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            modTimeDictionary.Add(mod.ToString(), 0f);
            modRatioDictionary.Add(mod.ToString(), 0f);
            modDamageDictionary.Add(mod.ToString(), 0f);
            enemyModDamageDictionary.Add(mod.ToString(), 0f);
            modArmLPressesDictionary.Add(mod.ToString(), 0f);
            modArmRPressesDictionary.Add(mod.ToString(), 0f);
            modLegsPressesDictionary.Add(mod.ToString(), 0f);
            modKillsDictionary.Add(mod.ToString(), 0f);
        }

        buttonPressesDictionary = new Dictionary<string, object>();
        buttonPressesDictionary.Add("armL", 0f);
        buttonPressesDictionary.Add("armR", 0f);
        buttonPressesDictionary.Add("legs", 0f);
        buttonPressesDictionary.Add("dodge", 0f);
        buttonPressesDictionary.Add("swap", 0f);
        buttonPressesDictionary.Add("skin", 0f);
    }

    //#if !UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        currentLevelTime += Time.deltaTime;
        totalCurrentLevelTime += Time.deltaTime;

        /*
        timer += Time.deltaTime;
        totalTime += Time.deltaTime;

        if (timer >= timeBetweenTicks)
        {
            playerHealthTicks++;
            timer = 0;

            UpdatePlayerHealthTotal();
            UpdateModTimes();

        }

        UpdateButtonPresses();
        */

        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            WriteOutToTextFile();
        }
        */
    }
    //#endif

    private void OnApplicationQuit()
    {

        // FormatDictionaries();

        /*
#if !UNITY_EDITOR
        if (sendData)
        {
            Analytics.CustomEvent("levelTriggers", levelDataDictionary);
            Analytics.CustomEvent("modTimes", modTimeDictionary);
            Analytics.CustomEvent("modRatios", modRatioDictionary);
            Analytics.CustomEvent("quitGame", quitGameDictionary);
            Analytics.CustomEvent("buttonPresses", buttonPressesDictionary);
            Analytics.CustomEvent("modDamage", modDamageDictionary);
            Analytics.CustomEvent("enemyModDamage", enemyModDamageDictionary);
            Analytics.CustomEvent("armLButtonPresses", modArmLPressesDictionary);
            Analytics.CustomEvent("armRButtonPresses", modArmRPressesDictionary);
            Analytics.CustomEvent("legsButtonPresses", modLegsPressesDictionary);
            Analytics.CustomEvent("modKills", modKillsDictionary);
        }
#endif
*/

    }
    #endregion

    #region Public Methods
    public void StartLevel(string level, string leftArm, string rightArm)
    {
        
    }

    


    public void FormatPlayerModsInDictionary(ref Dictionary<string, object> dict)
    {
        if (manager != null)
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

            //if (modSocketDictionary[ModSpot.Legs].mod != null)
            //{
            //    dict["legs"] = modSocketDictionary[ModSpot.Legs].mod.getModType().ToString();
            //}
            //else
            //{
            dict["legs"] = "null";
            //}
        }
    }

    public int GetCurrentWave() {
        return currentWave;
    }

    public void SwapMods()
    {
        quitGameDictionary["swaps"] = (float)quitGameDictionary["swaps"] + 1f;
    }

    public void DropMod()
    {
        quitGameDictionary["drops"] = (float)quitGameDictionary["drops"] + 1f;
    }

    public void ActivateLevelTrigger(string triggerName)
    {
        if (levelDataDictionary.ContainsKey(triggerName))
        {
            levelDataDictionary[triggerName] = true;
        }
    }

    public void AddLevelTrigger(string triggerName)
    {
        if (levelDataDictionary == null)
        {
            levelDataDictionary = new Dictionary<string, object>();
        }

        if (!levelDataDictionary.ContainsKey(triggerName))
        {
            levelDataDictionary.Add(triggerName, false);
        }
    }


    public void SetModManager(ModManager manager)
    {
        this.manager = manager;


    }

    public void AddModDamage(ModType modType, float damage)
    {
        modDamageDictionary[modType.ToString()] = (float)modDamageDictionary[modType.ToString()] + damage;
    }

    public void AddEnemyModDamage(ModType modType, float damage)
    {
        enemyModDamageDictionary[modType.ToString()] = (float)enemyModDamageDictionary[modType.ToString()] + damage;
    }

    public void AddModKill(ModType modType)
    {
        modKillsDictionary[modType.ToString()] = (float)modKillsDictionary[modType.ToString()] + 1f;
    }

    public void SetPlayerStats(Stats stats)
    {
        if (stats != null)
        {
            playerStats = stats;
            playerHealthTotal = stats.GetStat(CharacterStatType.Health);
            playerHealthTicks = 1;
        }
    }

    public void SetCurrentWave(int wave)
    {
        currentWave = wave;
    }

    public void SetCurrentLevelTime(float time)
    {
        currentLevelTime = time;
    }

    #endregion

    #region Private Methods
    private void OnLevelStarted(params object[] parameters)
    {
        currentLevelTime = 0f;
        totalCurrentLevelTime = 0f;

        string level = parameters[0] as string;
        string leftArm = parameters[1] as string;
        string rightArm = parameters[2] as string;


        Dictionary<string, object> startLevelDictionary = new Dictionary<string, object>();
        startLevelDictionary.Add("level", level);
        startLevelDictionary.Add("leftArm", leftArm);
        startLevelDictionary.Add("rightArm", rightArm);

#if UNITY_EDITOR
        Debug.Log(String.Format("Started level event fired: {0}, {1}, {2}", level, leftArm, rightArm));
#endif

        Analytics.CustomEvent(Strings.Events.LEVEL_STARTED, startLevelDictionary);

    }

    private void OnPlayerKilled(params object[] parameters)
    {
        Dictionary<string, object> playerEventDeath = new Dictionary<string, object>();

        string level = parameters[0] as string;
        int wave = (int)parameters[1];
        string leftArm = parameters[2] as string;
        string rightArm = parameters[3] as string;

        playerEventDeath.Add("level", level);
        playerEventDeath.Add("wave", wave.ToString());
        playerEventDeath.Add("leftArm", leftArm);
        playerEventDeath.Add("rightArm", rightArm);

#if UNITY_EDITOR
        Debug.Log(String.Format("Player death event fired: {0}, {1}, {2}, {3}", level, wave.ToString(), leftArm, rightArm));
#endif

        Analytics.CustomEvent(Strings.Events.PLAYER_KILLED, playerEventDeath);
    }

    private void OnLevelRestart(params object[] parameters)
    {
        Dictionary<string, object> levelRestartDictionary = new Dictionary<string, object>();

        string level = parameters[0] as string;
        int wave = (int)parameters[1];
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = (int)parameters[2];
        string leftArm = parameters[3] as string;
        string rightArm = parameters[4] as string;

        levelRestartDictionary.Add("level", level);
        levelRestartDictionary.Add("wave", wave);
        levelRestartDictionary.Add("runTime", runtime);
        levelRestartDictionary.Add("levelTime", totalLevelTime);
        levelRestartDictionary.Add("score", score);
        levelRestartDictionary.Add("leftArm", leftArm);
        levelRestartDictionary.Add("rightArm", rightArm);

#if UNITY_EDITOR
        Debug.Log(String.Format("Level restarted event fired: {0}, {1}, {2}, {3}, {4}, {5}, {6}", level, wave, runtime.ToString(), totalLevelTime.ToString(), score.ToString(), leftArm, rightArm));
#endif

        Analytics.CustomEvent(Strings.Events.LEVEL_RESTARTED, levelRestartDictionary);

        currentLevelTime = 0f;
    }

    private void OnLevelQuit(params object[] parameters)
    {
        Dictionary<string, object> levelQuitDictionary = new Dictionary<string, object>();

        string level = parameters[0] as string;
        int wave = (int)parameters[1];
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = (int)parameters[2];
        string leftArm = parameters[3] as string;
        string rightArm = parameters[4] as string;

        levelQuitDictionary.Add("level", level);
        levelQuitDictionary.Add("wave", wave);
        levelQuitDictionary.Add("runTime", runtime);
        levelQuitDictionary.Add("levelTime", totalLevelTime);
        levelQuitDictionary.Add("score", score);
        levelQuitDictionary.Add("leftArm", leftArm);
        levelQuitDictionary.Add("rightArm", rightArm);

#if UNITY_EDITOR
        Debug.Log(String.Format("Level quit event fired: {0}, {1}, {2}, {3}, {4}, {5}, {6}", level, wave, runtime.ToString(), totalLevelTime.ToString(), score.ToString(), leftArm, rightArm));
#endif

        Analytics.CustomEvent(Strings.Events.LEVEL_QUIT, levelQuitDictionary);

        currentLevelTime = 0f;
        totalCurrentLevelTime = 0f;
    }

    private void OnLevelCompleted(params object[] parameters)
    {
        Dictionary<string, object> levelCompletedDictionary = new Dictionary<string, object>();

        string level = parameters[0] as string;
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = (int)parameters[1];
        string leftArm = parameters[2] as string;
        string rightArm = parameters[3] as string;

        levelCompletedDictionary.Add("level", level);
        levelCompletedDictionary.Add("runTime", runtime);
        levelCompletedDictionary.Add("levelTime", totalLevelTime);
        levelCompletedDictionary.Add("score", score);
        levelCompletedDictionary.Add("leftArm", leftArm);
        levelCompletedDictionary.Add("rightArm", rightArm);

#if UNITY_EDITOR
        Debug.Log(String.Format("Level completed event fired: {0}, {1}, {2}, {3}, {4}, {5}", level, runtime.ToString(), totalLevelTime.ToString(), score.ToString(), leftArm, rightArm));
#endif

        Analytics.CustomEvent(Strings.Events.LEVEL_COMPLETED, levelCompletedDictionary);

        currentLevelTime = 0f;
        totalCurrentLevelTime = 0f;
    }

    private void FormatDictionaries()
    {
        if (playerStats != null)
        {
            int totalTimeMinutes = Mathf.FloorToInt(totalTime) / 60;
            if (totalTimeMinutes < 1) totalTimeMinutes = 1;

            FormatPlayerModsInDictionary(ref quitGameDictionary);
            quitGameDictionary["averageHP"] = (int)(playerHealthTotal / playerHealthTicks);
            quitGameDictionary["currentHP"] = (int)playerStats.GetStat(CharacterStatType.Health);
            quitGameDictionary["swaps"] = (float)quitGameDictionary["swaps"];
            quitGameDictionary["drops"] = (float)quitGameDictionary["drops"];
            quitGameDictionary["deaths"] = deaths;
            quitGameDictionary["sessionTimeMins"] = totalTimeMinutes;

            // Changes button presses to be average per minute
            buttonPressesDictionary["armL"] = (float)buttonPressesDictionary["armL"] / (float)totalTimeMinutes;
            buttonPressesDictionary["armR"] = (float)buttonPressesDictionary["armR"] / (float)totalTimeMinutes;
            buttonPressesDictionary["legs"] = (float)buttonPressesDictionary["legs"] / (float)totalTimeMinutes;
            buttonPressesDictionary["dodge"] = (float)buttonPressesDictionary["dodge"] / (float)totalTimeMinutes;
            buttonPressesDictionary["swap"] = (float)buttonPressesDictionary["swap"] / (float)totalTimeMinutes;
            buttonPressesDictionary["skin"] = (float)buttonPressesDictionary["skin"] / (float)totalTimeMinutes;


            // Changes mod times to be a percentage of total play time
            foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
            {
                modRatioDictionary[mod.ToString()] = (float)modTimeDictionary[mod.ToString()] / totalTime;
            }
        }
    }

    private void WriteOutToTextFile()
    {
        string filePath = "./TestInfo.txt";

        FormatDictionaries();

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Mod Ratios:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modRatioDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Mod Times (in seconds):" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modTimeDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Total Player Mod Damage:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modDamageDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Total Enemy Mod Damage:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, enemyModDamageDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Left Arm Mod Button Presses:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modArmLPressesDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Right Arm Mod Button Presses:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modArmRPressesDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Leg Mod Button Presses:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modLegsPressesDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Mod Kills:" + Environment.NewLine);
        AppendTextToFileFromModDictionary(filePath, modKillsDictionary);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Quit Game Dictionary:" + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Left Arm: " + quitGameDictionary["armL"].ToString() + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Right Arm:" + quitGameDictionary["armR"].ToString() + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Legs:" + quitGameDictionary["legs"].ToString() + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Average HP: " + quitGameDictionary["averageHP"] + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Current HP: " + quitGameDictionary["currentHP"] + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Deaths: " + quitGameDictionary["deaths"] + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Swaps: " + quitGameDictionary["swaps"] + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Drops: " + quitGameDictionary["drops"] + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, "Seesion Time (in Minutes): " + quitGameDictionary["sessionTimeMins"] + Environment.NewLine);

        System.IO.File.AppendAllText(filePath, Environment.NewLine + "Button Presses:" + Environment.NewLine);
        System.IO.File.AppendAllText(filePath, String.Format("Right Arm: {0}" + Environment.NewLine, (float)buttonPressesDictionary["armR"]));
        System.IO.File.AppendAllText(filePath, String.Format("Left Arm: {0}" + Environment.NewLine, (float)buttonPressesDictionary["armL"]));
        System.IO.File.AppendAllText(filePath, String.Format("Legs: {0}" + Environment.NewLine, (float)buttonPressesDictionary["legs"]));
        System.IO.File.AppendAllText(filePath, String.Format("Dodge: {0}" + Environment.NewLine, (float)buttonPressesDictionary["dodge"]));
        System.IO.File.AppendAllText(filePath, String.Format("Swap: {0}" + Environment.NewLine, (float)buttonPressesDictionary["swap"]));
        System.IO.File.AppendAllText(filePath, String.Format("Skin: {0}" + Environment.NewLine, (float)buttonPressesDictionary["skin"]));

    }

    private void AppendTextToFileFromModDictionary(string fileName, Dictionary<string, object> dict)
    {
        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            System.IO.File.AppendAllText(fileName, String.Format("{0}, {1}" + Environment.NewLine, mod.ToString(), (float)dict[mod.ToString()]));
        }
    }

    private void UpdatePlayerHealthTotal()
    {
        if (playerStats != null)
        {
            playerHealthTotal += playerStats.GetStat(CharacterStatType.Health);
        }
    }

    private void UpdateButtonPresses()
    {
        if (manager != null)
        {
            Dictionary<ModSpot, ModManager.ModSocket> modSocketDictionary = manager.GetModSpotDictionary();

            if (InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_LEFT, ButtonMode.DOWN))
            {
                buttonPressesDictionary["armL"] = (float)buttonPressesDictionary["armL"] + 1f;

                if (modSocketDictionary[ModSpot.ArmL].mod != null)
                {
                    string modTypeToString = modSocketDictionary[ModSpot.ArmL].mod.getModType().ToString();
                    modArmLPressesDictionary[modTypeToString] = (float)modArmLPressesDictionary[modTypeToString] + 1f;
                }
            }

            if (InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_RIGHT, ButtonMode.DOWN))
            {
                buttonPressesDictionary["armR"] = (float)buttonPressesDictionary["armR"] + 1f;

                if (modSocketDictionary[ModSpot.ArmR].mod != null)
                {
                    string modTypeToString = modSocketDictionary[ModSpot.ArmR].mod.getModType().ToString();
                    modArmRPressesDictionary[modTypeToString] = (float)modArmRPressesDictionary[modTypeToString] + 1f;
                }
            }

            if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE, ButtonMode.DOWN))
            {
                buttonPressesDictionary["dodge"] = (float)buttonPressesDictionary["dodge"] + 1f;
            }

            //if (InputManager.Instance.QueryAction(Strings.Input.Actions.SKIN, ButtonMode.DOWN)) {
            //    buttonPressesDictionary["skin"] = (float)buttonPressesDictionary["skin"] + 1f;
            //}
        }
    }

    private void UpdateModTimes()
    {
        if (manager != null)
        {
            Dictionary<ModSpot, ModManager.ModSocket> modSpotDictionary = manager.GetModSpotDictionary();
            List<ModType> equippedMods = new List<ModType>();

            if (modSpotDictionary[ModSpot.ArmL].mod != null)
            {
                equippedMods.Add(modSpotDictionary[ModSpot.ArmL].mod.getModType());
            }

            if (modSpotDictionary[ModSpot.ArmR].mod != null)
            {
                if (!equippedMods.Contains(modSpotDictionary[ModSpot.ArmR].mod.getModType()))
                {
                    equippedMods.Add(modSpotDictionary[ModSpot.ArmR].mod.getModType());
                }
            }

            //if (modSpotDictionary[ModSpot.Legs].mod != null)
            //{
            //    if (!equippedMods.Contains(modSpotDictionary[ModSpot.Legs].mod.getModType()))
            //    {
            //        equippedMods.Add(modSpotDictionary[ModSpot.Legs].mod.getModType());
            //    }
            //}

            foreach (ModType m in equippedMods)
            {
                modTimeDictionary[m.ToString()] = (float)modTimeDictionary[m.ToString()] + timeBetweenTicks;
            }
        }
    }
    #endregion

    #region Private Structures
    #endregion

}
