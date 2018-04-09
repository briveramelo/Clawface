using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    #region Public Fields

    public override Selectable InitialSelection { get { return mainButton; } }

    #endregion

    #region Serialized Unity Fields

    [SerializeField]
    private Button mainButton;

    #endregion

    #region Public Interface

    public MainMenu() : base(Strings.MenuStrings.MAIN) {}
    public override MenuType ThisMenuType { get { return MenuType.Main; } }
    protected override void ShowStarted() {
        base.ShowStarted();        
    }


    //// Actions used by Buttons on this Menu
    public void StartAction()
    {
        // Target Level is hard coded right now.
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = menu as LoadMenu;
        loadMenu.SetNavigation(Strings.Scenes.ScenePaths.Arena);

        // Transition to level Select
        menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU);
        PLELevelSelectMenu levelSelectMenu = menu as PLELevelSelectMenu;
        levelSelectMenu.backMenuTarget = Strings.MenuStrings.MAIN;
        MenuManager.Instance.DoTransition(levelSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        //SFXManager.Instance.Play(SFXType.AnnounceTitle, Vector3.zero);
    }

    public void EditorAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = menu as LoadMenu;
        loadMenu.SetNavigation(Strings.Scenes.ScenePaths.Editor);

        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void SettingsAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.SETTINGS,
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
}
