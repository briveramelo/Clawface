using PlayerLevelEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerLevelEditorMenu : Menu {

    public PlayerLevelEditorMenu(string name) : base(name) { }

    [SerializeField] protected LevelEditor levelEditor;
    [SerializeField] protected Selectable initiallySelected;

    public override Selectable InitialSelection { get { return initiallySelected; } }

    protected bool allowInput;

    protected virtual void Update() {
        if (allowInput) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP)) {
                BackAction();
            }
        }
    }

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    public virtual void BackAction() {
        (levelEditor.GetMenu(PLEMenu.MAIN) as MainPLEMenu).OpenFloorSystemAction();
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        allowInput = true;
    }

    protected override void HideStarted() {
        base.HideStarted();
        allowInput = false;
    }
}
