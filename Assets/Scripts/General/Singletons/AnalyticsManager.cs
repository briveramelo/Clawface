// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

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

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.LEVEL_STARTED, OnLevelStarted },
                { Strings.Events.PLAYER_KILLED, OnPlayerKilled },
                { Strings.Events.LEVEL_COMPLETED, OnLevelCompleted },
                { Strings.Events.LEVEL_QUIT, OnLevelQuit },
                { Strings.Events.LEVEL_RESTARTED, OnLevelRestart },
                { Strings.Events.WAVE_COMPLETE, OnWaveComplete },
            };
        }
    }
    #endregion


    #region Unity Lifecycle

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

    public void IncrementWave()
    {
        currentWave++;
    }

    public void SetCurrentLevelTime(float time)
    {
        currentLevelTime = time;
    }

    #endregion

    #region Private Methods
    private void OnLevelStarted(params object[] parameters)
    {
        if (SceneTracker.IsCurrentSceneEditor || SceneTracker.IsCurrentScenePlayerLevels)
        {
            return;
        }

        currentLevelTime = 0f;
        totalCurrentLevelTime = 0f;
        levelEatPresses = 0;
        levelDodgePresses = 0;
        currentWave = 0;

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
        if (SceneTracker.IsCurrentSceneEditor || SceneTracker.IsCurrentScenePlayerLevels)
        {
            return;
        }

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
        playerEventDeath.Add("maxCombo", ScoreManager.Instance.GetHighestCombo());

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
        if (SceneTracker.IsCurrentSceneEditor || SceneTracker.IsCurrentScenePlayerLevels)
        {
            return;
        }

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
        levelRestartDictionary.Add("maxCombo", ScoreManager.Instance.GetHighestCombo());
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
        currentWave = 0;
    }

    private void OnLevelQuit(params object[] parameters)
    {
        if (SceneTracker.IsCurrentSceneEditor || SceneTracker.IsCurrentScenePlayerLevels)
        {
            return;
        }

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
        levelQuitDictionary.Add("maxCombo", ScoreManager.Instance.GetHighestCombo());
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
        currentWave = 0;
    }

    private void OnWaveComplete(params object[] parameters)
    {

        if (SceneTracker.IsCurrentSceneEditor || SceneTracker.IsCurrentScenePlayerLevels || currentWave <= 0)
        {
            return;
        }

        Dictionary<string, object> waveCompletedDictionary = new Dictionary<string, object>();

        string level = SceneTracker.CurrentSceneName;
        float runtime = currentLevelTime;
        float totalLevelTime = totalCurrentLevelTime;
        int score = ScoreManager.Instance.GetScore();
        string leftArm = leftArmOnLoad;
        string rightArm = rightArmOnLoad;

        waveCompletedDictionary.Add("level", level);
        waveCompletedDictionary.Add("runTime", runtime);
        waveCompletedDictionary.Add("wave", currentWave);
        waveCompletedDictionary.Add("score", score);
        waveCompletedDictionary.Add("leftArm", leftArm);
        waveCompletedDictionary.Add("rightArm", rightArm);
        waveCompletedDictionary.Add("deaths", currentLevelDeaths);
        waveCompletedDictionary.Add("eats", levelEatPresses);
        waveCompletedDictionary.Add("dodges", levelDodgePresses);
        waveCompletedDictionary.Add("maxCombo", ScoreManager.Instance.GetHighestCombo());

#if !UNITY_EDITOR
        Analytics.CustomEvent(Strings.Events.WAVE_COMPLETE, waveCompletedDictionary);
#endif

    }

    private void OnLevelCompleted(params object[] parameters)
    {
        if (SceneTracker.IsCurrentSceneEditor || SceneTracker.IsCurrentScenePlayerLevels)
        {
            return;
        }

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
        levelCompletedDictionary.Add("maxCombo", ScoreManager.Instance.GetHighestCombo());

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
   
#endregion

#region Private Structures
#endregion

}
