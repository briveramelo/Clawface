using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : Menu
{
    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return returnButton;
        }
    }

    #endregion

    #region Unity Serilization Fields

    [SerializeField]
    private Button returnButton;

    #endregion

    #region Public Interface

    public CreditsMenu() : base(Strings.MenuStrings.CREDITS) {}

    public void BackAction()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN,
            Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    #endregion

    #region Protected Interface

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion
}
