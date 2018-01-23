using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmittedGameObject : MonoBehaviour {

    protected Vector3 originalScale;

    protected float lifeTimer = 0.0f;

    protected float duration;

    protected Rigidbody rb;

    protected AnimationCurve scaleCurve;

    public void Init (float duration, AnimationCurve scaleCurve)
    {
        this.duration = duration;
        this.scaleCurve = scaleCurve;
    }

    protected virtual void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= duration)
        {
            Destroy(gameObject);
        }

        else
        {
            float scaleValue = scaleCurve.Evaluate(lifeTimer / duration);
            transform.localScale = originalScale * scaleValue;
        }
    }
}
