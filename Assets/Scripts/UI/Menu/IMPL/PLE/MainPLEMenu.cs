using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using ModMan;
using PlayerLevelEditor;

public class MainPLEMenu : PlayerLevelEditorMenu {

    public MainPLEMenu() : base(Strings.MenuStrings.LevelEditor.MAIN_PLE_MENU) { }

    #region Public Fields

    #endregion

    #region Serialized Unity Fields
    [SerializeField] private GroupTextColorSetter textColorSetter;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle firstToggle;
    [SerializeField] private Toggle floorToggle, propsToggle, spawnsToggle, waveToggle, testToggle, saveToggle, loadToggle, helpToggle, exitToggle;
    #endregion

    #region Private Fields
    private List<MenuToggle> menuToggles = new List<MenuToggle>();
    #endregion  

    #region Unity Lifecycle
    private void Awake() {
        InitializeMenuToggles();
        SetupToggleInteractability();
    }
    protected override void Start() {
        base.Start();
        PauseMenu pauseMenu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        pauseMenu.CanPause = false;
    }

    void InitializeMenuToggles() {        
        menuToggles.Add(new MenuToggle(PLEMenu.FLOOR, floorToggle));
        menuToggles.Add(new MenuToggle(PLEMenu.PROPS, propsToggle));
        menuToggles.Add(new MenuToggle(PLEMenu.SPAWN, spawnsToggle));
        menuToggles.Add(new MenuToggle(PLEMenu.WAVE, waveToggle));
        menuToggles.Add(new MenuToggle(PLEMenu.TEST, testToggle));        
        menuToggles.Add(new MenuToggle(PLEMenu.SAVE, saveToggle));
        menuToggles.Add(new MenuToggle(PLEMenu.LEVELSELECT, loadToggle));
        menuToggles.Add(new MenuToggle(PLEMenu.HELP, helpToggle));
    }

    void SetupToggleInteractability() {
        menuToggles.ForEach(menuToggle => { menuToggle.toggle.interactable = false; });
        floorToggle.interactable = true;
        loadToggle.interactable = true;
        helpToggle.interactable = true;
        exitToggle.interactable = true;
    }

    #endregion

    #region Public Interface
    public void SetMenuToggleOn(PLEMenu menu) {
        menuToggles.ForEach(menuToggle => { menuToggle.toggle.onValueChanged.SwitchListenerState(UnityEngine.Events.UnityEventCallState.Off); });
        menuToggles.Find(menuToggle => menuToggle.menu == menu).toggle.isOn = true;
        menuToggles.ForEach(menuToggle => { menuToggle.toggle.onValueChanged.SwitchListenerState(UnityEngine.Events.UnityEventCallState.RuntimeOnly); });
    }

    public void ToggleMenuInteractable(PLEMenu menu, bool isInteractable) {
        Toggle toggle = menuToggles.Find(item => item.menu == menu).toggle;
        toggle.interactable = isInteractable;
    }

    public void LoadLevel() {
        toggleGroup.SetAllTogglesOff();
        firstToggle.isOn = true;
    }

    public void OpenFloorSystemAction() {
        SelectMenuItem(PLEMenu.FLOOR);
    }

    public void OpenPropsAction()
    {
        SelectMenuItem(PLEMenu.PROPS);
    }

    public void OpenSpawnsAction()
    {
        SelectMenuItem(PLEMenu.SPAWN);
    }    

    public void OpenWaveAction()
    {
        SelectMenuItem(PLEMenu.WAVE);
    }

    public void OpenSaveAction()
    {
        SelectMenuItem(PLEMenu.SAVE);
    }

    public void OpenLevelSelectAction() {
        SelectMenuItem(PLEMenu.LEVELSELECT);
    }

    public void TestLevelAction()
    {
        SelectMenuItem(PLEMenu.TEST);
    }

    public void OpenHelpAction() {
        SelectMenuItem(PLEMenu.HELP);
    }
    public void SelectMenuItem(PLEMenu menu) {
        MenuToggle selectedMenuToggle = menuToggles.Find(menuToggle => menuToggle.menu == menu);
        Toggle selectedToggle = selectedMenuToggle.toggle;
        if (menu != levelEditor.currentDisplayedMenu) {
            menuToggles.ForEach(menuToggle => {
                menuToggle.toggle.onValueChanged.SwitchListenerState(UnityEngine.Events.UnityEventCallState.Off);
                menuToggle.toggle.isOn = false;
            });
            selectedToggle.isOn = true;
            selectedToggle.Select();
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectedToggle.gameObject);

            levelEditor.SwitchToMenu(menu);
            textColorSetter.SetColor();
            menuToggles.ForEach(menuToggle => {
                menuToggle.toggle.onValueChanged.SwitchListenerState(UnityEngine.Events.UnityEventCallState.RuntimeOnly);
                menuToggle.spriteShifter.OnToggleChanged();
                menuToggle.toggler.SetState(false);
            });
            selectedMenuToggle.toggler.SetState(true);
        }
    }
    public void QuitAction()
    {

        ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);

        Action onYesAction = () =>
        {
            Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
            LoadMenu loadMenu = (LoadMenu)menu;
            loadMenu.SetNavigation(Strings.Scenes.ScenePaths.MainMenu);
            MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        };

        Action onNoAction = () =>
        {
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { });
            SelectInitialButton();
        };

        confirmMenu.DefineActions("You will lose unsaved data, are you sure?", onYesAction, onNoAction);

        MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] { });

    }

    #endregion

    #region Protected Interface

    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
    }

    protected override void HideComplete() {
        base.HideComplete();
    }

    #endregion

    #region Private interface
    
    #endregion

}

[System.Serializable]
class MenuToggle {
    public MenuToggle(PLEMenu menu, Toggle toggle) {
        this.menu = menu;
        this.toggle = toggle;
        spriteShifter = toggle.GetComponent<SpriteShifter>();
        toggler= toggle.gameObject.GetComponentInChildren<GameObjectToggler>(true);
    }
    public PLEMenu menu;
    public Toggle toggle;
    public SpriteShifter spriteShifter;
    public GameObjectToggler toggler;
}
