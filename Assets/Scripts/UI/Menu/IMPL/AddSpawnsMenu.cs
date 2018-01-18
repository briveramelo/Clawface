using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;

public class AddSpawnsMenu : Menu {

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

    #endregion

    #region Public Interface

    public AddSpawnsMenu() : base(Strings.MenuStrings.ADD_SPAWNS_PLE)
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

    public void AddAction()
    {
        //TODO: Set Button to activated state via Sprite change
#if UNITY_EDITOR
        Debug.Log("Adding enemies ooooh");
#endif

        addingEnabled = !addingEnabled;

    }

    #endregion

}
