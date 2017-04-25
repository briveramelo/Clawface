using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserBase : MonoBehaviour {
    
    [SerializeField]
    private float fadeOutTime;

    private float initAlpha;
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        if (material.HasProperty("_Opacity"))
        {
            initAlpha = material.GetFloat("_Opacity");
        }
    }

    private void OnEnable()
    {
        material.SetFloat("_Opacity", initAlpha);
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeOutTime)
        {
            material.SetFloat("_Opacity", Mathf.Lerp(initAlpha, 0.0f, elapsedTime / fadeOutTime));
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.unscaledDeltaTime;
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        material.SetFloat("_Opacity", initAlpha);
    }
}
