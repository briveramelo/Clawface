using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectGlowController : MonoBehaviour {

	[SerializeField] Color confirmedColor;
    [SerializeField] Color normalColor;
    [SerializeField] float fadeTime = 0.5f;
    [SerializeField] new MeshRenderer renderer;
    Color color;
    MaterialPropertyBlock propBlock;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        color = renderer.material.GetColor("_Tint");
    }

    public void SetConfirmed (bool instant=false)
    {
        if (instant) SetColorInstant(confirmedColor);
        else StartCoroutine(SetColorFade (confirmedColor));
    }

    public void Reset(bool instant=false)
    {
        if (instant) SetColorInstant(normalColor);
        else StartCoroutine(SetColorFade (normalColor));
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

    IEnumerator SetColorFade (Color color)
    {
        Color startColor = this.color;
        float t = 0.0f;
        while (t < fadeTime)
        {
            SetColor (Color.Lerp(startColor, color, t / fadeTime));

            t += Time.deltaTime;
            yield return null;
        }

        this.color = color;
    }
}
