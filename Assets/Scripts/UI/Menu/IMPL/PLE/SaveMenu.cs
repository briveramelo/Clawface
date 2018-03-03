using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class SaveMenu : PlayerLevelEditorMenu {
    
    #region Public Fields

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private GameObject realLevelParent;
    [SerializeField] private InputField nameText, descriptionText;

    #endregion

    #region Private Fields
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }

    #endregion

    #region Unity Lifecycle    

    #endregion

    #region Public Interface

    public SaveMenu() : base(Strings.MenuStrings.LevelEditor.SAVE_PLE_MENU)
    { } 


    #endregion

    #region Protected Interface

    protected override void ShowStarted() {
        base.ShowStarted();
        nameText.text = ActiveDataSave.ActiveLevelData.name;
        descriptionText.text = ActiveDataSave.ActiveLevelData.description;
    }

    #endregion

    #region Private Interface
    #endregion
}
