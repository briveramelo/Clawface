using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;
using System.Collections.Generic;
using System.Linq;
using ModMan;

public class MainPLEMenu : PlayerLevelEditorMenu {

    public MainPLEMenu() : base(Strings.MenuStrings.LevelEditor.MAIN_PLE_MENU) { }

    #region Public Fields

    #endregion

    #region Serialized Unity Fields
    [SerializeField] private SetTextColorHelper textColorSetter;
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
        Toggle selectedMenuToggle = menuToggles.Find(menuToggle => menuToggle.menu == menu).toggle;
        if (menu != levelEditor.currentDisplayedMenu) {
            menuToggles.ForEach(menuToggle => {
                menuToggle.toggle.onValueChanged.SwitchListenerState(UnityEngine.Events.UnityEventCallState.Off);
                menuToggle.toggle.isOn = false;
            });
            selectedMenuToggle.isOn = true;
            selectedMenuToggle.Select();
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectedMenuToggle.gameObject);

            levelEditor.SwitchToMenu(menu);
            textColorSetter.SetTextColor();
            menuToggles.ForEach(menuToggle => {
                menuToggle.toggle.onValueChanged.SwitchListenerState(UnityEngine.Events.UnityEventCallState.RuntimeOnly);
                menuToggle.spriteShifter.OnToggleChanged();
            });
        }
    }

    public void SetSelectedTextColor(PLEMenu i_selection)
    {

    }

    public void QuitAction()
    {        
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.TargetSceneName = Strings.Scenes.MainMenu;
        //loadMenu.Fast = true;
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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

class MenuToggle {
    public MenuToggle(PLEMenu menu, Toggle toggle) {
        this.menu = menu;
        this.toggle = toggle;
        spriteShifter = toggle.GetComponent<SpriteShifter>();
    }
    public PLEMenu menu;
    public Toggle toggle;
    public SpriteShifter spriteShifter;
}
