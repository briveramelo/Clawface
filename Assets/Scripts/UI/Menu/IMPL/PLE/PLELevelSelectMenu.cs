using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLELevelSelectMenu : Menu {
    public PLELevelSelectMenu() : base (Strings.MenuStrings.LEVEL_SELECT)
	{
    }
    public override Button InitialSelection {
        get {
            throw new System.NotImplementedException();
        }
    }

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
