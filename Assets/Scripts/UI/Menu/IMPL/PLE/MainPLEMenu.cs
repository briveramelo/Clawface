using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;
using System.Collections.Generic;
using System.Linq;

public class MainPLEMenu : Menu {

    public MainPLEMenu() : base(Strings.MenuStrings.MAIN_PLE_MENU) { }

    #region Public Fields

    public override Button InitialSelection {
        get {
            return initiallySelected;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private LevelEditor editorInstance;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle firstToggle;
    [SerializeField] private Toggle floorToggle, propsToggle, spawnsToggle, waveToggle, testToggle, saveToggle, loadToggle, helpToggle, exitToggle;
    #endregion

    #region Private Fields
    private List<Toggle> allToggles = new List<Toggle>();
    private bool inputGuard = false;

    #endregion  

    #region Unity Lifecycle

    protected override void Start() {
        base.Start();
        SetupToggleInteractability();
    }

    void SetupToggleInteractability() {
        allToggles.ForEach(toggle => { toggle.interactable = false; });
        floorToggle.interactable = true;
        loadToggle.interactable = true;
        helpToggle.interactable = true;
        exitToggle.interactable = true;
    }

    private void Update() {
        if (inputGuard) {

            //if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP))
            //{
            //    QuitAction();
            //}

        }
    }

    #endregion

    #region Private interface
    Toggle GetToggle(PLEMenu menu) {
        switch (menu) {
            case PLEMenu.FLOOR: return floorToggle;
            case PLEMenu.PROPS: return propsToggle;
            case PLEMenu.SPAWN: return spawnsToggle;
            case PLEMenu.WAVE: return waveToggle;
            case PLEMenu.TEST: return testToggle;
            case PLEMenu.SAVE: return saveToggle;
            case PLEMenu.LEVELSELECT: return loadToggle;
            case PLEMenu.HELP: return helpToggle;
        }
        return floorToggle;
    }
    #endregion

    #region Public Interface
    public void ToggleMenuInteractable(PLEMenu menu, bool isInteractable) {
        Toggle toggle = GetToggle(menu);
        toggle.interactable = isInteractable;
    }

    public void LoadLevel() {
        toggleGroup.SetAllTogglesOff();
        firstToggle.isOn = true;
    }

    public void OpenFloorSystemAction() {
        editorInstance.SwitchToMenu(PLEMenu.FLOOR);
    }

    public void OpenPropsAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.PROPS);
    }

    public void OpenSpawnsAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.SPAWN);
    }    

    public void OpenSaveAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.SAVE);
    }

    public void OpenLevelSelectAction() {
        editorInstance.SwitchToMenu(PLEMenu.LEVELSELECT);
    }

    public void OpenWaveAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.WAVE);
    }

    public void TestLevelAction()
    {
        editorInstance.SwitchToMenu(PLEMenu.TEST);
    }

    public void OpenHelpAction() {
        editorInstance.SwitchToMenu(PLEMenu.HELP);
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

    #region Protected Interface

    protected override void ShowComplete() {
        base.ShowComplete();
        inputGuard = true;
    }

    protected override void HideStarted() {
        base.HideStarted();
        inputGuard = false;
    }

    protected override void HideComplete() {
        base.HideComplete();
    }


    protected override void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    #endregion


}
