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

    private Vector3 sceneMousePos;
    private Vector3 newItemPos = Vector3.zero;
    #endregion

    #region Serialized Unity Fields
    [SerializeField] private Button flattenAllButton;

    #endregion


    #region Unity Lifecycle

    #endregion  

    #region Public Interface

    public FloorMenu() : base(Strings.MenuStrings.LevelEditor.FLOOR_PLE_MENU) { }


    public void DropFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.Pit);
    }

    public void FlatFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.Floor);
    }

    public void RiseFloorAction() {
        UpdateSelectedAndOpenTilesState(LevelUnitStates.Cover);
    }
    public void ColorAction(int tileType) {
        List<GridTile> tiles = levelEditor.gridController.GetSelectedGridTiles();
        if (tiles.Count == 0)
            return;

        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        Color newColor = TileColors.GetColor(tileType);
        for (int i = 0; i < tiles.Count; i++) {
            GridTile tile = tiles[i];
            PLEBlockUnit blockUnit = tile.blockUnit;
            blockUnit.riseColor = newColor;
            blockUnit.SyncTileStatesAndColors();
        }
    }

    public void FlattenAllTiles() {
        List<GridTile> allTiles = levelEditor.gridController.GetAllActiveGridTiles();
        UpdateTiles(allTiles, LevelUnitStates.Floor, true);
    }

    public override void SetMenuButtonInteractabilityByState() {
        bool anyTilesSelected = levelEditor.gridController.AnyTilesSelected();
        allSelectables.ForEach(selectable => { selectable.interactable = anyTilesSelected; });

        bool anyActiveTilesNotFlat = levelEditor.gridController.AnyActiveTilesNotFlat();
        flattenAllButton.interactable = anyActiveTilesNotFlat;
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
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        levelEditor.gridController.ClearSelectedBlocks();
        levelEditor.gridController.SetGridVisiblity(false);
        levelEditor.gridController.ShowWalls();
    }

    #endregion

    #region Private Interface    
    private void UpdateSelectedAndOpenTilesState(LevelUnitStates state) {
        List<GridTile> selectedGridTiles = levelEditor.gridController.GetSelectedGridTiles();
        UpdateTiles(selectedGridTiles, state, false);
    }

    private void UpdateTiles(List<GridTile> tiles, LevelUnitStates state, bool wasToldToChangeColor) {
        if (tiles.Count == 0)
            return;

        SFXManager.Instance.Play(SFXType.UI_Click);
        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        for (int i = 0; i < tiles.Count; i++) {
            GridTile tile = tiles[i];
            PLEBlockUnit blockUnit = tile.blockUnit;
            LevelUnit levelUnit = tile.levelUnit;
            if (!blockUnit.HasActiveSpawn) {
                List<LevelUnitStates> levelUnitStates = blockUnit.GetLevelStates();
                levelUnitStates[currentWaveIndex] = state;
                blockUnit.SyncTileStatesAndColors();
                levelUnit.TryTransitionToState(state, wasToldToChangeColor);
            }
        }
    }
    #endregion
}
