﻿//Garin + Brandon
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PropsMenu : PlacementMenu
{
    public PropsMenu() : base(Strings.MenuStrings.LevelEditor.ADD_PROPS_PLE) { }
    #region Public Interface

    #endregion

    #region Private Fields
    private const int maxRotation = 270;
    private const int minRotation = 0;

    private float CurrentRotation {
        get { return currentRotation; }
        set {
            float newRotation = value;
            if (newRotation > maxRotation) {
                newRotation = minRotation;
            }
            else if (newRotation < minRotation) {
                newRotation = maxRotation;
            }
            currentRotation = newRotation;
        }
    }
    private float currentRotation;
    #endregion

    #region Serialzied Unity Fields
    [SerializeField] private Text rotationLabel;
    #endregion

    

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem !=null; } }
    protected override bool SelectItem { get { return base.SelectItem && MouseHelper.currentProp != null; } }
    protected override bool CanDeletedHoveredItem { get { return base.CanDeletedHoveredItem && MouseHelper.currentProp; } }

    protected override void DeselectAllGameItems() {
        base.DeselectAllGameItems();
    }
    protected override void PostPlaceItem(GameObject newItem) {
        MouseHelper.currentBlockUnit.SetOccupation(true);
        MouseHelper.currentBlockUnit.SetProp(newItem);
    }
    protected override void PostOnSelectUIItem(GameObject newItem) {
        ApplyRotation(newItem.transform, 0f);
    }

    protected override void HideStarted() {
        base.HideStarted();
    }
    protected override void ShowStarted() {
        base.ShowStarted();
        TrySelectUIItem(scrollGroup.GetLastUIItem());
        ApplyRotation(0);
    }
    protected override void ShowComplete() {
        base.ShowComplete();        
    }
    protected override void UpdatePreviewPosition()
    {
        base.UpdatePreviewPosition();
        previewItem.transform.localEulerAngles = new Vector3(0, CurrentRotation, 0);
    }
    protected override void SetInteractabilityByState() {
        bool isItemSelected = selectedPLEItem != null;
        selectables.ForEach(selectable => { selectable.interactable = isItemSelected; });
    }
    #endregion

    #region Public Interface
    public void ApplyRotationDelta(float i_del) {
        CurrentRotation += i_del;
        ApplyRotation(CurrentRotation);
    }

    private void ApplyRotation(Transform item, float rotation) {
        CurrentRotation = rotation;
        rotationLabel.text = CurrentRotation.ToString("0");
        if (item != null) {
            item.localEulerAngles = new Vector3(0f, CurrentRotation, 0f);
        }
    }
    private void ApplyRotation(float rotation) {
        CurrentRotation = rotation;
        rotationLabel.text = CurrentRotation.ToString("0");
        if (selectedPLEItem != null) {
            selectedPLEItem.transform.localEulerAngles = new Vector3(0f, CurrentRotation, 0f);
        }
    }

    protected override void SelectGameItem() {
        base.SelectGameItem();
        MouseHelper.currentProp.Select();
        selectedPLEItem = MouseHelper.currentProp;
        ApplyRotation(selectedPLEItem.transform.localEulerAngles.y);
        SetInteractabilityByState();
    }
    protected override void DeselectItem() {
        base.DeselectItem();
    }
    #endregion
}
