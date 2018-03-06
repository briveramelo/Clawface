using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardsMenu : Menu
{
    #region Serialized fields
    [SerializeField]
    private Button allTimeButton;
    #endregion

    #region Public Interface
    public LeaderboardsMenu(string name) : base(name)
    {
    }

    public override Selectable InitialSelection
    {
        get
        {
            return allTimeButton;
        }
    }
    #endregion
    #region protected methods
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
