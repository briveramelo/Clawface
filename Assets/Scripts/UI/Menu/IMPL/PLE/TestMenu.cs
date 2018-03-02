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
    [SerializeField] private GameObject editorCamera;    
    #endregion

    #region Private Fields

    GameObject playerSpawnerInstance = null;

    #endregion

    #region Unity Lifecycle
    

    #endregion

    #region Public Interface

    public TestMenu() : base(Strings.MenuStrings.TEST_PLE_MENU) { }


    #endregion


    #region Protected Interface
    protected override void ShowStarted() {
        base.ShowStarted();
    }
    protected override void ShowComplete()
    {
        base.ShowComplete();
        InitTestMode();
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        ReleaseTestMode();
    }

    #endregion

    #region Private Interface

    public override void BackAction() {
        Menu mainPLEMenu = levelEditor.GetMenu(PLEMenu.MAIN);
        MenuManager.Instance.DoTransition(mainPLEMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        base.BackAction();
    }


    private void InitTestMode()
    {
        MenuManager.Instance.DoTransition(levelEditor.GetMenu(PLEMenu.MAIN), Transition.HIDE, new Effect[] {});

        if(tileParents.childCount == 0 || SpawnMenu.playerSpawnInstance == null)
        {
            return;
        }

        editorCamera.SetActive(false);
        SpawnMenu.playerSpawnInstance.SetActive(false);

        playerSpawnerInstance = Instantiate(playerSpawnerPrefab);
        playerSpawnerInstance.transform.position = SpawnMenu.playerSpawnInstance.transform.position;
        levelEditor.waveSystem.ResetToWave0();

        //WeaponSelectMenu weaponSelectMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT) as WeaponSelectMenu;
        //weaponSelectMenu.backMenuTarget = null;
        //weaponSelectMenu.forwardMenuTarget = null;
        //MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });


        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, Strings.Scenes.Editor, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());
    }



    private void ReleaseTestMode() {
        //Debug.Log("ReleaseTestMode");

        GameObject playerInstance = null;

        if (playerSpawnerInstance != null)
        {
            playerInstance = playerSpawnerInstance.GetComponent<PlayerSpawner>().GetplayerPrefabGO();

            DestroyImmediate(playerSpawnerInstance);
            playerSpawnerInstance = null;
        }

        if(playerInstance != null)
        {
            DestroyImmediate(playerInstance);
            playerInstance = null;
        }

        editorCamera.SetActive(true);

        if (SpawnMenu.playerSpawnInstance != null)
        {
            SpawnMenu.playerSpawnInstance.SetActive(true);
        }

        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_END);
    }

    #endregion
}
