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

    #endregion

    #region Privates
    private float comboTimer;

    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        currentCombo = 0;
        highestCombo = 0;
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

        }
	}
    #endregion

    #region Public Methods
    public void ResetCombo()
    {
        currentCombo = 0;
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

    #endregion
}
