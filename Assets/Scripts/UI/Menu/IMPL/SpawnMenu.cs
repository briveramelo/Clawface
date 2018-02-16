using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

using ModMan;
using PlayerLevelEditor;


public class SpawnMenu : PlacementMenu {

    #region Public Interface
    public SpawnMenu() : base(Strings.MenuStrings.ADD_SPAWNS_PLE) { }
    #endregion

    #region Private Fields
    private PointerEventData pointerData;
    #endregion

    
    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentSpawn != null; } }

    protected override void ShowComplete() {
        base.ShowComplete();
        editorInstance.gridController.currentEditorMenu = EditorMenu.SPAWN_MENU;
    }

    protected override void SelectUIItem() {
        PLESpawn currentSpawn = ScrollGroupHelper.currentSpawn;

        if(currentSpawn) {
            selectedItem = currentSpawn.registeredSpawner;
            TryDestroyPreview();
            previewItem = Instantiate(selectedItem);
        }
    }
    #endregion

}
