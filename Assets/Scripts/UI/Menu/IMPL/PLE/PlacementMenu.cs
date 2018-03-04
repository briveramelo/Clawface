//Brandon

using System.Collections.Generic;
using UnityEngine;
using PlayerLevelEditor;
using ModMan;
using UnityEngine.UI;
using System.Linq;

public abstract class PlacementMenu : PlayerLevelEditorMenu {

    public PlacementMenu(string menuName) : base(menuName) { }

    [SerializeField] protected Transform createdItemsParent;
    [SerializeField] protected ScrollGroup scrollGroup;

    protected GameObject selectedItem = null;
    protected GameObject previewItem = null;
    protected List<string> itemNames = new List<string>();

    #region Boolean Helpers
    protected virtual bool SelectUI { get { return Input.GetMouseButtonDown(MouseButtons.LEFT); } }
    protected virtual bool SelectItem { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && MouseHelper.HitItem; } }
    protected virtual bool DeSelectItem { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT); } }
    protected bool RightClick { get { return Input.GetMouseButtonDown(MouseButtons.RIGHT); } }
    protected bool Place { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedItem != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied() && MouseHelper.currentBlockUnit.IsFlatAtWave(WaveSystem.currentWorkingWave); } }
    protected bool UpdatePreview { get { return previewItem != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied() && MouseHelper.currentBlockUnit.IsFlatAtWave(WaveSystem.currentWorkingWave); } }
    protected bool CanDeletedHoveredItem { get { return MouseHelper.currentHoveredObject && itemNames.Contains(MouseHelper.currentHoveredObject.name); } }
    #endregion

    #region Unity Lifecycle
    protected override void Update() {
        base.Update();
        if (allowInput) {
            if (SelectUI) {
                SelectUIItem();
            }
            else if (Place) {
                PlaceItem();
            }
            else if (SelectItem) {
                SelectGameItem();
            }
            else if (DeSelectItem) {
                bool deletedPreviewItem = DeselectUIItem();
                DeselectItem();
                if (!deletedPreviewItem && CanDeletedHoveredItem) {
                    DeleteHoveredItem();
                }
            }
            else if (UpdatePreview) {
                UpdatePreviewPosition();
            }
        }
    }
    #endregion

    #region Protected Interface
    protected virtual void SelectUIItem() {
        PLEUIItem currentUIItem = ScrollGroupHelper.currentUIItem;

        if (currentUIItem) {
            OnSelectUIItem(currentUIItem);
        }
    }
    protected virtual void OnSelectUIItem(PLEUIItem item) {
        selectedItem = item.registeredItem;
        TryDestroyPreview();
        previewItem = Instantiate(selectedItem);
    }
    protected abstract void DeselectItem();

    protected virtual void SelectGameItem() {
        DeselectAll();
    }
    protected virtual void DeselectAll() {
        List<PLEItem> items = createdItemsParent.GetComponentsInChildren<PLEItem>().ToList();
        items.ForEach(item => { item.Deselect(); });
    }

    protected virtual void PlaceItem() {
        GameObject newItem = Instantiate(selectedItem, createdItemsParent);
        newItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        newItem.name = selectedItem.name.TryCleanClone();
        itemNames.Add(newItem.name);
        PostPlaceItem(newItem);
    }
    protected virtual void PostPlaceItem(GameObject newItem) { }
    protected bool DeselectUIItem() {
        selectedItem = null;
        return TryDestroyPreview();
    }

    protected virtual void UpdatePreviewPosition() {
        previewItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
    }

    protected virtual void DeleteHoveredItem() {
        MouseHelper.currentBlockUnit.SetOccupation(false);
        itemNames.Remove(MouseHelper.currentHoveredObject.name);
        Helpers.DestroyProper(MouseHelper.currentHoveredObject);
    }

    public void ResetMenu(List<string> loadedItemNames) {
        itemNames.Clear();
        loadedItemNames.ForEach(item => {
            itemNames.Add(item);
        });
    }

    protected bool TryDestroyPreview() {
        if (previewItem) {
            Helpers.DestroyProper(previewItem);
            return true;
        }
        return false;
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        OnSelectUIItem(scrollGroup.GetFirstUIItem());
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
        DeselectAll();
        TryDestroyPreview();
    }

    #endregion

    #region Private Interface

    #endregion
}