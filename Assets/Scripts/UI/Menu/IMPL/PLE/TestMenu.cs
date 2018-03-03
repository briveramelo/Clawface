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
    [SerializeField] private GameObject playerSpawnerPrefab;
    #endregion

    #region Private Fields

    GameObject playerSpawnerInstance = null;
    private bool hasCreatedPlayer = false;
    #endregion

    #region Unity Lifecycle
    

    #endregion

    #region Public Interface

    public TestMenu() : base(Strings.MenuStrings.LevelEditor.TEST_PLE_MENU) { }

    public override void BackAction() {
        if (hasCreatedPlayer) {
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
            CreatePlayer();
            levelEditor.ToggleCameraGameObject(false);
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, Strings.Scenes.Editor, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());
        };

        weaponSelectMenu.DefineNavigation(null, null, onStartAction, null, onReturnFromPLE);

        MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    void CreatePlayer() {
        SpawnMenu.playerSpawnInstance.SetActive(false);

        playerSpawnerInstance = Instantiate(playerSpawnerPrefab);
        playerSpawnerInstance.transform.position = SpawnMenu.playerSpawnInstance.transform.position;
        levelEditor.waveSystem.ResetToWave0();
        hasCreatedPlayer = true;
    }

    private void ReleaseTestMode() {        

        GameObject playerInstance = null;
        if (playerSpawnerInstance != null) {
            playerInstance = playerSpawnerInstance.GetComponent<PlayerSpawner>().GetplayerPrefabGO();

            DestroyImmediate(playerSpawnerInstance);
            playerSpawnerInstance = null;
        }

        if(playerInstance != null) {
            DestroyImmediate(playerInstance);
            playerInstance = null;
        }        

        if (SpawnMenu.playerSpawnInstance != null) {
            SpawnMenu.playerSpawnInstance.SetActive(true);
        }

        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_END);
        hasCreatedPlayer = false;
    }

    #endregion
}
