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
    [SerializeField] protected Image selectedPreviewImage;
    [SerializeField] protected Color highlightColor;

    protected PLEItem lastHoveredItem;
    protected PLEItem selectedPLEItem;
    protected GameObject selectedItemPrefab = null;
    protected GameObject previewItem = null;
    protected List<string> itemNames = new List<string>();
    public Transform SelectedItem { get { return selectedPLEItem!=null ? selectedPLEItem.transform : null; } }

    #region Boolean Helpers
    protected virtual bool SelectUI { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftAlt); } }
    protected virtual bool SelectItem { get { return (Input.GetMouseButtonDown(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftAlt)) && MouseHelper.currentItem != null && MouseHelper.currentBlockUnit != null; } }
    protected virtual bool DeSelectItem { get { return ((Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT)) && !Input.GetKey(KeyCode.LeftAlt)) && !MouseHelper.HitUI; } }
    protected virtual bool Place { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedItemPrefab != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied && MouseHelper.currentBlockUnit.IsFlatAtWave(PLESpawnManager.Instance.CurrentWaveIndex); } }
    protected virtual bool IsCurrentTileAvailable { get { return MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied && MouseHelper.currentBlockUnit.IsFlatAtWave(PLESpawnManager.Instance.CurrentWaveIndex); } }
    protected virtual bool UpdatePreview { get { return previewItem != null && IsCurrentTileAvailable; } }
    protected virtual bool UpdateGameItem { get { return selectedPLEItem != null && (Input.GetMouseButton(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftAlt) ) && IsCurrentTileAvailable; } }
    protected virtual bool ReplaceGameItem { get { return selectedPLEItem != null && (Input.GetMouseButtonUp(MouseButtons.LEFT) && !Input.GetKey(KeyCode.LeftAlt)) && IsCurrentTileAvailable; } }
    protected virtual bool DeleteInputDown { get { return (Input.GetMouseButtonDown(MouseButtons.RIGHT) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)); } }
    protected virtual bool CanDeleteHoveredItem { get { return DeleteInputDown && MouseHelper.currentHoveredObject!=null && itemNames.Contains(MouseHelper.currentHoveredObject.name); } }
    protected virtual bool CanDeleteSelectedItem { get { return DeleteInputDown && selectedPLEItem != null; } }
    #endregion

    #region Unity Lifecycle
    protected override void Update() {
        base.Update();
        if (allowInput) {
            if (Place) {
                PlaceItem();
            }
            else if (SelectItem) {
                SelectGameItem(MouseHelper.currentItem, true);
            }
            else if (DeSelectItem) {
                bool deletedPreviewItem = DeselectUIItem();
                DeselectItem();
                if (!deletedPreviewItem && CanDeleteHoveredItem) {
                    DeleteHoveredItem();
                }
            }
            else if (CanDeleteSelectedItem) {
                DeleteItem(selectedPLEItem.gameObject);
                DeselectUIItem();
                DeselectItem();
            }
            else if (UpdatePreview) {
                UpdatePreviewPosition();
            }
            else if (UpdateGameItem) {
                UpdateSelectedPLEItemPosition();
            }
            else if (ReplaceGameItem) {
                RePlaceGameItem();
            }

            CheckToHightlight();
        }
    }
    #endregion

    #region Protected Interface
    protected virtual void RePlaceGameItem() {
        MouseHelper.currentBlockUnit.AddSpawn(selectedPLEItem.gameObject);
        selectedPLEItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        selectedPLEItem.tile = levelEditor.gridController.GetTileAtPoint(MouseHelper.currentBlockUnit.transform.position);
        SelectGameItem(selectedPLEItem);
    }
    protected virtual void CheckToHightlight() {
        if (previewItem==null && !(selectedPLEItem != null && Input.GetMouseButton(MouseButtons.LEFT))) {
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

    protected virtual void SelectGameItem(PLEItem selectedItem, bool isPickingUp=false) {
        DeselectUIItem();
        DeselectAllGameItems();
        selectedItem.Select(highlightColor);
        selectedPLEItem = selectedItem;
        if (isPickingUp) {
            selectedItem.tile.blockUnit.RemoveSpawn(selectedItem.gameObject);
        }
    }
    protected virtual void DeselectAllGameItems() {
        List<PLEItem> items = createdItemsParent.GetComponentsInChildren<PLEItem>().ToList();
        items.ForEach(item => { item.Deselect(); });
        selectedPLEItem = null;
    }

    protected virtual void PlaceItem() {
        GameObject newItem = Instantiate(selectedItemPrefab, createdItemsParent);        
        newItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        newItem.GetComponent<PLEItem>().tile = levelEditor.gridController.GetTileAtPoint(MouseHelper.currentBlockUnit.transform.position);
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
    protected virtual void UpdateSelectedPLEItemPosition() {
        selectedPLEItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
    }

    protected virtual void DeleteHoveredItem() {
        DeleteItem(MouseHelper.currentHoveredObject);
    }
    protected virtual void DeleteItem(GameObject item) {
        itemNames.Remove(item.name);
        Helpers.DestroyProper(item);
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
        allowInput = true;
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
        DeselectAllGameItems();
        TryDestroyPreview();
    }

    public virtual void TrySelectFirstAvailable() {
        PLEUIItem firstAvailable = scrollGroup.TryGetFirstAvailableUIItem();
        if (firstAvailable) {
            selectedPreviewImage.sprite = firstAvailable.imagePreview.sprite;
            scrollGroup.SelectUIItem(firstAvailable.ItemIndex);
        }
    }
    #endregion

    #region Private Interface

    #endregion
}