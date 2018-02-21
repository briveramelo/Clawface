//Brandon

using System.Collections.Generic;
using UnityEngine;
using PlayerLevelEditor;
using ModMan;
using UnityEngine.UI;

public abstract class PlacementMenu : Menu {

    public PlacementMenu(string menuName) : base(menuName) { }

    public override Button InitialSelection { get { return initiallySelected; } }

    [SerializeField] protected LevelEditor editorInstance;
    [SerializeField] protected Button initiallySelected;
    [SerializeField] protected Transform createdItemsParent;

    protected bool inputGuard = false;
    protected GameObject selectedItem = null;
    protected GameObject previewItem = null;
    protected List<string> itemNames = new List<string>();

    #region Boolean Helpers
    protected virtual bool SelectUI { get { return Input.GetMouseButtonDown(MouseButtons.LEFT); } }
    protected bool RightClick { get { return Input.GetMouseButtonDown(MouseButtons.RIGHT); } }
    protected bool Place { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedItem != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied(); } }
    protected bool UpdatePreview { get { return previewItem != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.IsOccupied(); } }
    protected bool CanDeletedHoveredItem { get { return MouseHelper.currentHoveredObject && itemNames.Contains(MouseHelper.currentHoveredObject.name); } }
    #endregion

    #region Unity Lifecycle
    protected virtual void Update() {
        if (inputGuard) {
            if (SelectUI) {
                SelectUIItem();
            }
            else if (Place) {
                PlaceItem();
            }
            else if (RightClick) {
                bool deletedPreviewItem = DeselectUIItem();
                if (!deletedPreviewItem && CanDeletedHoveredItem) {
                    DeleteHoveredItem();
                }
            }
            else if (UpdatePreview) {
                UpdatePreviewPosition();
            }

            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP)) {
                BackAction();
            }
        }
    }
    #endregion

    #region Protected Interface
    protected abstract void SelectUIItem();

    protected virtual void PlaceItem() {
        GameObject newItem = Instantiate(selectedItem, createdItemsParent);
        newItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        newItem.name = selectedItem.name.TryCleanClone();
        itemNames.Add(newItem.name);
        MouseHelper.currentBlockUnit.SetOccupation(true);
        PostPlaceItem(newItem);
    }
    protected virtual void PostPlaceItem(GameObject newItem) { }
    protected bool DeselectUIItem() {
        selectedItem = null;
        return TryDestroyPreview();
    }

    protected void UpdatePreviewPosition() {
        previewItem.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
    }

    private void DeleteHoveredItem() {
        MouseHelper.currentBlockUnit.SetOccupation(false);
        itemNames.Remove(MouseHelper.currentHoveredObject.name);
        Helpers.DestroyProper(MouseHelper.currentHoveredObject);
    }

    protected bool TryDestroyPreview() {
        if (previewItem) {
            Helpers.DestroyProper(previewItem);
            return true;
        }
        return false;
    }


    protected override void ShowComplete() {
        base.ShowComplete();
        inputGuard = true;
    }

    protected override void HideStarted() {
        base.HideStarted();
        inputGuard = false;
        TryDestroyPreview();
    }

    protected override void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    #endregion

    #region Private Interface

    protected void BackAction() {
        MainPLEMenu menu = editorInstance.GetMenu(PLEMenu.MAIN) as MainPLEMenu;

        MenuManager.Instance.DoTransition(menu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    #endregion
}