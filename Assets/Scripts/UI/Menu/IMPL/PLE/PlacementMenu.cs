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
    [SerializeField] protected Selectable leftButton, rightButton;
    [SerializeField] protected Color highlightColor;

    protected PLEItem lastHoveredItem;
    protected PLEItem selectedPLEItem;
    protected GameObject selectedItemPrefab = null;
    protected GameObject previewItem = null;
    protected List<string> itemNames = new List<string>();

    #region Boolean Helpers
    protected virtual bool SelectUI { get { return Input.GetMouseButtonDown(MouseButtons.LEFT); } }
    protected virtual bool SelectItem { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && MouseHelper.HitItem; } }
    protected virtual bool DeSelectItem { get { return (Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT)) && !MouseHelper.HitUI; } }
    protected bool RightClick { get { return Input.GetMouseButtonDown(MouseButtons.RIGHT); } }
    protected virtual bool Place { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedItemPrefab != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied() && MouseHelper.currentBlockUnit.IsFlatAtWave(PLESpawnManager.Instance.CurrentWaveIndex); } }
    protected virtual bool UpdatePreview { get { return previewItem != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied() && MouseHelper.currentBlockUnit.IsFlatAtWave(PLESpawnManager.Instance.CurrentWaveIndex); } }
    protected virtual bool CanDeletedHoveredItem { get { return MouseHelper.currentHoveredObject && itemNames.Contains(MouseHelper.currentHoveredObject.name); } }
    #endregion

    #region Unity Lifecycle
    protected override void Update() {
        base.Update();
        if (allowInput) {
            if (Place) {
                PlaceItem();
            }
            else if (SelectItem) {
                SelectGameItem(MouseHelper.currentItem);
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

            if (allowInput) {
                CheckToHightlight();
            }
        }
    }
    #endregion

    #region Protected Interface
    protected virtual void CheckToHightlight() {
        if (previewItem==null) {
            PLEItem currentItem = MouseHelper.currentItem;
            if (lastHoveredItem != null && (lastHoveredItem != currentItem || currentItem == null)) {
                lastHoveredItem.TryUnHighlight();
            }
            if (currentItem != null) {
                currentItem.TryHighlight(highlightColor);
            }
            lastHoveredItem = currentItem;
        }
    }
    public virtual void PostSelectUIItem(PLEUIItem item) {
        if (allowInput) {
            if (item.isInteractable) {
                DeselectItem();
                selectedItemPrefab = item.registeredItem;
                TryDestroyPreview();
                previewItem = Instantiate(selectedItemPrefab);
                previewItem.name = previewItem.name.TryCleanName(Strings.CLONE);
                previewItem.name += Strings.PREVIEW;
                previewItem.GetComponent<PLEItem>().TryHighlight(highlightColor);
                PostSelectUIItemMenuSpecific(previewItem);
            }
            else {
                scrollGroup.DeselectAllUIItems();
            }
        }
    }
    protected virtual void PostSelectUIItemMenuSpecific(GameObject newItem) { }
    protected virtual void DeselectItem() {
        if (selectedPLEItem != null) {
            selectedPLEItem.Deselect();
            selectedPLEItem = null;
        }
        SetMenuButtonInteractabilityByState();
    }

    protected virtual void SelectGameItem(PLEItem selectedItem) {
        DeselectUIItem();
        DeselectAllGameItems();
        selectedItem.Select(highlightColor);
        selectedPLEItem = selectedItem;
    }
    protected virtual void DeselectAllGameItems() {
        List<PLEItem> items = createdItemsParent.GetComponentsInChildren<PLEItem>().ToList();
        items.ForEach(item => { item.Deselect(); });
        selectedPLEItem = null;
    }

    protected virtual void PlaceItem() {
        GameObject newItem = Instantiate(selectedItemPrefab, createdItemsParent);
        newItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        newItem.name = selectedItemPrefab.name.TryCleanName(Strings.CLONE);
        itemNames.Add(newItem.name);
        PostPlaceItem(newItem);
    }
    protected virtual void PostPlaceItem(GameObject newItem) { }

    protected virtual bool DeselectUIItem() {
        scrollGroup.DeselectAllUIItems();
        selectedItemPrefab = null;
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
        //ForceMenuButtonInteractability(false);
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
        DeselectAllGameItems();
        TryDestroyPreview();
    }
    #endregion

    #region Private Interface

    #endregion
}