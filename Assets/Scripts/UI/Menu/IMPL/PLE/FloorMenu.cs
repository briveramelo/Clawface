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

    public FloorMenu() : base(Strings.MenuStrings.LevelEditor.SET_DYNLEVEL_PLE) { }


    public void DropFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.Pit);
    }

    public void FlatFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.Floor);
    }

    public void RiseFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.Cover);
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
        List<GridTile> selectedGridTiles = levelEditor.gridController.GetSelectedGridTiles();

        if (selectedGridTiles.Count == 0)
            return;

        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        for (int i=0; i<selectedGridTiles.Count; i++) {
            GridTile tile = selectedGridTiles[i];
            PLEBlockUnit blockUnit = tile.blockUnit;
            LevelUnit levelUnit = tile.levelUnit;
            if (!blockUnit.HasActiveSpawn) {
                List<LevelUnitStates> levelUnitStates = blockUnit.GetLevelStates();
                levelUnitStates[currentWaveIndex] = state;
                blockUnit.SyncTileHeightStates();
                levelUnit.TryTransitionToState(state, false);
            }
        }

        //string eventName = Strings.Events.PLE_TEST_WAVE_ + PLESpawnManager.Instance.CurrentWaveIndex;
        //bool shouldChangeColor = false;
        //EventSystem.Instance.TriggerEvent(eventName, shouldChangeColor);
        //EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
    }    
    #endregion
}
