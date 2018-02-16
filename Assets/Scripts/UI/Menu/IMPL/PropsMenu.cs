//Garin + Brandon
using UnityEngine.EventSystems;


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
