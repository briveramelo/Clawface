﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerLevelEditor;

public class WaveMenu : Menu
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

    public WaveMenu() : base(Strings.MenuStrings.MAIN_PLE_MENU)
    { }

    #endregion

    #region Private Fields

    private bool inputGuard = false;

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

    public void BackAction()
    {
        MenuManager.Instance.DoTransition(editorInstance.GetMenu(PLEMenu.MAIN), Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    #endregion
}
