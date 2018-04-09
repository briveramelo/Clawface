using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum SteamMenuSubMenu {
    BaseMenu,
    UploadMenu,
    DownloadMenu,
    UploadSuccessMenu
}
public class SteamMenu : PLEMenu {

    public SteamMenu() : base(Strings.MenuStrings.LevelEditor.STEAM_PLE_MENU) { }    

    #region Unity Serialized Fields
    [SerializeField] private GameObject baseMenu, uploadMenu, downloadMenu, uploadSuccessMenu;
    [SerializeField] private Text nameText, descriptionText, uploadingWaitingText;
    [SerializeField] private Selectable uploadSuccessMenuBackButton, uploadButton;
    [SerializeField] private Image levelImage;
    [SerializeField] private string successMessage, failureMessage, uploadingWaitingString;
    #endregion

    LevelData WorkingLevelData { get { return DataPersister.ActiveDataSave.workingLevelData; } }
    private List<GameObject> menuObjects;
    private List<GameObject> MenuObjects {
        get {
            if (menuObjects == null || (menuObjects!=null && menuObjects.Count==0)) {
                menuObjects = new List<GameObject>() {
                    baseMenu, uploadMenu, downloadMenu, uploadSuccessMenu
                };
            }
            return menuObjects;
        }
    }
    private bool isWaitingForConfirmation;
    public bool IsWaitingForConfirmation {
        get {
            return isWaitingForConfirmation;
        }
        private set {
            isWaitingForConfirmation = value;
            uploadSuccessMenuBackButton.interactable = !value;
        }
    }
    private bool wasUploadSuccessful = false;

    protected override void Update() {
        if (allowInput && !IsWaitingForConfirmation) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN)) {
                BackAction();
            }
        }
    }

    //for custom editor
    public void ShowMenu(SteamMenuSubMenu subMenu) {
        GameObject selectedMenu= baseMenu;
        switch (subMenu) {
            case SteamMenuSubMenu.BaseMenu: selectedMenu = baseMenu; break;
            case SteamMenuSubMenu.DownloadMenu: selectedMenu = downloadMenu; break;
            case SteamMenuSubMenu.UploadMenu: selectedMenu = uploadMenu; break;
            case SteamMenuSubMenu.UploadSuccessMenu: selectedMenu = uploadSuccessMenu; break;
        }
        ShowMenuExclusively(selectedMenu);
    }

    public void ShowDownloadMenu() {
        ShowMenuExclusively(downloadMenu);
    }
    public void ShowUploadMenu() {
        ShowMenuExclusively(uploadMenu);
    }
    public void ShowBaseMenu() {
        ShowMenuExclusively(baseMenu);
    }

    public void Upload() {
        //upload to steam with callback: OnGetUploadResult(bool success)
        IsWaitingForConfirmation = true;
        uploadingWaitingText.text = uploadingWaitingString;
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        StartCoroutine(DelayUploadMessage());
        ShowMenuExclusively(uploadSuccessMenu);
    }

    public void ShowSteamWorkshop() {
        //nothing happens
    }

    public override void SetMenuButtonInteractabilityByState() {
        bool activeLevelDataHasBeenSaved = !WorkingLevelData.IsEmpty;
        uploadButton.interactable = activeLevelDataHasBeenSaved;
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        IsWaitingForConfirmation = false;
        wasUploadSuccessful = false;
        ShowMenuExclusively(baseMenu);
        nameText.text = WorkingLevelData.name;
        descriptionText.text = WorkingLevelData.description;
        levelImage.sprite = WorkingLevelData.MySprite;
    }

    protected override void HideStarted() {
        base.HideStarted();
        IsWaitingForConfirmation = false;
        wasUploadSuccessful = false;
    }

    private void ShowMenuExclusively(GameObject toShow) {
        MenuObjects.ForEach(menu => {
            bool show = toShow == menu;
            menu.SetActive(show);
        });
    }

    private void OnGetUploadResult(bool success) {
        IsWaitingForConfirmation = false;
        wasUploadSuccessful = success;
    }

    IEnumerator DelayUploadMessage() {
        const float maxWaitTime = 7f;
        const float ellipsesInterval = .4f;
        float timeWaited = 0f;
        float ellipsesTimer = 0f;
        int ellipsesIndex = 0;
        while (IsWaitingForConfirmation && timeWaited<maxWaitTime) {
            timeWaited += Time.deltaTime;
            ellipsesTimer += Time.deltaTime;
            if (ellipsesTimer> ellipsesInterval) {
                ellipsesTimer = 0f;
                ellipsesIndex++;
                UpdateEllipsesText(ellipsesIndex);
            }
            yield return null;
        }
        IsWaitingForConfirmation = false;
        uploadingWaitingText.text = wasUploadSuccessful ? successMessage : failureMessage;
    }

    void UpdateEllipsesText(int index) {
        int newIndex = index % 4;
        string finalString = uploadingWaitingString;
        for (int i = 0; i < newIndex; i++) {
            finalString += ".";
        }
        uploadingWaitingText.text = finalString;
    }
}
