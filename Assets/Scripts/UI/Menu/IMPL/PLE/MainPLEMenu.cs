using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using ModMan;
using PLE;

public class MainPLEMenu : PLEMenu {

    public MainPLEMenu() : base(Strings.MenuStrings.LevelEditor.MAIN_PLE_MENU) { }

    #region Serialized Unity Fields
    [Header ("Menus")]
    [SerializeField] private FloorMenu floorEditorMenu;
    [SerializeField] private PropsMenu propsEditorMenu;
    [SerializeField] private SpawnMenu spawnsEditorMenu;
    [SerializeField] private WaveMenu waveEditorMenu;
    [SerializeField] private TestMenu testEditorMenu;
    [SerializeField] private SaveMenu saveEditorMenu;
    [SerializeField] private PLELevelSelectMenu levelSelectEditorMenu;
    [SerializeField] private HelpMenu helpEditorMenu;
    [SerializeField] private ExitMenu exitMenu;

    [Header("Other things")]
    [SerializeField] private Selectable firstSelectable;
    [SerializeField] private Selectable floorItem, propsItem, spawnsItem, waveItem, testItem, saveItem, loadItem, helpItem, exitItem;
    [SerializeField] private RefsToggle helpSideBarToggler;
    #endregion

    #region Private Fields
    private List<PLEMenu> pleMenus = new List<PLEMenu>();
    private List<MenuSelectable> menuToggles = new List<MenuSelectable>();
    private PLEMenuType currentDisplayedMenu;
    #endregion  

    #region Unity Lifecycle
    protected override void Awake() {
        base.Awake();
        InitializeMenuToggles();
        SetupToggleInteractability();
    }
    protected override void Start() {
        base.Start();
        PauseMenu pauseMenu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        pauseMenu.CanPause = false;
        SetUpMenus();
    }
    #endregion

    #region Public Interface
    public void SetUpMenus() {
        pleMenus.Clear();
        pleMenus = new List<PLEMenu>() {
            floorEditorMenu,
            propsEditorMenu,
            spawnsEditorMenu,
            waveEditorMenu,
            testEditorMenu,
            saveEditorMenu,
            levelSelectEditorMenu,
            helpEditorMenu,
            exitMenu
        };
        //Hide menus that aren't need to be shown yet
        pleMenus.ForEach(menu => {
            MenuManager.Instance.DoTransition(menu, Transition.HIDE, new Effect[] { Effect.INSTANT });
            menu.Canvas.SetActive(false);
            menu.CanvasGroup.alpha = 0.0F;
            menu.CanvasGroup.blocksRaycasts = false;
            menu.CanvasGroup.interactable = false;
        });

        //show the main/floor menus
        MenuManager.Instance.DoTransition(this, Transition.SHOW, new Effect[] { Effect.INSTANT });
        MenuManager.Instance.DoTransition(floorEditorMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });

        currentDisplayedMenu = PLEMenuType.FLOOR;
        
