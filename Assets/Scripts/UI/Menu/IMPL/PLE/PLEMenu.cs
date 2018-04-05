using PLE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PLEMenu : Menu {

    public PLEMenu(string name) : base(name) { }

    [SerializeField] protected Selectable initiallySelected;
    [SerializeField] protected LevelEditor levelEditor;
    [SerializeField] protected MainPLEMenu mainPLEMenu;
    [SerializeField] protected List<Selectable> allSelectables;

    public override Selectable InitialSelection { get { return initiallySelected; } }

    protected virtual void Update() {
        if (allowInput) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN)) {
                BackAction();
            }
        }
    }

    public virtual void BackAction() {
        mainPLEMenu.OpenFloorSystemAction();
    }

    public abstract void SetMenuButtonInteractabilityByState();
    public virtual void ForceMenuButtonInteractability(bool isInteractable) {
        allSelectables.ForEach(selectable => { selectable.interactable = isInteractable; });
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        SetMenuButtonInteractabilityByState();        
    }

    protected override void HideStarted() {
        base.HideStarted();
    }
}
