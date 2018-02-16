//Garin

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using PlayerLevelEditor;
using ModMan;

public class PropsMenu : PlacementMenu
{
    #region Public Interface
    public PropsMenu() : base(Strings.MenuStrings.ADD_PROPS_PLE) { }
    #endregion

    #region Private Fields
    private PointerEventData pointerData;    
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentProp !=null; } }    
   
    protected override void SelectUIItem() {
        PLEProp currentProp = ScrollGroupHelper.currentProp;

        if(currentProp) {
            selectedItem = currentProp.registeredProp;
            TryDestroyPreview();
            previewItem = Instantiate(selectedItem);
        }
    }

    protected override void ShowComplete() {
        base.ShowComplete();
        //draw the grid
        editorInstance.gridController.currentEditorMenu = EditorMenu.PROPS_MENU;
    }
    #endregion

}
