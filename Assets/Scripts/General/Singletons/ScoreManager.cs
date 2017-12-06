﻿// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager> {

    #region Serialized

    [SerializeField]
    private bool useAlternateScoreMode;

    [SerializeField] private int score;
    [SerializeField] private int currentCombo;
    [SerializeField] private int highestCombo;
    [SerializeField] private List<float> maxTimeRemainingPerCombo;
    [SerializeField] private List<int> scoreMultiplierPerCombo;

    [SerializeField] private float maxTimeRemaining;

    [SerializeField] private Dictionary<string, int> highScores;
    #endregion

    #region Privates
    private float comboTimer;
    private float currentQuadrant;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        OnLevelStart();

        EventSystem.Instance.RegisterEvent(Strings.Events.DEATH_ENEMY, OnPlayerKilledEnemy);
        EventSystem.Instance.RegisterEvent(Strings.Events.EAT_ENEMY, OnPlayerAte);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_DAMAGED, OnPlayerDamaged);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, OnLevelStart);
    }

    private new void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.DEATH_ENEMY, OnPlayerKilledEnemy);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.EAT_ENEMY, OnPlayerAte);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_DAMAGED, OnPlayerDamaged);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, OnLevelStart);
        }

        base.OnDestroy();
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
        currentCombo = 0;
        EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_TIMER_UPDATED, 0.0f);
    }

    private void OnPlayerAte(params object[] parameters)
    {
        if (useAlternateScoreMode)
        {
            currentCombo++;
            EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_UPDATED, currentCombo);
            CalculateTimerQuadrant();
        }
    }

    private void OnPlayerKilledEnemy(params object[] parameters)
    {
        if (useAlternateScoreMode)
        {
            if (parameters != null && parameters[1] != null)
            {
                AddToScore((int)parameters[1]);
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
            currentCombo = 0;
            EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_UPDATED, currentCombo);
            CalculateTimerQuadrant();
        }
    }

    private void OnLevelStart(params object[] parameters) {
        currentCombo = 0;
        highestCombo = 0;
        currentQuadrant = 0;

        highScores = new Dictionary<string, int>();
    }

    public void AddToCombo()
    {
        currentCombo++;
        
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

        EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_UPDATED, currentCombo);
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
        int delta = 0;
        if (currentCombo < scoreMultiplierPerCombo.Count)
        {
            delta = points * scoreMultiplierPerCombo[currentCombo];
            score += delta;
        }
        else
        {
            delta = points * scoreMultiplierPerCombo[scoreMultiplierPerCombo.Count - 1];
            score += delta;
        }

        EventSystem.Instance.TriggerEvent(Strings.Events.SCORE_UPDATED,score,delta);
    }

    public int GetCurrentMultiplier()
    {
        return (currentCombo < scoreMultiplierPerCombo.Count) ? scoreMultiplierPerCombo[currentCombo] : scoreMultiplierPerCombo[scoreMultiplierPerCombo.Count - 1];
    }

    public void AddToScoreAndCombo(int points)
    {
        AddToScore(points);
        AddToCombo();
    }

    public float GetCurrentTimeRemaining()
    {
        return comboTimer;
    }

    public float GetMaxTimeRemaining()
    {
        return maxTimeRemaining;
    }


    public void UpdateHighScore(string level, int scoreToCheck)
    {
        if (!highScores.ContainsKey(level))
        {
            highScores.Add(level, scoreToCheck);
        }
        else
        {
            if (highScores[level] < scoreToCheck)
            {
                highScores[level] = scoreToCheck;
            }
        }

        EventSystem.Instance.TriggerEvent(Strings.Events.SET_LEVEL_SCORE, level, highScores[level]);
    }

    public int GetHighScore(string level)
    {
        if (highScores.ContainsKey(level))
        {
            return highScores[level];
        }
        else
        {
            return 0;
        }
    }

    public Dictionary<string, int> GetAllHighScores()
    {
        return highScores;
    }
    #endregion

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
            EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_TIMER_UPDATED, nextQuadrant);
        }
    }
}
