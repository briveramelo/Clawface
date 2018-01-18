using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;

public class MainPLEMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return initiallySelected;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private LevelEditor editorInstance;

    #endregion

    #region Public Interface

    public MainPLEMenu() : base(Strings.MenuStrings.MAIN_PLE_MENU)
    { }

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

    #region Private Interface

    public void OpenPropsAction()
    {
        AddPropsMenu menu = editorInstance.GetMenu(PLEMenu.PROPS) as AddPropsMenu;
        menu.adding = true;
        MenuManager.Instance.DoTransition(menu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        

    }

    public void OpenSpawnsAction()
    {
        AddSpawnsMenu menu = editorInstance.GetMenu(PLEMenu.SPAWN) as AddSpawnsMenu;
        MenuManager.Instance.DoTransition(menu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });

    }

    public void OpenFloorSystemAction()
    {
        SetDynamicLevelMenu menu = editorInstance.GetMenu(PLEMenu.DYNAMIC) as SetDynamicLevelMenu;
        MenuManager.Instance.DoTransition(menu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });

    }

    public void TestLevelAction()
    {

    }

    public void DuplicateToolAction()
    {

    }

    public void QuitAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = menu as LoadMenu;
        loadMenu.TargetScene = Strings.Scenes.MainMenu;

        MenuManager.Instance.DoTransition(loadMenu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    #endregion

}
