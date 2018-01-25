using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class ExplosionTrigger : RoutineRunner {

    private float scaleRate;
    private float blastRadius;

    public void Initialize(float p_blastRadius, float p_scaleRate)
    {
        blastRadius = p_blastRadius;
        scaleRate = p_scaleRate;
    }

    public void OnEnable()
    {
        Timing.RunCoroutine(CreateExplosionSphere(), coroutineName);
    }

    private IEnumerator<float> CreateExplosionSphere()
    {
        while (gameObject.transform.localScale.x < blastRadius)
        {
            gameObject.transform.localScale += new Vector3(1f, 0.0f, 1f) * Time.deltaTime * (blastRadius/scaleRate);
            yield return 0.0f;
        }

        yield return Timing.WaitForSeconds(0.25f);

        gameObject.SetActive(false);
        gameObject.transform.localScale = new Vector3(0.0f, 0.1f, 0.0f);
    }

}
