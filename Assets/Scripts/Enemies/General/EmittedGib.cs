using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmittedGib : MonoBehaviour
{
    new SkinnedMeshRenderer renderer;
    Vector3 originalScale;

    private void Awake()
    {
        renderer = GetComponent<SkinnedMeshRenderer>();
        originalScale = transform.localScale;
        Invoke ("DoFadeOut", 1.0f);
    }

    public void DoFadeOut ()
    {
        StartCoroutine (FadeCoroutine(1.0f));
    }

    IEnumerator FadeCoroutine (float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {

            //Color color = renderer.material.color;
            //color.a = 1.0f - t;
            //renderer.material.color = color;            
            const float offset = .99f;
            transform.localScale = originalScale * Mathf.Sqrt(offset * (1.0f - t) + (1f- offset));
            t += Time.deltaTime;
            yield return null;
        }

        Destroy (gameObject);
    }
}
