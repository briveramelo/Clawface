//Garin + Brandon
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PropsMenu : PlacementMenu
{
    #region Public Interface
    public PropsMenu() : base(Strings.MenuStrings.ADD_PROPS_PLE) { }
    #endregion

    #region Private Fields
    private float currentRotation = 0.0f;
    #endregion

    #region Serialzied Unity Fields
    [SerializeField] private Text rotationLabel;
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
        newItem.transform.localEulerAngles = new Vector3(0, currentRotation, 0);
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
    protected override void UpdatePreviewPosition()
    {
        base.UpdatePreviewPosition();
        previewItem.transform.localEulerAngles = new Vector3(0, currentRotation, 0);
    }
    #endregion

    #region Public Interface
    public void ApplyRotationDelta(float i_del) {
        currentRotation += i_del;
        if (currentRotation > 270.0f || currentRotation < -270.0f)
            currentRotation = 0.0f;
        rotationLabel.text = currentRotation.ToString();
    }
    #endregion  
}
