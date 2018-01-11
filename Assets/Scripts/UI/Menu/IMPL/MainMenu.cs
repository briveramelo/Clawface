using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return mainButton;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField]
    private Button mainButton;

    #endregion

    #region Public Interface

    public MainMenu() : base(Strings.MenuStrings.MAIN) {}

    //// Actions used by Buttons on this Menu
    public void StartAction()
    {
        // Target Level is hard coded right now.
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = menu as LoadMenu;
        loadMenu.TargetScene = Strings.Scenes.Arena;        

        // Transition to weapon select
        // TODO - When adding level editor + more levels will need new and improved level select.
        menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
        WeaponSelectMenu weaponMenu = menu as WeaponSelectMenu;
        weaponMenu.menuTarget = Strings.MenuStrings.MAIN;
        MenuManager.Instance.DoTransition(weaponMenu, Transition.SHOW,
            new Effect[] { Effect.EXCLUSIVE });
    }

    public void ControlsAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.CONTROLS,
            Transition.SHOW, new Effect[] { Effect.EXCLUSIVE, Effect.FADE });
    }

    public void CreditsAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.CREDITS,
            Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void QuitAction()
    {
        Application.Quit();
    }

    #endregion

    #region Protected Interface

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion
}
