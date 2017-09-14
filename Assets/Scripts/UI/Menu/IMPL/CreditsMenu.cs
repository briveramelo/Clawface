using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : Menu
{
    #region Public Fields

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
            return returnButton;
        }
    }

    #endregion

    #region Unity Serilization Fields

    [SerializeField]
    private Button returnButton;

    #endregion

    #region Private Fields

    private bool displayed = false;

    #endregion

    #region Public Interface

    public CreditsMenu() : base(Strings.MenuStrings.CREDITS) {}

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.HIDE:
                if (!displayed) return;
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0F, 0.0F, 1.0F,
                    canvasGroup, () => { HideComplete(); OnTransitionEnded(transition, effects); }));
                break;
            case Transition.SHOW:
                if (displayed) return;
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0F, 1.0F, 1.0F,
                    canvasGroup, () => { ShowComplete(); OnTransitionEnded(transition, effects); }));
                break;
            case Transition.TOGGLE:
                DoTransition(displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
        }
    }

    public void BackAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN,
            Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    #endregion

    #region Private Interface

    private void HideComplete()
    {
        displayed = false;
    }

    private void ShowComplete()
    {
        displayed = true;
    }

    #endregion
}
