using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDAnimator : MonoBehaviour
{
    [SerializeField]
    RectTransform combo;
    [SerializeField]
    RectTransform score;

    [SerializeField] float comboBounceSpeed = 2.0f;
    [SerializeField] float comboBounceAmplitude = 1.0f;
    [SerializeField] float comboBounceLandOffset = 10.0f;

    Vector3 comboOriginalScale;
    Vector3 scoreOriginalScale;

    float comboScale;
    float comboScaleVelocity;

    float scoreScale;
    float scoreScaleVelocity;

    private void Awake()
    {
        comboOriginalScale = combo.localScale;
        scoreOriginalScale = score.localScale;

        EventSystem.Instance.RegisterEvent(Strings.Events.COMBO_UPDATED, BounceCombo);
        EventSystem.Instance.RegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
    }

    private void OnDestroy()
    {
        EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_UPDATED, BounceCombo);
        EventSystem.Instance.UnRegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
    }

    private void Update()
    {
        /*if (comboScale <= comboOriginalScale.x)
        {
            comboScale = comboOriginalScale.x;
            comboScaleVelocity = 0.0f;
        }

        comboScaleVelocity = Mathf.Clamp(comboScaleVelocity - Time.deltaTime, 0.0f, comboScaleVelocity);
        comboScale += comboScaleVelocity * Time.deltaTime;
        combo.localScale = new Vector3(comboScale, comboScale, comboScale);*/
    }

    void BounceCombo(params object[] parameters)
    {
        //comboScaleVelocity = 1.0f;
        StopCoroutine(DoBounceCombo(1.0f));
        StartCoroutine(DoBounceCombo(1.0f));
    }

    IEnumerator DoBounceCombo (float multiplier)
    {
        float t = 0.0f;
        float landRotation = Random.Range(-comboBounceLandOffset, comboBounceLandOffset);
        float originalRotation = combo.localEulerAngles.z;

        while (t <= 1.0f)
        {
            t += Time.deltaTime * comboBounceSpeed;

            float scaleVal = multiplier * comboBounceAmplitude * Mathf.Sin (t * Mathf.PI) + comboOriginalScale.x;
            combo.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
            combo.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp (originalRotation, landRotation, t));

            yield return null;
        }

        if (multiplier > 0.01f)
            yield return DoBounceCombo(multiplier * 0.2f);
    }

    void BounceScore(params object[] parameters)
    {
        scoreScaleVelocity = 1.0f;
    }
}
