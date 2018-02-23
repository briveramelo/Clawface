using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;

public class MainPLEMenu : Menu
{

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

    #region Private Fields

    private bool inputGuard = false;

    #endregion  

    #region Unity Lifecycle

    private void Update()
    {
        if (inputGuard)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP))
            {
                QuitAction();
            }

        }
    }

    #endregion

    #region Public Interface

    public MainPLEMenu() : base(Strings.MenuStrings.MAIN_PLE_MENU)
    { }

    #endregion

    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
        editorInstance.gridController.currentEditorMenu = EditorMenu.MAIN_EDITOR_MENU;
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
    }

    protected override void HideComplete()
    {
        base.HideComplete();
    }


    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion

    #region Public Interface

    public void OpenPropsAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.PROPS);

    }

    public void OpenSpawnsAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.SPAWN);
    }

    public void OpenFloorSystemAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.DYNAMIC);

    }

    public void OpenSaveAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.SAVE);

    }

    public void OpenHelpAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.HELP);

    }

    public void OpenWaveAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.WAVE);

    }

    public void TestLevelAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.TEST);
    }

    public void LevelSelectAction() {
        editorInstance.SwitchToMenu(PLEMenu.LEVELSELECT);
    }

    public void SetSelectedTextColor(PLEMenu i_selection)
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
