using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class TestMenu : PlayerLevelEditorMenu
{

    #region Public Fields    

    #endregion

    #region Serialized Unity Fields
    [SerializeField] private Transform tileParents;
    #endregion

    #region Private Fields    
    private bool HasCreatedPlayer { get { return levelEditor.hasCreatedPlayer; } set { levelEditor.hasCreatedPlayer = value; } }
    #endregion

    #region Unity Lifecycle
    

    #endregion

    #region Public Interface

    public TestMenu() : base(Strings.MenuStrings.LevelEditor.TEST_PLE_MENU) { }

    public override void BackAction() {
        if (HasCreatedPlayer) {
            ReleaseTestMode();
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
            levelEditor.ToggleCameraGameObject(false);
        };

        weaponSelectMenu.DefineNavigation(null, null, onStartAction, null, onReturnFromPLE);

        MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }


    private void ReleaseTestMode() {
        levelEditor.ExitLevel();
    }

    #endregion
}
