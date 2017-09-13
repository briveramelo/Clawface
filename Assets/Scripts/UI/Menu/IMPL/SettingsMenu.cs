using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    #region Public Accessors
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
            return doneButton;
        }
    }
    #endregion

    #region Unity Serializations

    [SerializeField]
    private Button doneButton;

    #endregion

    #region Private Interface

    private bool displayed;

    #endregion

    #region Public Interface
    public SettingsMenu() : base(Strings.MenuStrings.SETTINGS) {}

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch(transition)
        {
            case Transition.SHOW:
                if (displayed) return;
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

    public void DoneEditingControls()
    {
        MenuManager.Instance.PopMenu(true);
    }
    #endregion

    #region Private Interface

    private void ShowComplete()
    {
        displayed = true;
    }

    private void HideComplete()
    {
        displayed = false;
    }

    #endregion
}
