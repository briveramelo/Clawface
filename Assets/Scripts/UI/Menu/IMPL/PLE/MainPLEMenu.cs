using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using ModMan;
using PlayerLevelEditor;

public class MainPLEMenu : PlayerLevelEditorMenu {

    public MainPLEMenu() : base(Strings.MenuStrings.LevelEditor.MAIN_PLE_MENU) { }

    #region Public Fields
    public PLEMenu currentDisplayedMenu;
    #endregion

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
    #endregion

    #region Private Fields
    private List<PlayerLevelEditorMenu> pleMenus = new List<PlayerLevelEditorMenu>();
    private List<MenuSelectable> menuToggles = new List<MenuSelectable>();
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
        pleMenus = new List<PlayerLevelEditorMenu>() {
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

        currentDisplayedMenu = PLEMenu.FLOOR;
        OpenFloorSystemAction();
        levelEditor.gridController.SetGridVisiblity(true);
        levelEditor.ToggleCameraController(true);
        SetMenuButtonInteractabilityByState();
    }

    public void SelectMenuItem(PLEMenu menu) {
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
        ToggleMenuInteractable(anyTilesOn, PLEMenu.PROPS, PLEMenu.SPAWN, PLEMenu.WAVE);

        bool anyTilesOnAndPlayerOn = anyTilesOn && SpawnMenu.playerSpawnInstance != null;        

        levelEditor.TryCreateAllWaveParents();
        levelEditor.levelDataManager.SyncWorkingSpawnData();

        bool allWavesHaveEnemies = levelEditor.levelDataManager.WorkingLevelData.AllWavesHaveEnemies(PLESpawnManager.Instance.MaxWaveIndex);
        bool playerOnTilesOnAndAllWavesHaveEnemies = anyTilesOnAndPlayerOn && allWavesHaveEnemies;
        ToggleMenuInteractable(playerOnTilesOnAndAllWavesHaveEnemies, PLEMenu.SAVE, PLEMenu.TEST);

        List<LevelData> levelDatas = DataPersister.ActiveDataSave.levelDatas;
        bool atLeastOneLevelExists = levelDatas.Count > 0 && !levelDatas[0].IsEmpty;
        ToggleMenuInteractable(atLeastOneLevelExists, PLEMenu.LEVELSELECT);

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
    public void SetMenuButtonInteractabilityByState(PLEMenu menuToSet) {
        GetMenu(menuToSet).SetMenuButtonInteractabilityByState();
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

        SaveMenu saveMenu = GetMenu(PLEMenu.SAVE) as SaveMenu;
        ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);
        confirmMenu.SetYesButtonText("Save");
        confirmMenu.SetNoButtonText("Save As");

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

        confirmMenu.DefineActions("", saveAction, saveAsAction);
        MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });
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

    public void QuitAction() {
        SelectMenuItem(PLEMenu.EXIT);
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
    
    public PlayerLevelEditorMenu GetMenu(PLEMenu i_menu) {
        switch (i_menu) {
            case PLEMenu.MAIN: return this;
            case PLEMenu.PROPS: return propsEditorMenu;
            case PLEMenu.SPAWN: return spawnsEditorMenu;
            case PLEMenu.FLOOR: return floorEditorMenu;
            case PLEMenu.WAVE: return waveEditorMenu;
            case PLEMenu.HELP: return helpEditorMenu;
            case PLEMenu.SAVE: return saveEditorMenu;
            case PLEMenu.TEST: return testEditorMenu;
            case PLEMenu.LEVELSELECT: return levelSelectEditorMenu;
            case PLEMenu.EXIT: return exitMenu;
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
        menuToggles.Add(new MenuSelectable(PLEMenu.FLOOR, floorItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.PROPS, propsItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.SPAWN, spawnsItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.WAVE, waveItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.TEST, testItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.SAVE, saveItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.LEVELSELECT, loadItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.HELP, helpItem));
        menuToggles.Add(new MenuSelectable(PLEMenu.EXIT, exitItem));
    }

    private void SetupToggleInteractability() {
        menuToggles.ForEach(menuToggle => { menuToggle.selectable.interactable = false; });
        floorItem.interactable = true;
        loadItem.interactable = true;
        helpItem.interactable = true;
        exitItem.interactable = true;
    }

    private void SwitchToMenu(PLEMenu i_newMenu) {
        if (i_newMenu != currentDisplayedMenu) {
            HideAllMenusExceptMain();
            ConfirmMenu confirmMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM) as ConfirmMenu;
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });

            Menu newMenu = GetMenu(i_newMenu);
            MenuManager.Instance.DoTransition(this, Transition.SHOW, new Effect[] { Effect.INSTANT });
            MenuManager.Instance.DoTransition(newMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });

            currentDisplayedMenu = i_newMenu;
            bool allowCameraMovement = !IsMenu(currentDisplayedMenu, PLEMenu.SAVE, PLEMenu.LEVELSELECT, PLEMenu.TEST);
            levelEditor.ToggleCameraController(allowCameraMovement);
        }
        SetMenuButtonInteractabilityByState();
    }

    private void ToggleMenuInteractable(bool isInteractable, params PLEMenu[] menus) {
        foreach (PLEMenu menu in menus) {
            Selectable selectable = menuToggles.Find(item => item.menu == menu).selectable;
            selectable.interactable = isInteractable;
        }
    }

    private bool IsMenu(PLEMenu menuInQuestion, params PLEMenu[] potentialMatches) {
        foreach (PLEMenu menu in potentialMatches) {
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
    public MenuSelectable(PLEMenu menu, Selectable selectable) {
        this.menu = menu;
        this.selectable = selectable;
    }
    public PLEMenu menu;
    public Selectable selectable;
}
