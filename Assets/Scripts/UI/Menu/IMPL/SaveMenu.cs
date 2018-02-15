using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class SaveMenu : Menu {
    
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

    #region Private Fields

    private bool inputGuard = false;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if(inputGuard)
        {
            //active update loop
        }
    }

    #endregion

    #region Public Interface

    public SaveMenu() : base(Strings.MenuStrings.SAVE_PLE_MENU)
    { } 


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

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion
}
