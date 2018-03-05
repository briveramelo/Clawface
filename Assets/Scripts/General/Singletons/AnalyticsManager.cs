// Adam Kay

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

    [SerializeField]
    private string leftArmOnLoad;

    [SerializeField]
    private string rightArmOnLoad;

    [SerializeField]
    private int currentLevelDeaths;

    [SerializeField]
    private int levelEatPresses;

    [SerializeField]
    private int levelDodgePresses;
    #endregion



    #region Private Fields
    private Stats playerStats;
    private ModManager manager;


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
    }


    // Use this for initialization
    void Start()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, OnLevelStarted);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, OnPlayerKilled);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, OnLevelCompleted);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_QUIT, OnLevelQuit);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, OnLevelRestart);

        
    }


    // Update is called once per frame
    void Update()
    {
        currentLevelTime += Time.deltaTime;
        totalCurrentLevelTime += Time.deltaTime;

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.EAT) == ButtonMode.DOWN)
        {
            levelEatPresses++;
        }

        if (InputManager.Instance.QueryAction(Strings.Input.Actions.DODGE) == ButtonMode.DOWN)
        {
            levelDodgePresses++;
        }
    }

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

    public int GetCurrentWave() {
        return currentWave;
    }


    public void ActivateLevelTrigger(string triggerName)
    {
        /*
        if (levelDataDictionary.ContainsKey(triggerName))
        {
            levelDataDictionary[triggerName] = true;
        }
        */
    }

    public void AddLevelTrigger(string triggerName)
    {
        /*
        if (levelDataDictionary == null)
        {
            levelDataDictionary = new Dictionary<string, object>();
        }

        if (!levelDataDictionary.ContainsKey(triggerName))
        {
            levelDataDictionary.Add(triggerName, false);
        }
        */
    }


    public void SetModManager(ModManager manager)
    {
        this.manager = manager;


    }

    public void AddModDamage(ModType modType, float damage)
    {
        // modDamageDictionary[modType.ToString()] = (float)modDamageDictionary[modType.ToString()] + damage;
    }
    
    public void AddModKill(ModType modType)
    {
        // modKillsDictionary[modType.ToString()] = (float)modKillsDictionary[modType.ToString()] + 1f;
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
        levelEatPresses = 0;
        levelDodgePresses = 0;

        string level = parameters[0] as string;
        string leftArm = parameters[1] as string;
        string rightArm = parameters[2] as string;

        leftArmOnLoad = leftArm;
        rightArmOnLoad = rightArm;

        currentLevelDeaths = 0;

        Dictionary<string, object> startLevelDictionary = new Dictionary<string, object>();
        startLevelDictionary.Add("level", level);
        startLevelDictionary.Add("leftArm", leftArm);
        startLevelDictionary.Add("rightArm", rightArm);

#if UNITY_EDITOR
// Debug.Log(String.Format("Started level event fired: {0}, {1}, {2}", level, leftArm, rightArm));
#endif

#if !UNITY_EDITOR
        Analytics.CustomEvent(Strings.Events.LEVEL_STARTED, startLevelDictionary);
#endif

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
        playerEventDeath.Add("runTime", currentLevelTime);
        playerEventDeath.Add("eats", levelEatPresses);
        playerEventDeath.Add("dodges", levelDodgePresses);

        currentLevelDeaths++;

#if UNITY_EDITOR
        // Debug.Log(String.Format("Player death event fired: {0}, {1}, {2}, {3}, {4}, {5}", level, wave.ToString(), leftArm, rightArm, levelEatPresses, levelDodgePresses));
#endif

#if !UNITY_EDITOR
        Analytics.CustomEvent(Strings.Events.PLAYER_KILLED, playerEventDeath);
#endif
    }

    private void OnLevelRestart(params object[] parameters)
    {
        Dictionary<string, object> levelRestartDictionary = new Dictionary<string, object>();

        string level = parameters[0] as string;
        int wave = (int)parameters[1];
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = (int)parameters[2];
        string leftArm = leftArmOnLoad;
        string rightArm = rightArmOnLoad;

        levelRestartDictionary.Add("level", level);
        levelRestartDictionary.Add("wave", wave);
        levelRestartDictionary.Add("runTime", runtime);
        levelRestartDictionary.Add("levelTime", totalLevelTime);
        levelRestartDictionary.Add("score", score);
        levelRestartDictionary.Add("leftArm", leftArm);
        levelRestartDictionary.Add("rightArm", rightArm);
        levelRestartDictionary.Add("deaths", currentLevelDeaths);
        levelRestartDictionary.Add("eats", levelEatPresses);
        levelRestartDictionary.Add("dodges", levelDodgePresses);

#if UNITY_EDITOR
// Debug.Log(String.Format("Level restarted event fired: {0}, {1}, {2}, {3}, {4}, {5}, {6}", level, wave, runtime.ToString(), totalLevelTime.ToString(), score.ToString(), leftArm, rightArm));
#endif

#if !UNITY_EDITOR
        Analytics.CustomEvent(Strings.Events.LEVEL_RESTARTED, levelRestartDictionary);
#endif

        currentLevelTime = 0f;
        levelEatPresses = 0;
        levelDodgePresses = 0;
    }

    private void OnLevelQuit(params object[] parameters)
    {
        Dictionary<string, object> levelQuitDictionary = new Dictionary<string, object>();

        string level = parameters[0] as string;
        int wave = (int)parameters[1];
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = (int)parameters[2];
        string leftArm = leftArmOnLoad;
        string rightArm = rightArmOnLoad;

        levelQuitDictionary.Add("level", level);
        levelQuitDictionary.Add("wave", wave);
        levelQuitDictionary.Add("runTime", runtime);
        levelQuitDictionary.Add("levelTime", totalLevelTime);
        levelQuitDictionary.Add("score", score);
        levelQuitDictionary.Add("leftArm", leftArm);
        levelQuitDictionary.Add("rightArm", rightArm);
        levelQuitDictionary.Add("deaths", currentLevelDeaths);
        levelQuitDictionary.Add("eats", levelEatPresses);
        levelQuitDictionary.Add("dodges", levelDodgePresses);

#if UNITY_EDITOR
// Debug.Log(String.Format("Level quit event fired: {0}, {1}, {2}, {3}, {4}, {5}, {6}", level, wave, runtime.ToString(), totalLevelTime.ToString(), score.ToString(), leftArm, rightArm));
#endif

#if !UNITY_EDITOR
        Analytics.CustomEvent(Strings.Events.LEVEL_QUIT, levelQuitDictionary);
#endif

        currentLevelTime = 0f;
        totalCurrentLevelTime = 0f;
        currentLevelDeaths = 0;
        levelEatPresses = 0;
        levelDodgePresses = 0;
    }

    private void OnLevelCompleted(params object[] parameters)
    {
        Dictionary<string, object> levelCompletedDictionary = new Dictionary<string, object>();

        string level = parameters[0] as string;
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = (int)parameters[1];
        string leftArm = leftArmOnLoad;
        string rightArm = rightArmOnLoad;

        levelCompletedDictionary.Add("level", level);
        levelCompletedDictionary.Add("runTime", runtime);
        levelCompletedDictionary.Add("levelTime", totalLevelTime);
        levelCompletedDictionary.Add("score", score);
        levelCompletedDictionary.Add("leftArm", leftArm);
        levelCompletedDictionary.Add("rightArm", rightArm);
        levelCompletedDictionary.Add("deaths", currentLevelDeaths);
        levelCompletedDictionary.Add("eats", levelEatPresses);
        levelCompletedDictionary.Add("dodges", levelDodgePresses);

#if UNITY_EDITOR

// Debug.Log(String.Format("Level completed event fired: {0}, {1}, {2}, {3}, {4}", level, time.ToString(), score.ToString(), leftArm, rightArm));
#endif

#if !UNITY_EDITOR
        Analytics.CustomEvent(Strings.Events.LEVEL_COMPLETED, levelCompletedDictionary);
#endif

        currentLevelTime = 0f;
        totalCurrentLevelTime = 0f;
        currentLevelDeaths = 0;
        levelEatPresses = 0;
        levelDodgePresses = 0;
    }

    /*
    private void WriteOutToTextFile()
    {
        string filePath = "./TestInfo.txt";

        // FormatDictionaries();

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
    */

    private void AppendTextToFileFromModDictionary(string fileName, Dictionary<string, object> dict)
    {
        foreach (ModType mod in System.Enum.GetValues(typeof(ModType)))
        {
            System.IO.File.AppendAllText(fileName, String.Format("{0}, {1}" + Environment.NewLine, mod.ToString(), (float)dict[mod.ToString()]));
        }
    }
    
   
#endregion

#region Private Structures
#endregion

}
