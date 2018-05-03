/**
 *  @author Cornelia Schultz
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : EventSubscriber {

    #region Properties
    private string menuName;
    protected static UnityEngine.EventSystems.EventSystem CurrentEventSystem {get{ return UnityEngine.EventSystems.EventSystem.current; } }
    protected static GameObject CurrentEventSystemGameObject { get { return CurrentEventSystem.currentSelectedGameObject; } }
    protected static GameObject lastSelectedGameObject;    
    public abstract MenuType ThisMenuType { get; }

    public string MenuName
    {
        get { return menuName; }
    }

    public abstract Selectable InitialSelection
    {
        get;
    }
    public bool Displayed
    {
        get
        {
            return displayed;
        }
    }

    public GameObject Canvas
    {
        get
        {
            return canvas;
        }
    }

    public CanvasGroup CanvasGroup
    {
        get
        {
            return canvasGroup;
        }
    }
    #endregion

    #region Accessors (Protected)

    protected float FaderDuration
    {
        get
        {
            return faderDuration;
        }
    }
    protected bool allowInput;
    protected DisplayState currentState=DisplayState.HIDE_FINISHED;
    protected Coroutine transitionRoutine;
    #endregion

    #region Serialized Unity Fields
    [SerializeField]
    protected GameObject canvas;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float faderDuration = 0F;
    #endregion

    #region Unity Lifecycle Methods
    protected override void Start()
    {
        base.Start();
        faderDuration = 0f;
        canvasGroup.alpha = 0.0F;
    }

    protected virtual void LateUpdate() {
        if (Displayed) {
            lastSelectedGameObject = CurrentEventSystemGameObject;
            TryRegainCamera();
        }
    }
    #endregion

    #region Private Fields

    private bool displayed;
    #endregion

    #region Public Interface
    public Menu(string name)
    {
        menuName = name;
    }

    public void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.SHOW:
                if (Displayed) return;
                ShowStarted();
                TransitionWithEffects(transition, effects);
                return;
            case Transition.HIDE:
                if (!Displayed) return;
                HideStarted();
                TransitionWithEffects(transition, effects);
                return;
            case Transition.TOGGLE:
                MenuManager.Instance.DoTransition(this, Displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
            //case Transition.SPECIAL:
            //    SpecialStarted();
            //    Special(transition, effects);
            //    return;
        }
    }
    #endregion

    #region Internal Interface 

    internal void SelectInitialButton ()
    {
        if (InitialSelection != null && Displayed)
        {
            InitialSelection.Select();
            CurrentEventSystem.SetSelectedGameObject(InitialSelection.gameObject);
        }
    }

    #endregion

    #region Protected Interface
    protected virtual void OnTransitionEnded(Transition transition, Effect[] effects)
    {
        if (!MenuManager.Instance.MouseMode)
        {
            Invoke ("SelectInitialButton", 0.0f);
            //SelectInitialButton();
        }
    }

    //// Helper Functions for Transitioning between menus
    // Default Show / Hide
    protected virtual void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }
    protected virtual void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    // Effect Based Implementations
    protected virtual void Fade(Transition transition, Effect[] effects)
    {
        float startAlpha = (transition == Transition.SHOW) ? 0F : 1F;
        float endAlpha = (transition == Transition.SHOW) ? 1F : 0F;
        if (transitionRoutine!=null) {
            StopCoroutine(transitionRoutine);
        }
        transitionRoutine = StartCoroutine(MenuTransitionsCommon.FadeCoroutine(startAlpha, endAlpha, faderDuration,
            canvasGroup, () =>
            {
                if (transition == Transition.SHOW)
                {
                    ShowComplete();
                } else
                {
                    HideComplete();
                }

                OnTransitionEnded(transition, effects);
            }));
    }
    protected virtual void Tween(Transition transition, Effect[] effects)
    {
        throw new NotImplementedException();
    }
    protected virtual void Instant(Transition transition, Effect[] effects)
    {
        canvasGroup.alpha = (transition == Transition.SHOW) ? 1F : 0F;
        if (transition == Transition.SHOW)
        {
            ShowComplete();
        } else
        {
            HideComplete();
        }
        OnTransitionEnded(transition, effects);
    }
    protected virtual void Special(Transition transition, Effect[] effects)
    {
        throw new NotImplementedException();
    }

    // "Events" Used Internally by implementations
    protected virtual void ShowStarted()
    {
        currentState = DisplayState.SHOW_TRANSITIONING;
        displayed = true;
        canvas.SetActive(true);
        TryRegainCamera();
    }
    protected virtual void ShowComplete()
    {
        currentState = DisplayState.SHOW_FINISHED;        
        allowInput = true;
    }
    protected virtual void HideStarted() {
        allowInput = false;
        currentState = DisplayState.HIDE_TRANSITIONING;
    }
    protected virtual void HideComplete()
    {
        canvas.SetActive(false);
        displayed = false;
        currentState = DisplayState.HIDE_FINISHED;
    }
    protected virtual void SpecialStarted() { }
    protected virtual void SpecialComplete() { } 
    #endregion

    #region Private Interface

    private void TransitionWithEffects(Transition transition, Effect[] effects)
    {        
        foreach (Effect effect in effects)
        { // First come, first serve
            if (effect == Effect.FADE)
            {
                Fade(transition, effects);
                return;
            } else if (effect == Effect.TWEEN)
            {
                Tween(transition, effects);
                return;
            } else if (effect == Effect.INSTANT)
            {
                Instant(transition, effects);
                return;
            }
        }

        if (transition == Transition.SHOW)
        {
            DefaultShow(transition, effects);
        } else if(transition == Transition.HIDE)
        {
            DefaultHide(transition, effects);
        }
    }

    private void TryRegainCamera()
    {
        Canvas actualCanvas = canvas.GetComponent<Canvas>();
        if (actualCanvas != null)
        {
            actualCanvas.worldCamera = Camera.main;
            actualCanvas.planeDistance = Camera.main.nearClipPlane + 0.01F;
        }
    }

    #endregion

    #region Types
    public enum MenuType {
        Main,
        Credits,
        Load,
        Pause,
        Tutorial,
        StageOver,
        Settings,
        Controls,
        WeaponSelect,
        Leaderboards,
        LevelSelect,
        Confirm,
        Other
    }
    public enum DisplayState {
        SHOW_FINISHED,
        SHOW_TRANSITIONING,
        HIDE_TRANSITIONING,
        HIDE_FINISHED,
    }
    public enum Transition
    {
        SHOW,       // Reveals this menu
        HIDE,       // Hides this menu
        TOGGLE,     // Toggles the show state (shown -> hide; hidden -> show)
        SPECIAL,    // No visibilty transition.  Useful (maybe) if we add more effects
    }
    public enum Effect
    {
        EXCLUSIVE,  // Adds this menu WHILE dismissing other menus, the default is Additive

        // Some Useful Effect Types
        INSTANT,    // Do transition instantly
        FADE,       // Perform a fade on the transition
        TWEEN,      // Perform a motion tween transition (implementation defined)
    }

    public delegate void TransitionStartedEventHandler(Transition transition, Effect[] effects);
    public delegate void TransitionEndedEventHandler(Transition transition, Effect[] effects);    
    #endregion
}
