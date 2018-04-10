using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PLE;

public class SaveMenu : PLEMenu {
    
    #region Public Fields

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private GameObject realLevelParent;
    [SerializeField] private InputField nameText, descriptionText;
    [SerializeField] private Button saveButton;

    #endregion

    #region Private Fields
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private LevelData WorkingLevelData { get { return ActiveDataSave.workingLevelData; } }
    private bool IsNameInputSelected { get { return CurrentEventSystemGameObject == nameText.gameObject; } }
    private bool IsDescriptionInputSelected { get { return CurrentEventSystemGameObject == descriptionText.gameObject; } }
    private bool MoveForward { get { return Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift); } }
    private bool MoveBack { get { return Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift); } }
    #endregion

    #region Unity Lifecycle      
    protected override void Update() {
        base.Update();
        if (allowInput && Displayed) {
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
        nameText.text = WorkingLevelData.name;
        descriptionText.text = WorkingLevelData.description;
    }

    protected override void ShowComplete() {
        base.ShowComplete();
        CanvasGroup.alpha = 0;
        CanvasGroup.interactable =false;
        CanvasGroup.blocksRaycasts = false;
    }

    #endregion

    #region Private Interface
    public bool IsOverwriteButtonInteractable() {
        return WorkingLevelData.isMadeByThisUser && !WorkingLevelData.IsEmpty;
    }

    void HandleInputFieldSelection() {
        if (MoveForward) {
            CurrentEventSystem.SetSelectedGameObject(descriptionText.gameObject);
        }
        else if (MoveBack) {
            CurrentEventSystem.SetSelectedGameObject(nameText.gameObject);
        }
    }

    public override void SetMenuButtonInteractabilityByState() {
        
    }
    #endregion
}
