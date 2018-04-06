using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using ModMan;
public class WeaponSelectGlowController : RoutineRunner {

	[SerializeField] private Color confirmedColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color halfColor;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] new MeshRenderer renderer;
    const string TintColor = "_Tint";
    private Color currentColor;
    private MaterialPropertyBlock propBlock;
    private MaterialPropertyBlock PropBlock {
        get {
            if (propBlock==null) {
                propBlock = new MaterialPropertyBlock();
            }
            return propBlock;
        }
    }

    private void Awake()
    {        
        currentColor = renderer.material.GetColor(TintColor);
    }

    public void SetState(WeaponSelectState state, bool instant = false) {
        Color targetColor = GetColor(state);
        TriggerColorChange(targetColor, instant);
    }

    void TriggerColorChange(Color targetColor, bool instant) {
        Timing.KillCoroutines(CoroutineName);
        if (instant) {
            SetColorInstant(targetColor);
        }
        else if (!currentColor.IsAboutEqual(targetColor)) {
            Timing.RunCoroutine(SetColorFade(targetColor), CoroutineName);
        }
    }


    void SetColor (Color color)
    {
        currentColor = color;
        PropBlock.SetColor (TintColor, currentColor);
        renderer.SetPropertyBlock(PropBlock);
    }

    void SetColorInstant (Color color)
    {
        SetColor (color);
    }

    IEnumerator<float> SetColorFade (Color targetColor)
    {
        Color startColor = this.currentColor;
        float currentTimer = 0.0f;
        while (currentTimer < fadeTime)
        {
            Color newColor = Color.Lerp(startColor, targetColor, currentTimer / fadeTime);
            SetColor(newColor);
            currentTimer += Time.deltaTime;
            yield return 0f;
        }

        SetColor(targetColor);
    }

    private Color GetColor(WeaponSelectState state) {
        switch (state) {
            case WeaponSelectState.Confirmed:   return confirmedColor;
            case WeaponSelectState.Highlighted: return normalColor;
            case WeaponSelectState.Unselected:  return halfColor;
        }
        return normalColor;
    }
}
public enum WeaponSelectState {
    Confirmed,
    Highlighted,
    Unselected,
}