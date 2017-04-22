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
        Assert.IsTrue(duration > 0.0F);
        float elapsedTime = 0.0F;
        while (elapsedTime < duration)
        {
            canvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }
     

        canvas.alpha = endAlpha;
        

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
