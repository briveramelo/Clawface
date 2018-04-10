// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : Singleton<ScoreManager> {

    #region Serialized

    [SerializeField] private bool useAlternateScoreMode;

    [SerializeField] private int score;
    [SerializeField] private int currentCombo;
    [SerializeField] private int highestCombo;
    [SerializeField] private List<float> maxTimeRemainingPerCombo;
    [SerializeField] private List<int> scoreMultiplierPerCombo;

    [SerializeField] private float maxTimeRemaining;

    [SerializeField] private float veryEasyModeMultiplier;
    [SerializeField] private float easyModeMultiplier;
    [SerializeField] private float normalModeMultiplier;
    [SerializeField] private float hardModeMultiplier;
    [SerializeField] private float veryHardModeMultiplier;

    /// <summary>
    /// Multiplies your max combo by this value at the end of the stage and the current difficulty multiplier, and adds it to your score
    /// </summary>
    [Tooltip("Multiplies your max combo by this value and the current difficulty multiplier at the end of the stage and adds it to your score.")]
    [SerializeField] private float maxComboMultiplier;

    /// <summary>
    /// Hard combo cap. Combo cannot increase past this value.
    /// </summary>
    [Tooltip("Hard combo cap. Combo cannot increase past this value.")]
    [SerializeField] private int comboCap;
    #endregion

    #region Privates
    private float comboTimer;
    private float currentQuadrant;

    private bool updateScore;
    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.DEATH_ENEMY, OnPlayerKilledEnemy },
                { Strings.Events.EAT_ENEMY, OnPlayerAte },
                { Strings.Events.PLAYER_DAMAGED, OnPlayerDamaged },
                { Strings.Events.LEVEL_STARTED, OnLevelStart },
                { Strings.Events.PLE_ON_LEVEL_READY, OnLevelStart },
                { Strings.Events.LEVEL_RESTARTED, OnLevelRestart },
                { Strings.Events.LEVEL_QUIT, OnLevelQuit },
                { Strings.Events.PLAYER_KILLED, OnPlayerKilled },
                { Strings.Events.LEVEL_COMPLETED, OnLevelCompleted },
            };
        }
    }
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Start () {
        OnLevelStart();

        base.Start();
    }       

    // Update is called once per frame
    void Update() {
        if (!useAlternateScoreMode)
        {
            if (currentCombo != 0)
            {
                comboTimer -= Time.deltaTime;

                if (comboTimer <= 0f)
                {
                    ResetCombo();
                }

                CalculateTimerQuadrant();
            }
        }
	}
    #endregion

    #region Public Methods
    public void ResetCombo()
    {
        if (GetCurrentMultiplier() != GetDifficultyMultiplier()) {
            EventSystem.Instance.TriggerEvent(Strings.Events.MULTIPLIER_UPDATED, GetDifficultyMultiplier());
        }

        if (currentCombo!=0) {
            currentCombo = 0;
            EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_UPDATED, 0);
            SFXManager.Instance.Play(SFXType.ComboLost, transform.position);
        }

        // EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_TIMER_UPDATED, 0.0f);
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void ResetScoreAndCombo()
    {
        ResetScore();
        ResetCombo();
    }

    

    public void AddToCombo()
    {
        float beforeMultiplier = GetCurrentPreDifficultyMultiplier();

        currentCombo++;

        if (currentCombo > comboCap) currentCombo = comboCap;
        
        if (currentCombo > highestCombo)
        {
            highestCombo = currentCombo;
        }

        if (currentCombo < maxTimeRemainingPerCombo.Count)
        {
            maxTimeRemaining = maxTimeRemainingPerCombo[currentCombo];
        }
        else
        {
            maxTimeRemaining = maxTimeRemainingPerCombo[maxTimeRemainingPerCombo.Count - 1];
        }

        comboTimer = maxTimeRemaining;

        float afterMultiplier = GetCurrentPreDifficultyMultiplier();

        EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_UPDATED, currentCombo);

        if (!Mathf.Approximately(beforeMultiplier, afterMultiplier))
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.MULTIPLIER_UPDATED, GetCurrentMultiplier());
        }

        CalculateTimerQuadrant();
    }

    public int GetScore()
    {
        return score;
    }

    public int GetCombo()
    {
        return currentCombo;
    }

    public int GetHighestCombo()
    {
        return highestCombo;
    }

    public void AddToScore(int points)
    {
        if (points <= 0) return;

        int delta = Mathf.FloorToInt(points * GetCurrentMultiplier());
        score += delta;
        EventSystem.Instance.TriggerEvent(Strings.Events.SCORE_UPDATED,score,delta);    
        SFXManager.Instance.Play(SFXType.Score, transform.position);
    }

    public void AddToScoreWithoutComboMultiplier(int points)
    {
        if (points <= 0) return;
        score += points;
        EventSystem.Instance.TriggerEvent(Strings.Events.SCORE_UPDATED, score, points);
    }

    public float GetCurrentPreDifficultyMultiplier()
    {
        float baseMultiplier = (currentCombo < scoreMultiplierPerCombo.Count) ? scoreMultiplierPerCombo[currentCombo] : scoreMultiplierPerCombo[scoreMultiplierPerCombo.Count - 1];
        return baseMultiplier;
    }
    public float GetDifficultyMultiplier() {
        Difficulty difficulty = SettingsManager.Instance.Difficulty;
        float difficultyMultiplier = 0f;
        switch (difficulty) {
            case Difficulty.VERY_EASY:
                difficultyMultiplier = veryEasyModeMultiplier;
                break;
            case Difficulty.EASY:
                difficultyMultiplier = easyModeMultiplier;
                break;
            case Difficulty.NORMAL:
                difficultyMultiplier = normalModeMultiplier;
                break;
            case Difficulty.HARD:
                difficultyMultiplier = hardModeMultiplier;
                break;
            case Difficulty.INSANE:
                difficultyMultiplier = veryHardModeMultiplier;
                break;
            default:
                difficultyMultiplier = normalModeMultiplier;
                break;
        }
        return difficultyMultiplier;
    }

    public float GetCurrentMultiplier()
    {
        float baseMultiplier = GetCurrentPreDifficultyMultiplier();
        float difficultyMultiplier = GetDifficultyMultiplier();
        return baseMultiplier * difficultyMultiplier;
    }

    public void AddToScoreAndCombo(int points)
    {
        AddToCombo();
        AddToScore(points);
    }

    public float GetCurrentTimeRemaining()
    {
        return comboTimer;
    }

    public float GetMaxTimeRemaining()
    {
        return maxTimeRemaining;
    }    

    public int GetMaxComboScore() {
        int result = Mathf.FloorToInt(highestCombo * maxComboMultiplier * GetDifficultyMultiplier());
        return result;
    }
    #endregion

    #region Private Methods
    private void CalculateTimerQuadrant()
    {
        float nextQuadrant = 0f;
        if (!Mathf.Equals(maxTimeRemaining, 0f) && comboTimer > 0f)
        {
            float percRemain = comboTimer / maxTimeRemaining;
            nextQuadrant = Mathf.Ceil(percRemain * 6.0f) * (1.0f / 6.0f);

        }

        if (nextQuadrant != currentQuadrant)
        {
            // EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_TIMER_UPDATED, nextQuadrant);
        }
    }

    private void OnPlayerAte(params object[] parameters)
    {
        AddToCombo();
    }

    private void OnPlayerKilledEnemy(params object[] parameters)
    {
        if (useAlternateScoreMode)
        {
            if (parameters != null && parameters[1] != null)
            {

                if (updateScore)
                {
                    AddToScoreAndCombo((int)parameters[1]);
                }

                /*
                currentCombo++;

                EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_UPDATED, currentCombo);
                CalculateTimerQuadrant();

                AddToScore((int)parameters[1]);
                */
                
            }
        }
        else
        {
            AddToScoreAndCombo((int)parameters[1]);
        }
    }

    private void OnPlayerDamaged(params object[] parameters)
    {
        if (useAlternateScoreMode)
        {
            ResetCombo();
        }
    }

    private void OnLevelStart(params object[] parameters)
    {
        updateScore = true;

        score = 0;
        currentCombo = 0;
        highestCombo = 0;
        currentQuadrant = 0;

        CalculateTimerQuadrant();
        EventSystem.Instance.TriggerEvent(Strings.Events.MULTIPLIER_UPDATED, GetDifficultyMultiplier());
    }

    private void OnLevelRestart(params object[] parameters)
    {
        OnLevelStart();
    }

    private void OnLevelQuit(params object[] parameters)
    {
        OnLevelStart();
    }

    private void OnPlayerKilled(params object[] parameters)
    {
        AddToScoreWithoutComboMultiplier(GetMaxComboScore());
        updateScore = false;
        SendScoresToLeaderboard();
    }

    private void OnLevelCompleted(params object[] parameters)
    {
        AddToScoreWithoutComboMultiplier(GetMaxComboScore());
        updateScore = false;
        SendScoresToLeaderboard();
    }    

    private void SendScoresToLeaderboard()
    {
        if (!SceneTracker.IsCurrentSceneEditor) {
            LevelUI levelUI = ((PLELevelSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU)).SelectedLevelUI;
            if (levelUI!=null) {
                string levelName = levelUI.levelData.UniqueSteamName;
                LeaderBoards.Instance.UpdateScore(score, levelName);
            }
        }
    }
    #endregion

}
