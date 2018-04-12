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
        while (t < 1.0f)
        {
            t += Time.deltaTime / duration;

            //Color color = renderer.material.color;
            //color.a = 1.0f - t;
            //renderer.material.color = color;

            if (t > 1.0f) break;

            transform.localScale = originalScale * Mathf.Sqrt(0.9f * (1.0f - t) + 0.1f);

            yield return null;
        }

        Destroy (gameObject);
    }
}
