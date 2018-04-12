using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;

public class PlayerHUDAnimator : EventSubscriber {
    const float BOUNCE_THRESHOLD = 0.1f;

    #region Serialized Fields
    [Header("BOUNCES DIMINISH each bounce as Bounciness * Bounciness > 0.1f")]
    [SerializeField] RectTransform multiplierTrans;
    [SerializeField] RectTransform combo, score, health;
    [SerializeField] BounceSettings multiplierSettings, comboSettings, negativeComboSettings, negativeMultiplierSettings, scoreSettings, healthSettings;
    [SerializeField] Color multiplierOriginalColor, comboOriginalColor, scoreOriginalColor, healthOriginalColor;
    [SerializeField] MaskableGraphic[] multiplierGraphics, comboGraphics, scoreGraphics, healthGraphics;
    #endregion


    #region Private Fields
    Vector3 multiplierOriginalScale, comboOriginalScale, scoreOriginalScale, healthOriginalScale;
    Coroutine comboBounce, scoreBounce, healthBounce, multiplierBounce;
    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.AwakeDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.COMBO_UPDATED, BounceCombo },
                { Strings.Events.MULTIPLIER_UPDATED, BounceMultiplier },
                { Strings.Events.SCORE_UPDATED, BounceScore },
                { Strings.Events.PLAYER_DAMAGED, BounceHealth },
                { Strings.Events.LEVEL_STARTED, HandleLevelStarted },
                { Strings.Events.LEVEL_RESTARTED, HandleLevelRestarted },
            };
        }
    }
    #endregion

    #region Unity LifeCycle
    protected override void Awake()
    {
        multiplierOriginalScale = multiplierTrans.localScale;
        comboOriginalScale = combo.localScale;
        scoreOriginalScale = score.localScale;
        healthOriginalScale = health.localScale;

        base.Awake();
    }
    #endregion

    #region Private Interface
    void BounceMultiplier(params object[] parameters) {
        if (multiplierBounce != null) StopCoroutine(multiplierBounce);
        bool useNegative = ((float)(parameters[0])).AboutEqual(ScoreManager.Instance.GetDifficultyMultiplier());
        BounceSettings settings = useNegative ? negativeMultiplierSettings : multiplierSettings;
        //TODO add sound effect for negative effect SFXManager.Instance.Play(SFXType.Score, transform.position);
        multiplierBounce = StartCoroutine(DoBounce(multiplierTrans, settings));
    }

    void BounceCombo(params object[] parameters)
    {
        if (comboBounce != null) StopCoroutine(comboBounce);
        bool useNegative = (int)parameters[0] == 0;
        BounceSettings settings = useNegative ? negativeComboSettings : comboSettings;
        comboBounce = StartCoroutine(DoBounce(combo, settings));
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
                float lerpProgress = settings.colorForward ? t : 1.0f-t;
                Color color = Color.Lerp (originalColor, settings.bounceColor, lerpProgress);
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
        StopAllCoroutines();

        multiplierTrans.localRotation = combo.localRotation = score.localRotation = health.localRotation = Quaternion.identity;

        multiplierTrans.localScale = multiplierOriginalScale;
        combo.localScale = comboOriginalScale;
        score.localScale = scoreOriginalScale;
        health.localScale = healthOriginalScale;

        comboGraphics[0].color = comboOriginalColor.ChangeAlpha(0f);
        multiplierGraphics[0].color = multiplierOriginalColor;
    }
    #endregion

    #region Internal Structures
    [System.Serializable]
    struct BounceSettings
    {
        public float speed;
        public float amplitude;
        public float rotationOffset;
        public float bounciness;
        public Color bounceColor;
        public bool colorForward;
    }
    #endregion

}
