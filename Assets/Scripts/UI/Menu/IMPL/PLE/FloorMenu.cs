using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;
using System.Linq;

public class FloorMenu : PlayerLevelEditorMenu {

    #region Public Fields

    #endregion

    #region Private Fields

    private bool inputGuard = false;
    private float raycastDistance = 1000.0f;
    private Vector3 sceneMousePos;
    private Vector3 newItemPos = Vector3.zero;
    #endregion

    #region Serialized Unity Fields

    #endregion


    #region Unity Lifecycle

    #endregion  

    #region Public Interface

    public FloorMenu() : base(Strings.MenuStrings.SET_DYNLEVEL_PLE) { }


    public void DropFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.pit);
    }

    public void FlatFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.floor);
    }

    public void RiseFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.cover);
    }

    public override void BackAction()
    {
        
    }


    #endregion

    #region Protected Interface
    protected override void ShowStarted() {
        base.ShowStarted();
        levelEditor.gridController.SetGridVisiblity(true);
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
        levelEditor.gridController.ClearSelectedBlocks();
        levelEditor.gridController.SetGridVisiblity(false);
        levelEditor.gridController.ShowWalls();
    }

    #endregion

    #region Private Interface    
    void UpdateSelectedAndOpenTilesState(LevelUnitStates state) {
        List<GameObject> selectedObjects = levelEditor.gridController.GetSelectedBlocks();

        if (selectedObjects.Count == 0)
            return;

        foreach (GameObject GO in selectedObjects) {
            if (GO != null) {
                PLEBlockUnit blockUnit = GO.GetComponent<PLEBlockUnit>();
                if (!blockUnit.HasActiveSpawn) {
                    List<LevelUnitStates> levelUnitStates = blockUnit.GetLevelStates();
                    levelUnitStates[WaveSystem.currentWave] = state;
                }
            }
            else {
                selectedObjects.Remove(GO);
            }
        }

        string event_name = Strings.Events.PLE_TEST_WAVE_ + WaveSystem.currentWave;
        bool shouldChangeColor = false;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        EventSystem.Instance.TriggerEvent(event_name, shouldChangeColor);
    }    
    #endregion
}
