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

        //TODO: Event System management. Make sure it's on or off when these transitions
        // are going on such that you aren't "selecting" on things when they are fading or hidden

        float elapsedTime = 0.0F;
        while (elapsedTime < duration)
        {
            ////Debug.Log("a: " + startAlpha);
            //startAlpha = Mathf.Lerp(startAlpha, endAlpha, .1f);
            //canvas.alpha = startAlpha;
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
