using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingMarker : MonoBehaviour {

    const float MAX_ALPHA = 0.25f;
    SpriteRenderer spriteRenderer;
    TransformMemento initialTransform = new TransformMemento();
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        initialTransform.Initialize(transform);
    }

    private void OnEnable()
    {
        StartCoroutine(FadeIn());        
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FadeIn ()
    {
        float t = 0.0f;
        initialTransform.Reset(transform);

        while (t <= 1.0f)
        {
            t += Time.deltaTime;

            Color color = spriteRenderer.color;
            color.a = t * MAX_ALPHA;
            spriteRenderer.color = color;

            yield return null;
        }
    }
}
