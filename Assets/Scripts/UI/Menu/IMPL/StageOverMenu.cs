/**
*  @author Cornelia Schultz
*/

using UnityEngine;
using UnityEngine.UI;

public class StageOverMenu : Menu
{
    #region Public Fields
    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }
    #endregion

    #region Serialized Unity Fields

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Text score;

    [SerializeField]
    private Text combo;

    #endregion

    #region Private Fields

    private bool displayed;

    #endregion

    #region Public Interface
    public StageOverMenu() : base(Strings.MenuStrings.STAGE_OVER) { }

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition) {
            case Transition.SHOW:
                UpdateScores();
                MenuManager.Instance.DoTransitionOthers(this, Transition.HIDE,
                    new Effect[] { Effect.FADE });
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0F, 1.0F, 1.0F,
                    canvasGroup, ShowComplete));
                break;
            case Transition.HIDE:
                if (!displayed) return;
                OnTransitionStarted(transition, effects);
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0F, 0.0F, 1.0F,
                    canvasGroup, HideComplete));
                break;
            case Transition.TOGGLE:
                DoTransition(Displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
        }
    }

    public void QuitAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.TargetScene = Strings.Scenes.MainMenu;
        loadMenu.Fast = true;
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }
    #endregion

    #region Private Interface

    private void UpdateScores()
    {
        score.text = ScoreManager.Instance.GetScore().ToString();
        combo.text = ScoreManager.Instance.GetHighestCombo().ToString();
    }

    private void ShowComplete()
    {
        displayed = true;
        quitButton.Select();
    }

    private void HideComplete()
    {
        displayed = false;
    }

    #endregion
}
