//Garin + Brandon
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PropsMenu : PlacementMenu
{
    #region Public Interface
    public PropsMenu() : base(Strings.MenuStrings.ADD_PROPS_PLE) { }
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentProp !=null; } }    
   
    protected override void SelectUIItem() {
        PLEProp currentProp = ScrollGroupHelper.currentProp;

        if(currentProp) {
            selectedItem = currentProp.registeredProp;
            TryDestroyPreview();
            previewItem = Instantiate(selectedItem);
            previewItem.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    protected override void PostPlaceItem(GameObject newItem) {
        Rigidbody rigbod = newItem.GetComponent<Rigidbody>();
        rigbod.isKinematic = true;
    }

    protected override void HideStarted() {
        base.HideStarted();
        List<Rigidbody> rigBods = createdItemsParent.GetComponentsInChildren<Rigidbody>().ToList();
        rigBods.ForEach(rigbod=> rigbod.isKinematic = false);
    }
    protected override void ShowStarted() {
        base.ShowStarted();
        List<Rigidbody> rigBods = createdItemsParent.GetComponentsInChildren<Rigidbody>().ToList();
        rigBods.ForEach(rigbod => rigbod.isKinematic = true);
    }
    protected override void ShowComplete() {
        base.ShowComplete();
        //draw the grid
        editorInstance.gridController.currentEditorMenu = EditorMenu.PROPS_MENU;
    }
    #endregion

}
