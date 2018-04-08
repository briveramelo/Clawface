using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmMenu: Menu
{
    #region Public Fields

    public override Selectable InitialSelection { get { return mainButton; } }

    private Action onYesAction;
    private Action onNoAction;
    private Action onCancelAction;

    #endregion

    #region Internal Fields

    internal string backMenuTarget = null;

    #endregion

    #region Private Fields

    private bool inputGuard = false;

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button mainButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Text yesButtonText;
    [SerializeField] private Text noButtonText;
    [SerializeField] private Text messageText;
    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if(inputGuard)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                if (onCancelAction != null) {
                    CancelAction();
                }
                else {
                    NoAction();
                }
            }
        }
    }

    #endregion

    #region Private Interface

    #endregion

    #region Public Interface

    public void DefineActions(string i_messageToSet, Action i_onYes = null, Action i_onNo = null, Action i_onCancel = null)
    {
        messageText.text = i_messageToSet;
        onYesAction = i_onYes;
        onNoAction = i_onNo;
        onCancelAction = i_onCancel;
        cancelButton.gameObject.SetActive(i_onCancel!=null);
    }

    public ConfirmMenu() : base(Strings.MenuStrings.CONFIRM) {}

    public void SetYesButtonText(string i_string)
    {
        yesButtonText.text = i_string.ToUpper();
    }

    public void SetNoButtonText(string i_string)
    {
        noButtonText.text = i_string.ToUpper();
    }

    public void CancelAction() {
        MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { Effect.INSTANT });
        SFXManager.Instance.Play(SFXType.UI_Click);
        if (onCancelAction != null) {
            onCancelAction();
        }
    }

    public void YesAction()
    {
        MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { Effect.INSTANT });
        SFXManager.Instance.Play(SFXType.UI_Click);
        if (onYesAction != null)
        {
            onYesAction();
        }
    }

    public void NoAction()
    {
        MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { Effect.INSTANT });
        SFXManager.Instance.Play(SFXType.UI_Click);
        if (onNoAction != null)
        {
            onNoAction();
        }

    }

    public void SetYesButtonInteractibility(bool i_state)
    {
        yesButton.interactable = i_state;
    }

    public void SetNoButtonInteractibility(bool i_state)
    {
        noButton.interactable = i_state;
    }

    #endregion

    #region Protected Interface
    public override MenuType ThisMenuType { get { return MenuType.Confirm; } }
    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
     }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
        SetNoButtonInteractibility(true);
        SetYesButtonInteractibility(true);
        SetNoButtonText("NO");
        SetYesButtonText("YES");
    }

    #endregion
}
