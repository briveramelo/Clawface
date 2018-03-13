using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerLevelEditor;

public class HelpMenu : PlayerLevelEditorMenu {

    #region Public Fields

    #endregion

    #region Serialized Unity Fields

    #endregion

    #region Public Interface

    public HelpMenu() : base(Strings.MenuStrings.LevelEditor.HELP_PLE_MENU) { }

    #endregion

    #region Private Fields

    #endregion  

    #region Unity Lifecycle


    #endregion

    #region Protected Interface

    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
    }

    #endregion

    #region Private Interface    

    #endregion

    #region Public Interface
    public override void SetMenuButtonInteractabilityByState() {

    }
    #endregion
}
