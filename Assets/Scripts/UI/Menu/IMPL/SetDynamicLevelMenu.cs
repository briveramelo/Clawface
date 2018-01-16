using UnityEngine.UI;
using UnityEngine;

public class SetDynamicLevelMenu : Menu {

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

    #endregion

    #region Public Interface

    public SetDynamicLevelMenu() : base(Strings.MenuStrings.SET_DYNLEVEL_PLE)
    { }

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
