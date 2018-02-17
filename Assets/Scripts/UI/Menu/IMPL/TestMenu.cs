using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class TestMenu : Menu
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
    [SerializeField] private Transform tileParents;

    [SerializeField] private GameObject playerSpawnerPrefab;

    #endregion

    #region Private Fields

    private bool inputGuard = false;
    GameObject playerSpawnerInstance = null;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if (inputGuard)
        {
            if(InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP))
            {
                BackAction();
            }



            //active update loop
        }
    }

    #endregion

    #region Public Interface

    public TestMenu() : base(Strings.MenuStrings.TEST_PLE_MENU)
    {

    }


    #endregion


    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;

        InitTestMode();
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;

        ReleaseTestMode();
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion

    #region Private Interface

    private void BackAction()
    {
        MenuManager.Instance.DoTransition(editorInstance.GetMenu(PLEMenu.MAIN), Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }



    private void InitTestMode()
    {
        if(tileParents.childCount == 0)
        {
            Debug.Log("No Tile");
            return;
        }

        foreach(Transform childGO in tileParents)
        {
            PLEBlockUnit PLEU = childGO.gameObject.GetComponent<PLEBlockUnit>();

            if (PLEU == null) continue;

            if(PLEU.IsOccupied() == false)
            {
                playerSpawnerInstance = Instantiate(playerSpawnerPrefab);
                playerSpawnerInstance.transform.position = PLEU.spawnTrans.position;

                return;
            }
        }

    }



    private void ReleaseTestMode()
    {
        Debug.Log("ReleaseTestMode");

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
    }

    #endregion
}
