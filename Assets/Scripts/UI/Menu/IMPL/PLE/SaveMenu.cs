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
    [SerializeField] private Button saveNewButton;

    #endregion

    #region Private Fields
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private LevelData ActiveLevelData { get { return ActiveDataSave.ActiveLevelData; } }
    private bool IsNameInputSelected { get { return CurrentEventSystemGameObject == nameText.gameObject; } }
    private bool IsDescriptionInputSelected { get { return CurrentEventSystemGameObject == descriptionText.gameObject; } }
    private bool MoveForward { get { return Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift); } }
    private bool MoveBack { get { return Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift); } }
    #endregion

    #region Unity Lifecycle    
    protected override void Update() {
        base.Update();
        if (allowInput) {
            HandleInputFieldSelection();
        }
    }
    #endregion

    #region Public Interface

    public SaveMenu() : base(Strings.MenuStrings.LevelEditor.SAVE_PLE_MENU)
    { }

    public void Save() {
        levelEditor.levelDataManager.SaveLevel();
        BackAction();
    }

    public void SaveAs() {
        levelEditor.levelDataManager.SaveNewLevel();
        BackAction();
    }

    #endregion

    #region Protected Interface

    protected override void ShowStarted() {
        base.ShowStarted();
        CurrentEventSystem.SetSelectedGameObject(nameText.gameObject);
        nameText.text = ActiveLevelData.name;
        descriptionText.text = ActiveLevelData.description;
    }

    #endregion

    #region Private Interface
    public bool IsOverwriteButtonInteractable() {
        return ActiveLevelData.isMadeByThisUser && !ActiveLevelData.IsEmpty;
    }

    void HandleInputFieldSelection() {
        if (MoveForward) {
            CurrentEventSystem.SetSelectedGameObject(descriptionText.gameObject);
        }
        else if (MoveBack) {
            CurrentEventSystem.SetSelectedGameObject(nameText.gameObject);
        }
    }
    #endregion
}
