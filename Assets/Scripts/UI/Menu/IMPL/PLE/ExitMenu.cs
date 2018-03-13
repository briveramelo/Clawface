using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenu : PlayerLevelEditorMenu {

    public ExitMenu() : base(Strings.MenuStrings.LevelEditor.EXIT_PLE_MENU) { }

    public override void SetMenuButtonInteractabilityByState() {
        
    }
    public override void BackAction() {
        base.BackAction();
    }
}
