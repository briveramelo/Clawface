//Garin
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
public class InGameUI : EventSubscriber {

    #region Public Fields
    #endregion

    #region Serialized Unity Inspector Fields
    [Header("Multiplier")]
    [SerializeField] private Text multiplierText;

    [Header("OnScreenCombo")]
    [SerializeField] private Text onScreenCombo;
    [SerializeField] private Image onScreenComboImage;
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
    [SerializeField] private Image orangeEatenImage;
    [SerializeField] private Image orangeCircleImage;

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

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.SCORE_UPDATED, UpdateScore },
                { Strings.Events.MULTIPLIER_UPDATED, UpdateMultiplier },
                { Strings.Events.COMBO_UPDATED, UpdateCombo },
                { Strings.Events.PLAYER_DAMAGED, DoDamageEffect },
                { Strings.Events.PLAYER_HEALTH_MODIFIED, SetHealth },
                { Strings.Events.SHOW_TUTORIAL_TEXT, ShowTutorialText },
                { Strings.Events.HIDE_TUTORIAL_TEXT, HideTutorialText },
                { Strings.Events.LEVEL_COMPLETED, HideHUD },
                { Strings.Events.LEVEL_FAILED, HideHUD },
                { Strings.Events.PLAYER_KILLED, HideHUD },
                { Strings.Events.WAVE_COMPLETE, ShowWaveText },
            };
        }
    }
    #endregion

    #region Unity Lifecycle

    protected override void Start()//
    {
        base.Start();
        multiplierText.text = "";
        onScreenCombo.text = "";
        onScreenScoreDelta.text = "";
        onScreenScore.text = "";
        waveCompleteText.text = "";
        UpdateCombo(null);
        HideTutorialText(null);
        UpdateMultiplier(ScoreManager.Instance.GetDifficultyMultiplier());
        ShowHUD(null);
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
        //healthMask.localScale = new Vector3(health, 1.0F, 1.0F);
        float healthBarScale;
        if (health == 0) //acounts for NaN cases...
        {
            healthBarScale = 0;
        }
        else
        {
            //healthBarScale = 1.0f / health;
            healthBarScale = health;
        }
        healthBar.localScale = new Vector3(healthBarScale, healthBar.localScale.y, healthBar.localScale.z);
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
            scale.x -= Time.deltaTime / modifier;
            scale.y -= Time.deltaTime / modifier;
            scale.z -= Time.deltaTime / modifier;
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
            SetAlphaOfImage(onScreenComboImage, 0.0f);
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

    private void UpdateMultiplier(params object[] parameters) {
        float multiplierValue = (float)parameters[0];
        multiplierText.text = "x"+multiplierValue.ToSimplestForm();
    }

    private void UpdateCombo(params object[] currentCombo)
    {
        SetAlphaOfText(onScreenCombo, 1.0f);
        SetAlphaOfImage(onScreenComboImage, 1.0f);

        int combo;

        if (currentCombo != null && currentCombo[0] != null)
        {
            combo = Mathf.FloorToInt((int)currentCombo[0]);
        }
        else
        {
            combo = Mathf.FloorToInt(0);
        }

        if (combo > 0)
        {
            onScreenCombo.text = combo.ToString();
            comboAnimation.Play();
        }
        else
        {
            if (onScreenCombo.color.a != 0f)
            {
                HideCombo();
            }
        }
    }

    void HideCombo (params object[] parameters)
    {
        SetAlphaOfText(onScreenCombo, 0.0f);
        SetAlphaOfImage(onScreenComboImage, 0.0f);
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

    private void SetAlphaOfImage(Image i_toMod, float i_newAlpha)
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
        orangeEatenImage.enabled = false;
        orangeCircleImage.enabled = false;
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
            InputManager.Binding currentBinding;
            currentBinding = InputManager.Instance.QueryBinding(Strings.Input.Actions.EAT);

            string key = "";
            switch (SettingsManager.Instance.MouseAimMode)
            {
                case MouseAimMode.AUTOMATIC:
                    if (!MenuManager.Instance.MouseMode)
                    {
                        key = currentBinding.joystick;
                    } else 
                    {
                        key = currentBinding.mouse != null ? currentBinding.mouse :         currentBinding.keyboard;
                    }
                    break;
                case MouseAimMode.ALWAYS_ON:
                    key = currentBinding.mouse != null ? currentBinding.mouse : currentBinding.keyboard;
                    break;
                case MouseAimMode.ALWAYS_OFF:
                    if (InputManager.Instance.HasJoystick())
                    {
                        key = currentBinding.joystick;
                    } else
                    {
                        key = currentBinding.keyboard;
                    }
                    break;
            }
            string[] words = key.Split(' ');
            if (words.Length > 1)
            {
                key = words[0][0].ToString() + words[1][0].ToString();
            }

            eatTutorialTextElement.text = eatTutorialTextElement.text.Replace("*",key.ToUpper()); 
            eatTutorialTextElement.enabled = true;
            orangeEatenImage.enabled = true;
            orangeCircleImage.enabled = true;
        }
        else
        {
            //dash
            InputManager.Binding currentBinding;
            currentBinding = InputManager.Instance.QueryBinding(Strings.Input.Actions.DODGE);

            string key = "";
            switch (SettingsManager.Instance.MouseAimMode)
            {
                case MouseAimMode.AUTOMATIC:
                    if (!MenuManager.Instance.MouseMode)
                    {
                        key = currentBinding.joystick;
                    }
                    else
                    {
                        key = currentBinding.mouse != null ? currentBinding.mouse : currentBinding.keyboard;
                    }
                    break;
                case MouseAimMode.ALWAYS_ON:
                    key = currentBinding.mouse != null ? currentBinding.mouse : currentBinding.keyboard;
                    break;
                case MouseAimMode.ALWAYS_OFF:
                    if (InputManager.Instance.HasJoystick())
                    {
                        key = currentBinding.joystick;
                    }
                    else
                    {
                        key = currentBinding.keyboard;
                    }
                    break;
            }
            string[] words = key.Split(' ');
            if (words.Length > 1)
            {
                key = words[0][0].ToString() + words[1][0].ToString();    
            }

            dashTutorialTextElement.text = dashTutorialTextElement.text.Replace("*",key.ToUpper());
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
