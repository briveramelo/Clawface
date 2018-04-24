using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmittedGib : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Vector3 originalScale;

    private void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Transform transformToScale = skinnedMeshRenderer == null ? transform : skinnedMeshRenderer.rootBone;
        originalScale = transformToScale.localScale;
        StartCoroutine (FadeCoroutine(3.75f, .25f, transformToScale));
    }    

    IEnumerator FadeCoroutine (float waitTime, float shrinkTime, Transform transformToScale)
    {
        yield return new WaitForSeconds(waitTime);
        float t = 0.0f;
        while (t < shrinkTime)
        {
            const float offset = 0.99f;
            float progress = t / shrinkTime;
            transformToScale.localScale = originalScale * Mathf.Sqrt(offset * (1.0f - progress) + (1f- offset));
            t += Time.deltaTime;
            yield return null;
        }

        Destroy (gameObject);
    }
}
