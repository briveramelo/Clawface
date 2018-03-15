using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDAnimator : MonoBehaviour
{
    const float BOUNCE_THRESHOLD = 0.1f;

    [SerializeField]
    RectTransform combo;
    [SerializeField]
    RectTransform score;
    [SerializeField] RectTransform health;

    [SerializeField] BounceSettings comboSettings;
    [SerializeField] BounceSettings scoreSettings;
    [SerializeField] BounceSettings healthSettings;

    Vector3 comboOriginalScale, scoreOriginalScale, healthOriginalScale;

    [SerializeField] Color comboOriginalColor, scoreOriginalColor, healthOriginalColor;

    float comboScale, comboScaleVelocity;
    float scoreScale, scoreScaleVelocity;
    float healthScale, healthScaleVelocity;

    [SerializeField]
    MaskableGraphic[] comboGraphics, scoreGraphics, healthGraphics;

    Coroutine comboBounce, scoreBounce, healthBounce;

    private void Awake()
    {
        comboOriginalScale = combo.localScale;
        scoreOriginalScale = score.localScale;
        healthOriginalScale = health.localScale;

        EventSystem.Instance.RegisterEvent(Strings.Events.MULTIPLIER_UPDATED, BounceCombo);
        EventSystem.Instance.RegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_DAMAGED, BounceHealth);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, HandleLevelStarted);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, HandleLevelRestarted);
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.MULTIPLIER_UPDATED, BounceCombo);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_DAMAGED, BounceHealth);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, HandleLevelStarted);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_RESTARTED, HandleLevelRestarted);
        }
    }

    void BounceCombo(params object[] parameters)
    {
        if (comboBounce != null) StopCoroutine(comboBounce);
        comboBounce = StartCoroutine(DoBounce(combo, comboSettings));
    }

    void BounceScore(params object[] parameters)
    {
        if (scoreBounce != null) StopCoroutine(scoreBounce);
        scoreBounce = StartCoroutine(DoBounce(score, scoreSettings));
    }

    void BounceHealth(params object[] parameters)
    {
        if (healthBounce != null) StopCoroutine(healthBounce);
        healthBounce = StartCoroutine(DoBounce(health, healthSettings));
    }

    IEnumerator DoBounce (RectTransform tr, BounceSettings settings, float multiplier=1.0f)
    {
        float t = 0.0f;
        float landRotation = Random.Range(-settings.rotationOffset, settings.rotationOffset) * multiplier;
        float originalRotation = tr.localEulerAngles.z;
        while (originalRotation > 180.0f) originalRotation -= 360.0f;
        while (originalRotation < -180.0f) originalRotation += 360.0f;

        Vector3 originalScale = Vector3.one;
        Color originalColor = Color.white;
        Color targetColor = Color.white;
        MaskableGraphic[] graphics = null;
        if (tr == combo)
        {
            originalScale = comboOriginalScale;
            originalColor = comboOriginalColor;
            graphics = comboGraphics;
        }
        else if (tr == score)
        {
            originalScale = scoreOriginalScale;
            originalColor = scoreOriginalColor;
            graphics = scoreGraphics;
        }
        else if (tr == health)
        {
            originalScale = healthOriginalScale;
            originalColor = healthOriginalColor;
            graphics = healthGraphics;
        }

        while (t <= 1.0f)
        {
            t += Time.deltaTime * settings.speed;

            float scaleVal = multiplier * settings.amplitude * Mathf.Sin (t * Mathf.PI) + originalScale.x;
            tr.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
            tr.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp (originalRotation, landRotation, t));

            if (multiplier == 1.0f)
            {
                Color color = Color.Lerp (originalColor, settings.bounceColor, 1.0f - t);
                foreach (MaskableGraphic graphic in graphics)
                {
                    graphic.color = color;
                }
            }
 
            yield return null;
        }

        if (multiplier >= BOUNCE_THRESHOLD)
            yield return DoBounce(tr, settings, multiplier * settings.bounciness);
    }

    void HandleLevelRestarted (params object[] parameters)
    {
        ResetBounce();
    }

    void HandleLevelStarted(params object[] parameters)
    {
        ResetBounce();
    }

    void ResetBounce ()
    {
        combo.localRotation = score.localRotation = 
            health.localRotation = Quaternion.identity;
        combo.localScale = comboOriginalScale;
        comboGraphics[0].color = comboOriginalColor;
        score.localScale = scoreOriginalScale;
        health.localScale = healthOriginalScale;
        StopAllCoroutines();
    }

    [System.Serializable]
    struct BounceSettings
    {
        public float speed;
        public float amplitude;
        public float rotationOffset;
        public float bounciness;
        public Color bounceColor;
    }
}
