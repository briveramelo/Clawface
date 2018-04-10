using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PLE;
using System;

public enum SteamMenuSubMenu {
    BaseMenu,
    UploadMenu,
    DownloadMenu,
    UploadSuccessMenu
}
public class SteamMenu : PLEMenu {

    public SteamMenu() : base(Strings.MenuStrings.LevelEditor.STEAM_PLE_MENU) { }

    #region Unity Serialized Fields
    [SerializeField] private SteamLevelLoader steamLevelLoader;
    [SerializeField] private LoadingText uploadLoadingText, downloadLoadingText;
    [SerializeField] private GameObject baseMenu, uploadMenu, downloadMenu, uploadSuccessMenu, downloadHelpTextParent, downloadLoadingTextParent;
    [SerializeField] private Text nameText, descriptionText, uploadingWaitingText;
    [SerializeField] private Selectable waitingMenuBackButton, uploadButton;
    [SerializeField] private Image levelImage;
    [SerializeField] private string steamWorkshopUrl;
    #endregion

    #region Private Fields
    private bool isWaitingForSave;
    private LevelData WorkingLevelData { get { return DataPersister.ActiveDataSave.workingLevelData; } }
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
    #endregion

    #region Unity LifeCycle
    protected override void Awake() {
        base.Awake();
        Action<bool> onToggleWaiting = (value) => {
            waitingMenuBackButton.interactable = !value;
            mainPLEMenu.SetMenuButtonInteractabilityByState();
        };
        uploadLoadingText.SetOnToggleWaitingForConfirmation(onToggleWaiting);
        downloadLoadingText.SetOnToggleWaitingForConfirmation(onToggleWaiting);
    }

    protected override void Update() {
        if (allowInput && !IsWaitingForConfirmation) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN)) {
                BackAction();
            }
        }
    }
    #endregion
    
    #region Public Interface
    public bool IsWaitingForConfirmation { get { return uploadLoadingText.IsWaitingForConfirmation || downloadLoadingText.IsWaitingForConfirmation || isWaitingForSave; } }
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
        SyncWorkshopData();
    }
    public void ShowUploadMenu() {
        ShowMenuExclusively(uploadMenu);
    }
    public void ShowBaseMenu() {
        ShowMenuExclusively(baseMenu);
    }

    public void Upload() {
        isWaitingForSave = true;
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        levelEditor.levelDataManager.SaveAndUploadSingleLevel(OnGetUploadResult, OnSaveComplete);
    }
    
    public void ShowSteamWorkshop() {
        Application.OpenURL(steamWorkshopUrl);
    }

    public void SyncWorkshopData() {
        downloadLoadingTextParent.SetActive(true);
        downloadHelpTextParent.SetActive(false);
        downloadLoadingText.IsWaitingForConfirmation = true;        
        steamLevelLoader.LoadSteamworkshopFiles(OnSteamLevelsLoaded);
    }

    public override void SetMenuButtonInteractabilityByState() {
        bool activeLevelDataHasBeenSaved = !WorkingLevelData.IsEmpty;
        uploadButton.interactable = activeLevelDataHasBeenSaved;
    }
    #endregion

    #region Protected
    protected override void ShowStarted() {
        base.ShowStarted();
        isWaitingForSave = false;
        uploadLoadingText.SetSuccess(false);
        downloadLoadingText.SetSuccess(false);

        ShowMenuExclusively(baseMenu);
        nameText.text = WorkingLevelData.name;
        descriptionText.text = WorkingLevelData.description;
        levelImage.sprite = WorkingLevelData.MySprite;
    }

    protected override void HideStarted() {
        base.HideStarted();
        isWaitingForSave = false;
        uploadLoadingText.SetSuccess(false);
        downloadLoadingText.SetSuccess(false);
    }
    #endregion

    #region Private Interface
    private void OnSteamLevelsLoaded() {
        downloadLoadingText.IsWaitingForConfirmation = false;
        downloadLoadingTextParent.SetActive(false);
        downloadHelpTextParent.SetActive(true);
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

    private void ShowMenuExclusively(GameObject toShow) {
        MenuObjects.ForEach(menu => {
            bool show = toShow == menu;
            menu.SetActive(show);
        });
    }

    private void OnGetUploadResult(bool success) {
        isWaitingForSave = false;
        uploadLoadingText.SetSuccess(success);
    }

    private void OnSaveComplete() {
        ShowMenuExclusively(uploadSuccessMenu);
        uploadLoadingText.IsWaitingForConfirmation = true;
    }
    #endregion
}
