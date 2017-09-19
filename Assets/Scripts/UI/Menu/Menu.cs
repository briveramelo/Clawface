/**
 *  @author Cornelia Schultz
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour {

    #region Properties
    private string menuName;
    public string MenuName
    {
        get { return menuName; }
    }

    public abstract Button InitialSelection
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

    public CanvasGroup CanvasGroup
    {
        get
        {
            return canvasGroup;
        }
    }
    #endregion

    #region Serialized Unity Fields
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float faderDuration = 1F;
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Start()
    {
        canvasGroup.alpha = 0.0F;
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
                OnTransitionStarted(transition, effects);
                ShowStarted();
                CallByEffect(transition, effects);
                return;
            case Transition.HIDE:
                if (!Displayed) return;
                OnTransitionStarted(transition, effects);
                HideStarted();
                CallByEffect(transition, effects);
                return;
            case Transition.TOGGLE:
                DoTransition(Displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
            case Transition.SPECIAL:
                OnTransitionStarted(transition, effects);
                SpecialStarted();
                Special(transition, effects);
                return;
        }
    }
    #endregion

    #region Protected Interface
    protected virtual void OnTransitionStarted(Transition transition, Effect[] effects)
    {
        if (TransitionStarted != null)
            TransitionStarted(transition, effects);
    }
    protected virtual void OnTransitionEnded(Transition transition, Effect[] effects)
    {
        if (TransitionEnded != null)
            TransitionEnded(transition, effects);

        if (InitialSelection != null && Displayed) // menu is now visible (it wasn't before)
        {
            InitialSelection.Select();
        }
    }

    //// Helper Functions for Transitioning between menus
    // Default Show / Hide
    protected abstract void DefaultShow(Transition transition, Effect[] effects);
    protected abstract void DefaultHide(Transition transition, Effect[] effects);

    // Effect Based Implementations
    protected virtual void Fade(Transition transition, Effect[] effects)
    {
        float start = (transition == Transition.SHOW) ? 0F : 1F;
        float end = (transition == Transition.SHOW) ? 1F : 0F;
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(start, end, faderDuration,
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
    protected virtual void ShowStarted() { }
    protected virtual void ShowComplete()
    {
        displayed = true;
    }
    protected virtual void HideStarted() { }
    protected virtual void HideComplete()
    {
        displayed = false;
    }
    protected virtual void SpecialStarted() { }
    protected virtual void SpecialComplete() { } 
    #endregion

    #region Private Interface

    private void CallByEffect(Transition transition, Effect[] effects)
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
        } else
        {
            DefaultHide(transition, effects);
        }
    }

    #endregion

    #region Types
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

    public delegate void TransitionStartedEventHandler(Transition transition,
            Effect[] effects);
    public delegate void TransitionEndedEventHandler(Transition transition,
            Effect[] effects);

    public event TransitionStartedEventHandler TransitionStarted;
    public event TransitionEndedEventHandler TransitionEnded;
    #endregion
}
