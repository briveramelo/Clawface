// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager> {

    #region Serialized
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
        currentCombo = 0;
        highestCombo = 0;
        currentQuadrant = 0;

        highScores = new Dictionary<string, int>();
	}
	
	// Update is called once per frame
	void Update () {
#if (UNITY_EDITOR)
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddToScoreAndCombo(100);
        }
#endif

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
    #endregion

    #region Public Methods
    public void ResetCombo()
    {
        currentCombo = 0;
        EventSystem.Instance.TriggerEvent(Strings.Events.COMBO_TIMER_UPDATED, 0.0f);
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
        if (currentCombo < scoreMultiplierPerCombo.Count)
        {
            score += (points * scoreMultiplierPerCombo[currentCombo]);
        }
        else
        {
            score += (points * scoreMultiplierPerCombo[scoreMultiplierPerCombo.Count - 1]);
        }

        EventSystem.Instance.TriggerEvent(Strings.Events.SCORE_UPDATED,score);
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
            return;
        }
        else
        {
            if (highScores[level] < scoreToCheck)
            {
                highScores[level] = scoreToCheck;
            }
        }
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
