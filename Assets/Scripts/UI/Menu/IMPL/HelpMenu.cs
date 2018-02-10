using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerLevelEditor;

public class HelpMenu : Menu
{

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
    [SerializeField] private GameObject realLevelParent;

    #endregion

    #region Public Interface

    public HelpMenu() : base(Strings.MenuStrings.HELP_PLE_MENU)
    { }

    #endregion

    #region Private Fields

    private bool inputGuard = false;

    #endregion  

    #region Unity Lifecycle

    private void Update()
    {
        if (inputGuard)
        {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                BackAction();
            }

        }
    }

    #endregion

    #region Protected Interface

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion

    #region Private Interface

    private void BackAction()
    {
        MainPLEMenu menu = editorInstance.GetMenu(PLEMenu.MAIN) as MainPLEMenu;
        MenuManager.Instance.DoTransition(menu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    #endregion
}
