using UnityEngine.UI;
using UnityEngine;

public class MainPLEMenu : Menu {

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

    public MainPLEMenu() : base(Strings.MenuStrings.MAIN_PLE_MENU)
    { }

    #endregion

    #region Private Interface 

    private bool addingEnabled;

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

    public void OpenPropsAction()
    {
        //TODO: Set Button to activated state via Sprite change
#if UNITY_EDITOR
        Debug.Log(" ooooh");
#endif

        addingEnabled = !addingEnabled;

    }

    #endregion

}