        OpenFloorSystemAction();
        levelEditor.gridController.SetGridVisiblity(true);
        levelEditor.ToggleCameraController(true);
        SetMenuButtonInteractabilityByState();
    }

    public void SelectMenuItem(PLEMenuType menu) {
        SFXManager.Instance.Play(SFXType.UI_Click);
        MenuSelectable selectedMenuItem = menuToggles.Find(menuToggle => menuToggle.menu == menu);
        Selectable selectedSelectable = selectedMenuItem.selectable;
        if (menu != currentDisplayedMenu) {
            selectedSelectable.Select();
            SwitchToMenu(menu);
        }
        CurrentEventSystem.SetSelectedGameObject(selectedSelectable.gameObject);
    }

    public void HideAllMenusExceptMain() {
        pleMenus.ForEach(menu => {
            if (menu.MenuName != Strings.MenuStrings.LevelEditor.MAIN_PLE_MENU) {
                MenuManager.Instance.DoTransition(menu, Transition.HIDE, new Effect[] { Effect.INSTANT });
            }
        });
    }

    public void DeselectAllBlocks() {
        levelEditor.gridController.ClearSelectedBlocks();
    }

    /// <summary>
    /// All Menus Update, including the main menu
    /// </summary>
    public override void SetMenuButtonInteractabilityByState() {
        bool anyTilesOn = levelEditor.gridController.AnyTilesActive();
        ToggleMenuInteractable(anyTilesOn, PLEMenuType.PROPS, PLEMenuType.SPAWN, PLEMenuType.WAVE);

        bool anyTilesOnAndPlayerOn = anyTilesOn && SpawnMenu.playerSpawnInstance != null;        

        levelEditor.TryCreateAllWaveParents();
        levelEditor.levelDataManager.SyncWorkingSpawnData();

        bool allWavesHaveEnemies = levelEditor.levelDataManager.WorkingLevelData.AllWavesHaveEnemies(PLESpawnManager.Instance.MaxWaveIndex);
        bool playerOnTilesOnAndAllWavesHaveEnemies = anyTilesOnAndPlayerOn && allWavesHaveEnemies;
        ToggleMenuInteractable(playerOnTilesOnAndAllWavesHaveEnemies, PLEMenuType.SAVE, PLEMenuType.TEST);

        List<LevelData> levelDatas = DataPersister.ActiveDataSave.levelDatas;
        bool atLeastOneLevelExists = levelDatas.Count > 0 && !levelDatas[0].IsEmpty;
        ToggleMenuInteractable(atLeastOneLevelExists, PLEMenuType.LEVELSELECT);

        waveEditorMenu.UpdateWaveText();
        pleMenus.ForEach(menu => {
            if (menu!=this) {
                menu.SetMenuButtonInteractabilityByState();
            }
        });
    }
    /// <summary>
    /// Only for Specific Menu
    /// </summary>    
    public void SetMenuButtonInteractabilityByState(PLEMenuType menuToSet) {
        GetMenu(menuToSet).SetMenuButtonInteractabilityByState();
    } 

    public void OpenFloorSystemAction() {
        SelectMenuItem(PLEMenuType.FLOOR);
    }

    public void OpenPropsAction()
    {
        SelectMenuItem(PLEMenuType.PROPS);
    }

    public void OpenSpawnsAction()
    {
        SelectMenuItem(PLEMenuType.SPAWN);
    }    

    public void OpenWaveAction()
    {
        SelectMenuItem(PLEMenuType.WAVE);
    }

    public void OpenSaveAction()
    {
        SelectMenuItem(PLEMenuType.SAVE);        

        SaveMenu saveMenu = GetMenu(PLEMenuType.SAVE) as SaveMenu;
        ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);
        confirmMenu.SetYesButtonText("SAVE");
        confirmMenu.SetNoButtonText("SAVE AS");

        //set interactibility of buttons
        bool isSaveButtonInteractable = saveMenu.IsOverwriteButtonInteractable();
        confirmMenu.SetYesButtonInteractibility(isSaveButtonInteractable);
        
        Action saveAction = () => {
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });
            saveMenu.Save();
        };
        
        Action saveAsAction = () => {
            saveMenu.CanvasGroup.alpha = 1;
            saveMenu.CanvasGroup.interactable = true;
            saveMenu.CanvasGroup.blocksRaycasts = true;
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });
        };

        Action cancelAction = () => {
            BackAction();
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });
        };

        confirmMenu.DefineActions("Save?", saveAction, saveAsAction, cancelAction);
        MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });
    }

    public void OpenLevelSelectAction() {
        SelectMenuItem(PLEMenuType.LEVELSELECT);
    }

    public void TestLevelAction()
    {
        SelectMenuItem(PLEMenuType.TEST);
    }

    public void OpenHelpAction() {
        SelectMenuItem(PLEMenuType.HELP);
    }

    public void QuitAction() {
        SelectMenuItem(PLEMenuType.EXIT);
        ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);

        Action onYesAction = () => {
            Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
            LoadMenu loadMenu = (LoadMenu)menu;
            loadMenu.SetNavigation(Strings.Scenes.ScenePaths.MainMenu);
            MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        };

        Action onNoAction = () => {
            MenuManager.Instance.DoTransition(Strings.MenuStrings.CONFIRM, Transition.HIDE, new Effect[] { Effect.INSTANT });
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });
            OpenFloorSystemAction();
        };

        confirmMenu.DefineActions("You will lose any unsaved data. Are you sure?", onYesAction, onNoAction);

        MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });
    }
    
    public PLEMenu GetMenu(PLEMenuType i_menu) {
        switch (i_menu) {
            case PLEMenuType.MAIN: return this;
            case PLEMenuType.PROPS: return propsEditorMenu;
            case PLEMenuType.SPAWN: return spawnsEditorMenu;
            case PLEMenuType.FLOOR: return floorEditorMenu;
            case PLEMenuType.WAVE: return waveEditorMenu;
            case PLEMenuType.HELP: return helpEditorMenu;
            case PLEMenuType.SAVE: return saveEditorMenu;
            case PLEMenuType.TEST: return testEditorMenu;
            case PLEMenuType.LEVELSELECT: return levelSelectEditorMenu;
            case PLEMenuType.EXIT: return exitMenu;
            default: return null;
        }
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
    private void InitializeMenuToggles() {
        menuToggles.Clear();
        menuToggles.Add(new MenuSelectable(PLEMenuType.FLOOR, floorItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.PROPS, propsItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.SPAWN, spawnsItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.WAVE, waveItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.TEST, testItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.SAVE, saveItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.LEVELSELECT, loadItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.HELP, helpItem));
        menuToggles.Add(new MenuSelectable(PLEMenuType.EXIT, exitItem));
    }

    private void SetupToggleInteractability() {
        menuToggles.ForEach(menuToggle => { menuToggle.selectable.interactable = false; });
        floorItem.interactable = true;
        loadItem.interactable = true;
        helpItem.interactable = true;
        exitItem.interactable = true;
    }

    private void SwitchToMenu(PLEMenuType i_newMenu) {
        if (i_newMenu != currentDisplayedMenu) {
            HideAllMenusExceptMain();
            ConfirmMenu confirmMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM) as ConfirmMenu;
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });

            Menu newMenu = GetMenu(i_newMenu);
            MenuManager.Instance.DoTransition(this, Transition.SHOW, new Effect[] { Effect.INSTANT });
            MenuManager.Instance.DoTransition(newMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });

            currentDisplayedMenu = i_newMenu;
            bool allowCameraMovement = !IsMenu(currentDisplayedMenu, PLEMenuType.SAVE, PLEMenuType.LEVELSELECT, PLEMenuType.TEST);
            levelEditor.ToggleCameraController(allowCameraMovement);

            bool hideSideBar = IsMenu(currentDisplayedMenu, PLEMenuType.HELP, PLEMenuType.LEVELSELECT, PLEMenuType.TEST);
            if (hideSideBar) {
                helpSideBarToggler.HidePane();
            }
            helpSideBarToggler.LockPane(hideSideBar);
        }
        SetMenuButtonInteractabilityByState();
    }

    private void ToggleMenuInteractable(bool isInteractable, params PLEMenuType[] menus) {
        foreach (PLEMenuType menu in menus) {
            Selectable selectable = menuToggles.Find(item => item.menu == menu).selectable;
            selectable.interactable = isInteractable;
        }
    }

    private bool IsMenu(PLEMenuType menuInQuestion, params PLEMenuType[] potentialMatches) {
        foreach (PLEMenuType menu in potentialMatches) {
            if (menuInQuestion == menu) {
                return true;
            }
        }
        return false;
    }
    #endregion

}

[Serializable]
class MenuSelectable {
    public MenuSelectable(PLEMenuType menu, Selectable selectable) {
        this.menu = menu;
        this.selectable = selectable;
    }
    public PLEMenuType menu;
    public Selectable selectable;
}
