using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmMenu: Menu
{
    #region Public Fields

    public override Selectable InitialSelection { get { return mainButton; } }

    private Action onYesAction;
    private Action onNoAction;

    #endregion

    #region Internal Fields

    internal string backMenuTarget = null;

    #endregion

    #region Private Fields

    private bool inputGuard = false;

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button mainButton;
    [SerializeField] private Text messageText;
    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if(inputGuard)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                NoAction();
            }
        }
    }

    #endregion

    #region Private Interface

    #endregion

    #region Public Interface

    public void DefineActions(string i_messageToSet, Action i_onYes = null, Action i_onNo = null)
    {
        messageText.text = i_messageToSet;
        onYesAction = i_onYes;
        onNoAction = i_onNo;
    }

    public void DefineNavigation(string i_backMenuTarget)
    {
        backMenuTarget = i_backMenuTarget;
    }

    public ConfirmMenu() : base(Strings.MenuStrings.CONFIRM) {}

  
    public void YesAction()
    {
        MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { });

        if (onYesAction != null)
        {
            onYesAction();
        }
    }

    public void NoAction()
    {
        MenuManager.Instance.DoTransition(this, Transition.HIDE, new Effect[] { });

        if (onNoAction != null)
        {
            onNoAction();
        }

    }

    #endregion

    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
     }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;

    }

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    #endregion
}
