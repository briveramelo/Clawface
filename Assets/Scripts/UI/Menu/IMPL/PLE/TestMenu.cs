﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class TestMenu : PlayerLevelEditorMenu
{

    #region Public Fields    

    #endregion

    #region Serialized Unity Fields

    #endregion

    #region Private Fields    
    private PauseMenu pauseMenu;
    private StageOverMenu stageOverMenu;
    #endregion

    #region Unity Lifecycle
    protected override void Start() {
        base.Start();
        stageOverMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.STAGE_OVER) as StageOverMenu;
        pauseMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE) as PauseMenu;
    }
    protected override void Update() {
        if (allowInput && !pauseMenu.IsPaused) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN)) {
                BackAction();
            }
        }
    }

    #endregion

    #region Public Interface

    public TestMenu() : base(Strings.MenuStrings.LevelEditor.TEST_PLE_MENU) { }

    public override void BackAction() {
        if (stageOverMenu.IsDisplaying) {
            MenuManager.Instance.DoTransition(stageOverMenu, Transition.HIDE, new Effect[] { });
        }


        levelEditor.ExitLevel();

        GameObject player = GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER);
        if (player) {
            Destroy(player.transform.root.gameObject);
        }

        MenuManager.Instance.DoTransition(Strings.MenuStrings.WEAPON_SELECT, Transition.HIDE, new Effect[] { Effect.INSTANT });

        Menu mainPLEMenu = levelEditor.GetMenu(PLEMenu.MAIN);
        MenuManager.Instance.DoTransition(mainPLEMenu, Transition.SHOW, new Effect[] { });
        
        base.BackAction();
    }

    #endregion


    #region Protected Interface
    protected override void ShowStarted() {
        ShowWeaponSelectMenu();
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
    }

    #endregion

    #region Private Interface    


    private void ShowWeaponSelectMenu()
    {
        levelEditor.ToggleCameraController(false);        

        WeaponSelectMenu weaponSelectMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT) as WeaponSelectMenu;
        System.Action onReturnFromPLE = () => {
            BackAction();
            levelEditor.SetUpMenus();
        };
        System.Action onStartAction = () => {
            base.ShowStarted();
            base.ShowComplete();
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, SceneTracker.CurrentSceneName, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());
        };

        weaponSelectMenu.DefineNavigation(null, null, onStartAction, null, onReturnFromPLE);

        MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    #endregion
}
