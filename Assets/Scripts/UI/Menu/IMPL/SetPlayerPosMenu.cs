using UnityEngine.UI;
using UnityEngine;

public class SetPlayerPosMenu : Menu {

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

    #region Private Interface 

    private bool settingEnabled;

    #endregion

    #region Public Interface

    public SetPlayerPosMenu() : base(Strings.MenuStrings.SET_PLAYER_POS_PLE)
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

    #region Private Interface
    
    public void SetAction()
    {

#if UNITY_EDITOR
        Debug.Log("Setting player position ooooh");
#endif
        settingEnabled = !settingEnabled;

    }

    #endregion
}
