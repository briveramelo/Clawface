﻿/**
 *  @author Cornelia Schultz
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMenu : Menu {

    #region Constants
    private static readonly float LOADING_TIME = 3.0F; // seconds
    #endregion

    #region Public Fields
    public string TargetScene
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }
    public bool Fast
    {
        get
        {
            return fast;
        }
        set
        {
            fast = value;
        }
    }

    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }

    public override Button InitialSelection
    {
        get
        {
            return null;
        }
    }

    #endregion

    #region Serialized Unity Fields
    [SerializeField]
    private Scrollbar loadingBar;
    [SerializeField]
    private Text loadingText;
    #endregion

    #region Private Fields
    private bool displayed = false;
    private bool loaded = false;
    private bool fast = false;
    private string target = "";
    #endregion

    #region Unity Lifecycle Methods
    void Update()
    {
        if (loaded && (InputManager.Instance.AnyKey() || fast))
        {
            fast = false;
            loaded = false;
            loadingText.text = "Starting...";
            MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { });
        }
    }
    #endregion

    #region Public Interface
    public LoadMenu() : base(Strings.MenuStrings.LOAD) {}

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.SHOW:
                if (displayed) return;
                MenuManager.Instance.DoTransitionOthers(this, Transition.HIDE,
                    new Effect[] { Effect.FADE });
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0F, 1.0F, 1.0F,
                    canvasGroup, () => { ShowComplete(); OnTransitionEnded(transition, effects); }));
                break;
            case Transition.HIDE:
                if (!displayed) return;
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0F, 0.0F, 1.0F,
                    canvasGroup, () => { HideComplete(); OnTransitionEnded(transition, effects); }));
                break;
            case Transition.TOGGLE:
                DoTransition(displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
        }
    }
    #endregion

    #region Private Interface
    // Callbacks
    private void ShowComplete()
    {
        displayed = true;
        StartCoroutine(LoadingCoroutine());
    }
    private void HideComplete()
    {
        displayed = false;
        loadingBar.size = 0.0F;
        loadingText.text = "Loading";
    }

    private IEnumerator LoadingCoroutine()
    {
        MovementEffects.Timing.KillCoroutines();
        ObjectPool.Instance.ResetPools();
        loadingBar.size = 0.0F;
        loaded = false;

        AsyncOperation async = SceneManager.LoadSceneAsync(target);
        while (!async.isDone)
        {
            loadingBar.size = Mathf.Lerp(0.0F, 1.0F, async.progress);
            yield return new WaitForEndOfFrame();
        }

        loadingBar.size = 1.0F;
        loadingText.text = "Press any key to continue...";
        loaded = true;
    }

    #endregion
}
