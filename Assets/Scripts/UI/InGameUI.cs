//Garin
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;
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

    [Header("Score")]
    [SerializeField] private Text onScreenScore;
    [SerializeField] private Text onScreenScoreDelta;

    [Header("HealthBar")]
    [SerializeField] private Transform healthMask;
    [SerializeField] private Transform healthBar;

    [Header("GlitchDamageEffect")]
    [SerializeField] private Sprite[] glitchSprites;
    
    [SerializeField] private float glitchSeconds = 1.0F;
    [SerializeField] private int glitchesPerSecond = 5;
    [SerializeField] private Image overlay;
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
        }
        onScreenCombo.text = "";
        onScreenScoreDelta.text = "";
        onScreenScore.text = "";
    }
    private void OnDestroy()
    {

        EventSystem instance = EventSystem.Instance;

        if (instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCORE_UPDATED, UpdateScore);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_UPDATED, UpdateCombo);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_DAMAGED, DoDamageEffect);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_HEALTH_MODIFIED, SetHealth);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_TIMER_UPDATED, UpdateComboQuadrant);
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

    private IEnumerator ShowScoreDelta(GameObject newDeltaGO)
    {

        Vector3 scale = newDeltaGO.transform.localScale;
        while (scale.x > 0f)
        {
            scale = newDeltaGO.transform.localScale;
            scale.x -= Time.fixedDeltaTime;
            scale.y -= Time.fixedDeltaTime;
            scale.z -= Time.fixedDeltaTime;
            newDeltaGO.transform.localScale = scale;
            yield return new WaitForEndOfFrame();            
        }

        newDeltaGO.transform.localScale = Vector3.zero;

        //Destroy(newDeltaGO);
        //yield return new WaitForEndOfFrame();
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
        //[0] new total [1] delta
        onScreenScore.text = score[0].ToString();
        onScreenScoreDelta.text = "+" + score[1].ToString();
        onScreenScoreDelta.transform.localScale = Vector3.one;
        //GameObject newGO = Instantiate(onScreenScoreDelta.gameObject);
        //onScreenScoreDelta.text = "";
        //newGO.transform.SetParent(onScreenScoreDelta.transform);
        //newGO.transform.localPosition = Vector3.zero;

        StartCoroutine(ShowScoreDelta(onScreenScoreDelta.gameObject));
    }

    private void UpdateCombo(params object[] currentCombo)
    {
        SetAlphaOfText(onScreenCombo, 1.0f);
        if ((int)currentCombo[0] > 0)
        {
            onScreenCombo.text = currentCombo[0].ToString();
        }
        else
        {
            if (onScreenCombo.color.a != 0f)
            {
                SetAlphaOfText(onScreenCombo, 0.0f);
            }
        }
    }

    private void SetAlphaOfText(Text i_toMod, float i_newAlpha)
    {
        Color c = i_toMod.color;
        c.a = i_newAlpha;
        i_toMod.color = c;
    }
    #endregion
}
