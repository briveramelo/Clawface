﻿using Rewired.Data;
using UnityEngine;
using UnityEngine.UI;

public class ControlMapperMenu : Menu
{
    #region Public Accessors

    public override Button InitialSelection
    {
        get
        {
            return doneButton;
        }
    }

    public UserDataStore_PlayerPrefs UserDataStore
    {
        get
        {
            return userDataStore;
        }
    }
    #endregion

    #region Unity Serializations

    [SerializeField]
    private Button doneButton;

    [SerializeField]
    private UserDataStore_PlayerPrefs userDataStore;

    #endregion

    #region Public Interface
    public ControlMapperMenu() : base(Strings.MenuStrings.CONTROLS) {}

    public void DoneEditingControls()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.SETTINGS);
        SettingsMenu settings = menu as SettingsMenu;
        settings.shouldInitialize = false;
        MenuManager.Instance.DoTransition(menu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE } );
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
