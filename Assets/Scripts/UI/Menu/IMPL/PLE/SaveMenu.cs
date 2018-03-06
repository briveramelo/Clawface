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
    [SerializeField] private Button saveNewButton, overwriteButton;

    #endregion

    #region Private Fields
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private LevelData ActiveLevelData { get { return ActiveDataSave.ActiveLevelData; } }
    private UnityEngine.EventSystems.EventSystem CurrentEventSystem { get { return UnityEngine.EventSystems.EventSystem.current; } }
    private GameObject CurrentSelectedGameObject { get { return UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject; } }
    private bool IsNameInputSelected { get { return CurrentSelectedGameObject == nameText; } }
    private bool IsDescriptionInputSelected { get { return CurrentSelectedGameObject == descriptionText; } }
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

    public void OverwriteLevel() {
        levelEditor.levelDataManager.SaveLevel();
        BackAction();
    }

    public void SaveNewLevel() {
        levelEditor.levelDataManager.SaveNewLevel();
        BackAction();
    }

    #endregion

    #region Protected Interface

    protected override void ShowStarted() {
        base.ShowStarted();
        CurrentEventSystem.SetSelectedGameObject(nameText.gameObject);
        SetButtonInteractability();
        nameText.text = ActiveLevelData.name;
        descriptionText.text = ActiveLevelData.description;
    }

    #endregion

    #region Private Interface
    void SetButtonInteractability() {
        bool interactable = ActiveLevelData.isMadeByThisUser;
        overwriteButton.interactable = interactable;
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
