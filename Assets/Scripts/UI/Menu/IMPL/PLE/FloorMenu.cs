using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;

public class FloorMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return initiallySelected;
        }
    }

    #endregion

    #region Private Fields

    private bool inputGuard = false;
    private float raycastDistance = 1000.0f;
    private Vector3 sceneMousePos;
    private Vector3 newItemPos = Vector3.zero;
    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private LevelEditor editorInstance;


    #endregion


    #region Unity Lifecycle


    private void Update()
    {
        if(inputGuard)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP))
            {
                BackAction();
            }
        }
    }



    #endregion  

    #region Public Interface

    public FloorMenu() : base(Strings.MenuStrings.SET_DYNLEVEL_PLE)
    { }


    public void DropFloorAction() {
        UpdateSelectedTiles(LevelUnitStates.pit);
    }

    public void FlatFloorAction() {
        UpdateSelectedTiles(LevelUnitStates.floor);
    }

    public void RiseFloorAction() {
        UpdateSelectedTiles(LevelUnitStates.cover);
    }

    void UpdateSelectedTiles(LevelUnitStates state) {
        List<GameObject> selectedObjects = editorInstance.gridController.GetSelectedBlocks();

        if (selectedObjects.Count == 0)
            return;

        foreach (GameObject GO in selectedObjects) {
            if (GO != null) {
                List<LevelUnitStates> levelUnitStates = GO.GetComponent<PLEBlockUnit>().GetLevelStates();
                levelUnitStates[WaveSystem.currentWave] = state;
            }
            else {
                selectedObjects.Remove(GO);
            }
        }
        
        string event_name = Strings.Events.PLE_TEST_WAVE_ + WaveSystem.currentWave;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        EventSystem.Instance.TriggerEvent(event_name);

        //            print("RiseFloorAction PLE_UPDATE_LEVELSTATE" + Strings.Events.PLE_UPDATE_LEVELSTATE);
        //            print("RiseFloorAction event_name" + event_name);
    }

    public void BackAction()
    {
        MenuManager.Instance.DoTransition(editorInstance.GetMenu(PLEMenu.MAIN), Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }


    #endregion

    #region Protected Interface
    protected override void ShowStarted() {
        base.ShowStarted();
        editorInstance.gridController.SetGridVisiblity(true);
    }
    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;        
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
        editorInstance.gridController.ClearSelectedBlocks();
        editorInstance.gridController.SetGridVisiblity(false);
        editorInstance.gridController.ShowWalls();
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion

    #region Private Interface

    private void UpdateObjectPreview()
    {
        Ray r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (Physics.Raycast(r, out h, raycastDistance))
        {
            sceneMousePos = h.point;

            if (Input.GetMouseButtonDown(MouseButtons.LEFT))
            {
                Vector3 objectPos = PlayerLevelEditor.ToolLib.ConvertToGrid(sceneMousePos);

                //Consider when placing on top of spawnpoints
                //IsLegalPlacement();

                if (objectPos != null)
                {
                    newItemPos = objectPos;
                }

            }

        }

        //draw preview block at location
        //ToolLib.draft(previewGridBlock, ToolLib.ConvertToGrid(sceneMousePos - previewGridBlock.transform.position), Color.green);

    }

    #endregion
}
