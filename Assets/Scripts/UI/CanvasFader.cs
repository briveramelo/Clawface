using UnityEngine;

public class CanvasFader : MonoBehaviour {

    #region Unity Serializations

    [SerializeField]
    private CanvasGroup canvasGroup;

    [Header("On Start Effect")]

    [SerializeField]
    private bool fadeOnStart = true;

    [SerializeField]
    private float startAlpha = 1.0F;

    [SerializeField]
    private float endAlpha = 0.0F;

    [SerializeField]
    private float duration = 1.0F;

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        canvasGroup.alpha = startAlpha;
    }

    private void Start () {
        if (fadeOnStart)
        {
            DoFade(startAlpha, endAlpha, duration, null);
        }
	}

    #endregion

    #region Public Interface

    public void DoFade(float startAlpha, float endAlpha, float duration,
        MenuTransitionsCommon.TransitionComplete callback)
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(startAlpha, endAlpha, duration,
            canvasGroup, callback));
    }

    public void DoShow(float duration, MenuTransitionsCommon.TransitionComplete callback)
    {
        DoFade(canvasGroup.alpha, 1.0F, duration, callback);
        canvasGroup.blocksRaycasts = true;
    }

    public void DoHide(float duration, MenuTransitionsCommon.TransitionComplete callback)
    {
        DoFade(canvasGroup.alpha, 0.0F, duration, () => { HideComplete(); if(callback != null) callback(); });
    }

    #endregion

    #region Private Interface

    private void HideComplete()
    {
        canvasGroup.blocksRaycasts = false;
    }

    #endregion
}
