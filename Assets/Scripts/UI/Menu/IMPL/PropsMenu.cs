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
    private PLEProp selectedProp;
    #endregion

    #region Serialzied Unity Fields
    [SerializeField] private Text rotationLabel;
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem !=null; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentProp != null; } }


    protected override void DeselectAll() {
        base.DeselectAll();
        selectedProp = null;
    }
    protected override void SelectUIItem() {
        PLEUIItem currentUIItem = ScrollGroupHelper.currentUIItem;

        if(currentUIItem) {
            selectedItem = currentUIItem.registeredItem;
            TryDestroyPreview();
            previewItem = Instantiate(selectedItem);
            previewItem.GetComponent<Rigidbody>().isKinematic = true;
        }
        
    }
    protected override void PostPlaceItem(GameObject newItem) {
        MouseHelper.currentBlockUnit.SetOccupation(true);
        MouseHelper.currentBlockUnit.SetProp(newItem);
        Rigidbody rigbod = newItem.GetComponent<Rigidbody>();
        rigbod.isKinematic = true;
        newItem.transform.localEulerAngles = new Vector3(0, currentRotation, 0);
    }
    protected override void HideStarted() {
        base.HideStarted();
        List<Rigidbody> rigBods = createdItemsParent.GetComponentsInChildren<Rigidbody>().ToList();
        rigBods.ForEach(rigbod=> rigbod.isKinematic = false);
        
        rotationLabel.text = 0.ToString("0");
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
        if (currentRotation > 270.0f || currentRotation < -270.0f) {
            currentRotation = 0.0f;
        }
        rotationLabel.text = currentRotation.ToString("0");
        if (selectedProp!=null) {
            selectedProp.transform.localEulerAngles = new Vector3(0f, currentRotation, 0f);
        }
    }

    protected override void SelectGameItem() {
        base.SelectGameItem();
        MouseHelper.currentProp.Select();
        selectedProp = MouseHelper.currentProp;
        rotationLabel.text = selectedProp.transform.localEulerAngles.y.ToString("0");
    }
    protected override void DeselectItem() {
        if (selectedProp!=null) {
            selectedProp.Deselect();
            selectedProp = null;
        }
    }
    #endregion
}
