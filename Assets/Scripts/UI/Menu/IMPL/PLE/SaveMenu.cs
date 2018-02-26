using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class SaveMenu : Menu {
    
    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return initiallySelected;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private LevelEditor editorInstance;
    [SerializeField] private GameObject realLevelParent;
    [SerializeField] private PLECameraController cameraController;
    [SerializeField] private InputField nameText, descriptionText;

    #endregion

    #region Private Fields
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private bool inputGuard = false;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if(inputGuard)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP)) {
                BackAction();
            }
        }
    }

    #endregion

    #region Public Interface

    public SaveMenu() : base(Strings.MenuStrings.SAVE_PLE_MENU)
    { } 


    #endregion

    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
    }

    protected override void ShowStarted()
    {
        base.ShowStarted();
        cameraController.enabled = false;
        nameText.text = ActiveDataSave.ActiveLevelData.name;
        descriptionText.text = ActiveDataSave.ActiveLevelData.description;
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
        cameraController.enabled = true;
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    public void BackAction()
    {
        Menu menuToHide = editorInstance.GetMenu(PLEMenu.SAVE);
        MenuManager.Instance.DoTransition(menuToHide, Menu.Transition.HIDE, new Menu.Effect[] { });
    }

    #endregion
}
