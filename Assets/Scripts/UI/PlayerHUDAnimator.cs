﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDAnimator : MonoBehaviour
{
    const float BOUNCE_THRESHOLD = 0.1f;

    [SerializeField] RectTransform multiplierTrans, combo, score, health;
    [SerializeField] BounceSettings multiplierSettings, comboSettings, scoreSettings, healthSettings;

    Vector3 multiplierOriginalScale, comboOriginalScale, scoreOriginalScale, healthOriginalScale;

    [SerializeField] Color multiplierOriginalColor, comboOriginalColor, scoreOriginalColor, healthOriginalColor;

    [SerializeField] MaskableGraphic[] multiplierGraphics, comboGraphics, scoreGraphics, healthGraphics;

    Coroutine comboBounce, scoreBounce, healthBounce, multiplierBounce;

    private void Awake()
    {
        multiplierOriginalScale = multiplierTrans.localScale;
        comboOriginalScale = combo.localScale;
        scoreOriginalScale = score.localScale;
        healthOriginalScale = health.localScale;

        EventSystem.Instance.RegisterEvent(Strings.Events.COMBO_UPDATED, BounceCombo);
        EventSystem.Instance.RegisterEvent(Strings.Events.MULTIPLIER_UPDATED, BounceMultiplier);
        EventSystem.Instance.RegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_DAMAGED, BounceHealth);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, HandleLevelStarted);
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_RESTARTED, HandleLevelRestarted);
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_UPDATED, BounceCombo);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.MULTIPLIER_UPDATED, BounceMultiplier);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_DAMAGED, BounceHealth);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, HandleLevelStarted);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_RESTARTED, HandleLevelRestarted);
        }
    }

    void BounceMultiplier(params object[] parameters) {
        if (multiplierBounce != null) StopCoroutine(multiplierBounce);
        multiplierBounce = StartCoroutine(DoBounce(multiplierTrans, multiplierSettings));
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

    IEnumerator DoBounce (RectTransform tr, BounceSettings settings, float scaleMultiplier=1.0f)
    {
        float t = 0.0f;
        float landRotation = Random.Range(-settings.rotationOffset, settings.rotationOffset) * scaleMultiplier;
        float originalRotation = tr.localEulerAngles.z;
        while (originalRotation > 180.0f) originalRotation -= 360.0f;
        while (originalRotation < -180.0f) originalRotation += 360.0f;

        Vector3 originalScale = Vector3.one;
        Color originalColor = Color.white;
        Color targetColor = Color.white;
        MaskableGraphic[] graphics = null;
        if (tr == combo) {
            originalScale = comboOriginalScale;
            originalColor = comboOriginalColor;
            graphics = comboGraphics;
        }
        else if (tr == score) {
            originalScale = scoreOriginalScale;
            originalColor = scoreOriginalColor;
            graphics = scoreGraphics;
        }
        else if (tr == health) {
            originalScale = healthOriginalScale;
            originalColor = healthOriginalColor;
            graphics = healthGraphics;
        }
        else if (tr == multiplierTrans) {
            originalScale = multiplierOriginalScale;
            originalColor = multiplierOriginalColor;
            graphics = multiplierGraphics;
        }

        while (t <= 1.0f)
        {
            t += Time.deltaTime * settings.speed;

            float scaleVal = scaleMultiplier * settings.amplitude * Mathf.Sin (t * Mathf.PI) + originalScale.x;
            tr.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
            tr.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp (originalRotation, landRotation, t));

            if (scaleMultiplier == 1.0f)
            {
                Color color = Color.Lerp (originalColor, settings.bounceColor, 1.0f - t);
                foreach (MaskableGraphic graphic in graphics)
                {
                    graphic.color = color;
                }
            }
 
            yield return null;
        }

        if (scaleMultiplier >= BOUNCE_THRESHOLD) {
            yield return DoBounce(tr, settings, scaleMultiplier * settings.bounciness);
        }
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
        multiplierTrans.localRotation = combo.localRotation = score.localRotation = health.localRotation = Quaternion.identity;

        multiplierTrans.localScale = multiplierOriginalScale;
        combo.localScale = comboOriginalScale;
        score.localScale = scoreOriginalScale;
        health.localScale = healthOriginalScale;


        comboGraphics[0].color = comboOriginalColor;
        multiplierGraphics[0].color = multiplierOriginalColor;
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
