using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLELevelSelectMenu : Menu {
    public PLELevelSelectMenu() : base (Strings.MenuStrings.LEVELSELECT_PLE_MENU) { }
    public override Button InitialSelection { get { return null; } }

    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void ShowStarted() {
        base.ShowStarted();
    }

    protected override void HideComplete() {
        base.HideComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
    }    

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        
    }

    protected override void DefaultShow(Transition transition, Effect[] effects) {
        
    }
}
