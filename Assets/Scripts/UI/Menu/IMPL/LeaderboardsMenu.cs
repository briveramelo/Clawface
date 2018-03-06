using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardsMenu : Menu
{
    #region Public methods
    public override Button InitialSelection
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion

    #region Public Interface
    public LeaderboardsMenu() : base(Strings.MenuStrings.LEADER_BOARDS) { }

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
