using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
public class WeaponSelectGlowController : RoutineRunner {

	[SerializeField] private Color confirmedColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color halfColor;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] new MeshRenderer renderer;
    private Color color;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        color = renderer.material.GetColor("_Tint");
    }

    public void SetConfirmed (bool instant=false)
    {
        Timing.KillCoroutines(coroutineName);
        if (instant) SetColorInstant(confirmedColor);
        else {
            Timing.RunCoroutine(SetColorFade (confirmedColor), coroutineName);
        }
    }

    public void Reset(bool instant=false)
    {
        Timing.KillCoroutines(coroutineName);
        if (instant) SetColorInstant(normalColor);
        else {
            Timing.RunCoroutine(SetColorFade (normalColor), coroutineName);
        }
    }

    void SetColor (Color color)
    {
        propBlock.SetColor ("_Tint", color);
        renderer.SetPropertyBlock(propBlock);        
    }

    void SetColorInstant (Color color)
    {
        SetColor (color);
    }

    public void SetUnselected(bool instant = false)
    {
        Timing.KillCoroutines(coroutineName);
        if (instant)
        {
            SetColorInstant(halfColor);
        }
        else
        {
            Timing.RunCoroutine(SetColorFade(halfColor), coroutineName);
        }
    }

    IEnumerator<float> SetColorFade (Color color)
    {
        Color startColor = this.color;
        float t = 0.0f;
        while (t < fadeTime)
        {
            SetColor (Color.Lerp(startColor, color, t / fadeTime));

            t += Time.deltaTime;
            yield return 0f;
        }

        this.color = color;
    }
}
