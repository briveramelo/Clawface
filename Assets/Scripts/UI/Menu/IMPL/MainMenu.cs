using UnityEngine;
using UnityEngine.UI;

public class MainMenu : PlayerLevelEditorMenu
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

    //// Actions used by Buttons on this Menu
    public void StartAction()
    {
        // Target Level is hard coded right now.
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = menu as LoadMenu;
        loadMenu.TargetSceneName = Strings.Scenes.Arena;        

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
        loadMenu.TargetSceneName = Strings.Scenes.Editor;
        loadMenu.Fast = true;

        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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

    public override void BackAction() {
        
    }

    #endregion
}
