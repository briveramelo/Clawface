//Garin
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {

    #region Public Fields
    #endregion

    #region Serialized Unity Inspector Fields
    [Header("OnScreenCombo")]
    [SerializeField] private Text onScreenCombo;
    [SerializeField] private float comboOnScreenTime = 2.0f;
    [SerializeField] private Image comboTimer;
    [SerializeField] private Animation comboAnimation;

    [Header("Score")]
    [SerializeField] private Text onScreenScore;
    [SerializeField] private Text onScreenScoreDelta;

    [Header("HealthBar")]
    [SerializeField] private Transform healthMask;
    [SerializeField] private Transform healthBar;

    [Header("Tutorial")]
    [SerializeField] private Text eatTutorialTextElement;
    [SerializeField] private Text dashTutorialTextElement;
    [SerializeField] private Image FullScreenFadeElement;
    [SerializeField] private Image tutorialBG;

    [Header("GlitchDamageEffect")]
    [SerializeField] private Sprite[] glitchSprites;
    [SerializeField] private float glitchSeconds = 1.0F;
    [SerializeField] private int glitchesPerSecond = 5;
    [SerializeField] private Image overlay;
    [SerializeField] private CanvasGroup hudCG;

    [Header("Wave Based Elements")]
    [SerializeField] private Text waveCompleteText;
    #endregion

    #region Private Fields
    
    private System.Random rng = new System.Random();
    private bool glitchInProgress = false;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        comboTimer.fillAmount = 0f;
        
    }
    private void Start()
    {
        if (EventSystem.Instance)
        {
            //register events
            EventSystem.Instance.RegisterEvent(Strings.Events.SCORE_UPDATED, UpdateScore);
            EventSystem.Instance.RegisterEvent(Strings.Events.COMBO_UPDATED, UpdateCombo);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_DAMAGED, DoDamageEffect);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, SetHealth);
            EventSystem.Instance.RegisterEvent(Strings.Events.COMBO_TIMER_UPDATED, UpdateComboQuadrant);
            EventSystem.Instance.RegisterEvent(Strings.Events.SHOW_TUTORIAL_TEXT, ShowTutorialText);
            EventSystem.Instance.RegisterEvent(Strings.Events.HIDE_TUTORIAL_TEXT, HideTutorialText);
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, HideHUD);
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_FAILED, HideHUD);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, HideHUD);
            EventSystem.Instance.RegisterEvent(Strings.Events.WAVE_COMPLETE, ShowWaveText);


        }
        onScreenCombo.text = "";
        onScreenScoreDelta.text = "";
        onScreenScore.text = "";
        waveCompleteText.text = "";
        HideTutorialText(null);
        ShowHUD(null);
    }



    private void OnDestroy()
    {
        
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCORE_UPDATED, UpdateScore);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_UPDATED, UpdateCombo);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_DAMAGED, DoDamageEffect);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, SetHealth);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_TIMER_UPDATED, UpdateComboQuadrant);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SHOW_TUTORIAL_TEXT, ShowTutorialText);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.HIDE_TUTORIAL_TEXT, HideTutorialText);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, HideHUD);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_FAILED, HideHUD);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, HideHUD);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WAVE_COMPLETE, ShowWaveText);
        }
    }
    #endregion

    #region Public Methods
    public void DoDamageEffect(params object[] items)
    {
        StartCoroutine(GlitchEffect());
    }

    public void SetHealth(params object[] i_healthVal)
    {
        float health = (float)i_healthVal[0];
        Assert.IsTrue(health >= 0.0F && health <= 1.0F);
        healthMask.localScale = new Vector3(health, 1.0F, 1.0F);
        float healthBarScale;
        if (health == 0) //acounts for NaN cases...
        {
            healthBarScale = 0;
        }
        else
        {
            healthBarScale = 1 / health;
        }
        healthBar.localScale = new Vector3(healthBarScale, 1.0F, 1.0F);
    }
    #endregion

    #region Private Methods
    private IEnumerator GlitchEffect()
    {
        if (glitchInProgress)
            yield break;
        else
            glitchInProgress = true;

        overlay.color = new Color(1.0F, 1.0F, 1.0F, 1.0F);

        float elapsedTime = 0.0F;
        float totalTime = 0.0F;
        while (totalTime < glitchSeconds)
        {
            if (elapsedTime == 0.0F)
            {
                overlay.sprite = GetRandomSprite();
            }
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            totalTime += Time.fixedDeltaTime;

            if (elapsedTime >= 1.0F / glitchesPerSecond)
            {
                elapsedTime = 0.0F;
            }
        }

        overlay.sprite = null;
        overlay.color = new Color(1.0F, 1.0F, 1.0F, 0.0F);
        glitchInProgress = false;
    }

    private IEnumerator PopTextAndHide(GameObject newDeltaGO, float modifier)
    {

        Vector3 scale = newDeltaGO.transform.localScale;
        while (scale.x > 0f)
        {
            scale = newDeltaGO.transform.localScale;
            scale.x -= Time.fixedDeltaTime / modifier;
            scale.y -= Time.fixedDeltaTime / modifier;
            scale.z -= Time.fixedDeltaTime / modifier;
            newDeltaGO.transform.localScale = scale;
            yield return new WaitForEndOfFrame();            
        }

        newDeltaGO.transform.localScale = Vector3.zero;
        
    }

    private void UpdateComboQuadrant(params object[] currentQuadrant)
    {
        float finalVal = 0f;

        if(currentQuadrant[0].GetType() != typeof(float))
        {
            finalVal = (float)currentQuadrant[0];
        }
        else
        {
            finalVal = System.Convert.ToSingle(currentQuadrant[0]);
        }

        comboTimer.fillAmount = finalVal;

        if (finalVal == 0f)
        {
            SetAlphaOfText(onScreenCombo, 0.0f);
        }


    }

    private Sprite GetRandomSprite()
    {
        return glitchSprites[rng.Next(glitchSprites.Length)];
    }

    private void SetTimerValue(float i_timeRem, float i_timeMax)
    {
        float percRemain = i_timeRem / i_timeMax;
        float result = Mathf.Ceil(percRemain * 6.0f) * (1.0f / 6.0f);

        comboTimer.fillAmount = result;
    }
    

    private void UpdateScore(params object[] score)
    {
        onScreenScore.text = score[0].ToString();
        onScreenScoreDelta.text = "+" + score[1].ToString();
        onScreenScoreDelta.transform.localScale = Vector3.one;

        StartCoroutine(PopTextAndHide(onScreenScoreDelta.gameObject,1.0f));
    }

    private void UpdateCombo(params object[] currentCombo)
    {
        SetAlphaOfText(onScreenCombo, 1.0f);
        if ((int)currentCombo[0] > 0)
        {
            onScreenCombo.text = "x " + currentCombo[0].ToString();
            comboAnimation.Play();
        }
        else
        {
            if (onScreenCombo.color.a != 0f)
            {
                SetAlphaOfText(onScreenCombo, 0.0f);
            }
        }
    }

    private void ShowWaveText(params object[] currentWave)
    {
        float mod = 0f;
        if (currentWave[0].ToString() == "0")
        {
            waveCompleteText.text =
                Strings.TextStrings.FLAVOR_TEXT[Random.Range(0, Strings.TextStrings.FLAVOR_TEXT.Length-1)];
            mod = 4f;
        }
        else
        {
            waveCompleteText.text = "WAVE " + currentWave[0].ToString() + " COMPLETE";
            mod = 2f;
        }
        
        waveCompleteText.transform.localScale = Vector3.one;
        StartCoroutine(PopTextAndHide(waveCompleteText.gameObject,mod));
    }

    private void SetAlphaOfText(Text i_toMod, float i_newAlpha)
    {
        Color c = i_toMod.color;
        c.a = i_newAlpha;
        i_toMod.color = c;
    }

    private void HideTutorialText(object[] parameters)
    {
        eatTutorialTextElement.enabled = false;
        dashTutorialTextElement.enabled = false;
        FullScreenFadeElement.enabled = false;
        tutorialBG.enabled = false;
    }

    private void ShowTutorialText(object[] parameters)
    {
        tutorialBG.enabled = true;
        int eatOrDash = (int)parameters[0];
        eatTutorialTextElement.enabled = false;
        dashTutorialTextElement.enabled = false;

        if (eatOrDash == 1)
        {
            //eat
            eatTutorialTextElement.enabled = true;
        }
        else
        {
            //dash
            dashTutorialTextElement.enabled = true;
        }

    }

    private void HideHUD(object[] parameters) {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 2.0f, hudCG, null));
    }

    private void ShowHUD(object[] parameters)
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 2.0f, hudCG, null));
        
    }

    #endregion
}
