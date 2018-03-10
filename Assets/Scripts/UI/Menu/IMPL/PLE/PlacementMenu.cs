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

    protected PLEItem selectedPLEItem;
    protected List<Selectable> selectables = new List<Selectable>();
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

    protected virtual void Awake() {
        InitializeSelectables();
    }
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
        }
    }
    #endregion

    #region Protected Interface
    public virtual void TrySelectUIItem(PLEUIItem item) {
        if (allowInput) {
            if (item.isInteractable) {
                DeselectItem();
                selectedItemPrefab = item.registeredItem;
                TryDestroyPreview();
                previewItem = Instantiate(selectedItemPrefab);
                previewItem.name = previewItem.name.TryCleanName(Strings.CLONE);
                previewItem.name += Strings.PREVIEW;
                PostOnSelectUIItem(previewItem);
            }
            else {
                scrollGroup.DeselectAll();
            }
        }
    }
    protected virtual void PostOnSelectUIItem(GameObject newItem) { }
    protected virtual void DeselectItem() {
        if (selectedPLEItem != null) {
            selectedPLEItem.Deselect();
            selectedPLEItem = null;
        }
        SetInteractabilityByState();
    }

    protected virtual void SelectGameItem(PLEItem selectedItem) {
        DeselectUIItem();
        DeselectAllGameItems();
        selectedItem.Select();
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

    protected bool DeselectUIItem() {
        scrollGroup.DeselectAll();
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
        ForceInteractability(false);
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
        DeselectAllGameItems();
        TryDestroyPreview();
    }

    protected virtual void InitializeSelectables() {
        selectables.Add(leftButton);
        selectables.Add(rightButton);
    }

    protected virtual void ForceInteractability(bool isInteractable) {
        selectables.ForEach(selectable => { selectable.interactable = isInteractable; });
    }
    protected abstract void SetInteractabilityByState();

    #endregion

    #region Private Interface

    #endregion
}