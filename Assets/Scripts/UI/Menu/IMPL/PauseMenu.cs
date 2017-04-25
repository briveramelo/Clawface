/**
 *  @author Cornelia Schultz
 */

using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu {

    #region Public Fields
    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }
    public bool CanPause
    {
        get
        {
            return canPause;
        }
        set
        {
            canPause = value;
        }
    }

    #endregion

    #region Serialized Unity Fields
    [SerializeField]
    private Button restartButton;
    #endregion

    #region Private Fields
    private bool displayed = false;
    private bool paused = false;
    private bool canPause = false;
    #endregion

    #region Unity Lifecycle Methods
    void Update()
    {
        if (canPause && InputManager.Instance.QueryAction(Strings.Input.Actions.PAUSE,
            ButtonMode.DOWN))
        {
            if (!paused)
            {
                MenuManager.Instance.DoTransition(this, Transition.TOGGLE, new Effect[] { });
            } else
            {
                MenuManager.Instance.ClearMenus();
            }
        }
    }
    #endregion

    #region Public Interface
    public PauseMenu() : base(Strings.MenuStrings.PAUSE) {}

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.SHOW:
                if (displayed) return;
                TogglePaused();
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
                DoTransition(displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
        }
    }

    public void restartAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.TargetScene = Strings.Scenes.Level1;
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void quitAction()
    {
        canPause = false;
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.TargetScene = Strings.Scenes.MainMenu;
        loadMenu.Fast = true;
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }


  
    #endregion

    #region Private Interface
    private void TogglePaused()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
    }

    // Callbacks
    private void ShowComplete()
    {
        displayed = true;
        restartButton.Select();
    }
    private void HideComplete()
    {
        displayed = false;
        TogglePaused();
    }

    #endregion
}
