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

    #region Serialized Unity Fields

    [SerializeField] private Button mainButton;
    [SerializeField] private Text messageText;
    #endregion

    #region Public Interface

    public void SetMessage(string i_msg)
    {
        messageText.text = i_msg;
    }

    public void DefineActions(Action i_onYes = null, Action i_onNo = null)
    {
        onYesAction = i_onYes;
        onNoAction = i_onNo;
    }

    public ConfirmMenu() : base(Strings.MenuStrings.CONFIRM) {}

  
    public void YesAction()
    {
        if(onYesAction != null)
        {
            onYesAction();
        }
    }

    public void NoAction()
    {
        if(onNoAction != null)
        {
            onNoAction();
        }

    }
    

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    #endregion
}
