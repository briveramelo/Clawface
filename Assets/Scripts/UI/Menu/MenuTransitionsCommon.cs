/**
 *  @author Cornelia Schultz
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class MenuTransitionsCommon {

    #region Public Interface
    public static IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration,
            CanvasGroup canvas, TransitionComplete callback)
    {
        //turn off event system
        EventSystem.current.GetComponent<StandaloneInputModule>().DeactivateModule();
        Assert.IsTrue(duration > 0.0F);
        float elapsedTime = 0.0F;
        while (elapsedTime < duration)
        {
            canvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }
     

        canvas.alpha = endAlpha;

        //turn on event system
        EventSystem.current.GetComponent<StandaloneInputModule>().ActivateModule();

        if (callback != null)
        {
            callback();
        }

    }
    #endregion

    #region Types
    public delegate void TransitionComplete();
    #endregion
}
