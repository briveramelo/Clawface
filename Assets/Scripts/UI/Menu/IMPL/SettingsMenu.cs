using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    #region Public Accessors

    public override Selectable InitialSelection
    {
        get
        {
            return doneButton;
        }
    }
    #endregion

    #region Unity Serializations

    [SerializeField]
    private Button doneButton;

    #endregion

    #region Public Interface
    public SettingsMenu() : base(Strings.MenuStrings.CONTROLS) {}

    public void DoneEditingControls()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE } );
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
