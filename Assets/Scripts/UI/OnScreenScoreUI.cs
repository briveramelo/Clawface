//Garin
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenScoreUI : MonoBehaviour {

    #region Public Fields
    #endregion

    #region Serialized Unity Inspector Fields
    [Header("OnScreenCombo")]
    [SerializeField] private Text onScreenCombo;
    [SerializeField] private float comboOnScreenTime = 2.0f;
    [SerializeField] private Image comboTimer;

    [Header("Score")]
    [SerializeField] private Text onScreenScore;

    [Header("HealthBar")]
    [SerializeField] private Transform healthMask;
    [SerializeField] private Transform healthBar;
    #endregion

    #region Private Fields
    private ScoreManager sm;
    //private bool comboDisplayed = false;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        
    }
    private void Start()
    {
        sm = ScoreManager.Instance;
        comboTimer.fillAmount = 0f;
    }

    private void LateUpdate()
    {
        UpdateScore();
        UpdateCombo();
        UpdateComboTimer();

    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    public void SetHealth(float i_val)
    {
        Assert.IsTrue(i_val >= 0.0F && i_val <= 1.0F);
        healthMask.localScale = new Vector3(i_val, 1.0F, 1.0F);
        healthBar.localScale = new Vector3(i_val == 0 ? 0 : 1 / i_val, 1.0F, 1.0F);
    }
    void UpdateComboTimer()
    {
        if (sm && !Mathf.Equals(sm.GetMaxTimeRemaining(), 0f) && sm.GetCurrentTimeRemaining() > 0f)
        {
            SetTimerValue(sm.GetCurrentTimeRemaining(), sm.GetMaxTimeRemaining());
        }
        else if (sm.GetCurrentTimeRemaining() < 0f)
        {
            ResetTimerValue();
        }
    }

    //TODO: Hmmm........There's a better way of doing this - G
    void SetTimerValue(float i_timeRem, float i_timeMax)
    {
        if (sm)
        {
            float percRemain = i_timeRem / i_timeMax;
            float result = Mathf.Ceil(percRemain * 6.0f) * (1.0f / 6.0f);
            comboTimer.fillAmount = result;
        }
    }
    
    void ResetTimerValue()
    {
        if(sm)
        {
            comboTimer.fillAmount = 0f;
        }
    }

    void UpdateScore()
    {
        if (sm.GetScore() > 0f)
        {
            onScreenScore.text = sm.GetScore().ToString();
        }
    }

    void UpdateCombo()
    {
        if (sm)
        {
            if (sm.GetCombo() > 0)
            {
                onScreenCombo.text = sm.GetCombo().ToString();
                StartCoroutine(ShowThenHideCombo());
            }
            else
            {
                if (onScreenCombo.color.a != 0f)
                {
                    SetAlphaOfText(onScreenCombo, 0.0f);
                }
            }
        }

    }

    void SetAlphaOfText(Text i_toMod, float i_newAlpha)
    {
        Color c = i_toMod.color;
        c.a = i_newAlpha;
        i_toMod.color = c;
    }
    #endregion

    #region Private Structures
    private IEnumerator ShowThenHideCombo()
    {
        SetAlphaOfText(onScreenCombo, 1.0f);
        yield return new WaitForSeconds(comboOnScreenTime);
        SetAlphaOfText(onScreenCombo, 0.0f);
    }
    #endregion
}
