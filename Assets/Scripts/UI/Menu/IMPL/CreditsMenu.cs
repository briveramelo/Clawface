using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : Menu
{
    #region Public Fields

    public override Selectable InitialSelection
    {
        get
        {
            return returnButton;
        }
    }

    #endregion

    #region Private Fields

    bool inputGuard = false;

    #endregion


    #region Unity Serialized Fields

    [SerializeField] private Button returnButton;

    #endregion

    #region Public Interface

    public CreditsMenu() : base(Strings.MenuStrings.CREDITS) {}

    public void BackAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN,
            Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if (allowInput && InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
        {
            BackAction();
        }
    }

    #endregion

    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
    }

    protected override void HideStarted()
    {
        base.HideStarted();
    }

    #endregion
}
