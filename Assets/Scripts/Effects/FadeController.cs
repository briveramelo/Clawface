using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour {

    [SerializeField] float flashSpeed = 1.0f;
    [SerializeField] new SkinnedMeshRenderer renderer;

    Coroutine flashCoroutine;

	public void StartFlashing ()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(DoFlash());
            flashCoroutine = null;
        }
        renderer.material.SetFloat("_Opacity", 1.0f);
        flashCoroutine = StartCoroutine(DoFlash());
    }

    public void StopFlashing ()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(DoFlash());
            flashCoroutine = null;
        }
        renderer.material.SetFloat("_Opacity", 1.0f);
        StopAllCoroutines();
    }

    IEnumerator DoFlash ()
    {
        float t = 0.0f;

        while (true)
        {
            t += Time.deltaTime * flashSpeed;

            float alpha = Mathf.Cos (t * Mathf.PI);

            renderer.material.SetFloat("_Opacity", alpha);

            yield return null;
        }
    }
}
