﻿using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LevelSelectMenu : Menu
{
    #region Accessors (Menu)

    public override Button InitialSelection
    {
        get
        {
            foreach(ThemeBundle bundle in themes)
            {
                if (bundle.theme == selectedTheme)
                {
                    if (!chooseLevel)
                    {
                        return bundle.button;
                    } else
                    {
                        return bundle.levels.buttons[0];
                    }
                }
            }

            return backButton;
        }
    }

    #endregion

    #region Accessors (Public)

    public string SelectedLevel
    {
        get
        {
            return selectedLevel;
        }
    }

    public bool ShouldResetLevel
    {
        set
        {
            resetLevel = value;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [SerializeField]
    private Button backButton;

    [SerializeField]
    private Button weaponSelectButton;

    [SerializeField]
    private Animator themeController;

    [SerializeField]
    private ThemeBundle[] themes;

    #endregion

    #region Fields (Private)

    private string selectedLevel = null;
    private bool resetLevel = false;
    private Theme selectedTheme = Theme.COMMON_AREA;
    private bool chooseLevel = false;

    #endregion

    #region Constructors (Public)

    public LevelSelectMenu() : base(Strings.MenuStrings.LEVEL_SELECT) {}

    #endregion

    #region Interface (Unity Serialization)

    private void Update()
    {
        // check to see  if the cancel button was pushed
        if (chooseLevel && InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
        {
            chooseLevel = false;
            SwitchMenus(true);
            UpdateDisplay();
        }
    }

    #endregion

    #region Interface (Menu)

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);

        // This menu needs to be exclusive.
        MenuManager.Instance.DoTransitionOthers(this, Transition.HIDE, new Effect[] { });
    }

    protected override void ShowStarted()
    {
        base.ShowStarted();
        if (resetLevel)
        {
            resetLevel = false; // reset this since we don't always want to reset the level
            selectedLevel = null;
        }
        UpdateDisplay();
    }

    #endregion

    #region Interface (Public)

    public void BackAction()
    {
        // Currently, these menus are displayed in the same scene
        // as the main menu.  As such, we merely have to display the main menu.
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Transition.SHOW,
            new Effect[] { Effect.EXCLUSIVE });
    }

    public void WeaponSelectAction()
    {
        // Transition to Weapon Select.
        MenuManager.Instance.DoTransition(Strings.MenuStrings.WEAPON_SELECT, Transition.SHOW,
            new Effect[] { Effect.EXCLUSIVE });
    }

    public void ThemeSelector(Theme theme)
    {
        selectedTheme = theme;
        UpdateDisplay();
    }

    public void ThemeAction(Theme theme)
    {
        chooseLevel = true;
        SwitchMenus(false);
        UpdateDisplay();
    }

    public void LevelAction(string scene)
    {
        selectedLevel = scene;
        EnableWeaponSelect(true);
    }

    #endregion

    #region Interface (Private)

    private void SwitchMenus(bool isTheme)
    {
        // Iterate through bundles.
        foreach (ThemeBundle bundle in themes)
        {
            // Find the one for the current theme
            if (bundle.theme == selectedTheme)
            {
                // Determine which menu we should be operating in.
                if (isTheme)
                {
                    bundle.button.Select();
                } else
                {
                    bundle.levels.buttons[0].Select();
                }
            }
        }
    }

    private void UpdateDisplay()
    {
        AlterVisible();
        RewireNavigation();
        EnableWeaponSelect(selectedLevel != null);
    }

    private void AlterVisible()
    {
        // Only display the theme selector if we're not choosing a level
        themeController.SetBool("Visible", !chooseLevel);

        // Iterate through themes to alter visibilities
        foreach (ThemeBundle themeBundle in themes)
        {
            bool levelsVisible = themeBundle.theme == selectedTheme;
            themeBundle.levels.controller.SetBool("Visible", levelsVisible);
            themeBundle.levels.controller.SetBool("Interactable", levelsVisible && chooseLevel);
        }
    }

    private void RewireNavigation()
    {
        // Acquire ThemeBundle
        ThemeBundle bundle = null;
        foreach (ThemeBundle innerBundle in themes)
        {
            if (innerBundle.theme == selectedTheme)
            {
                bundle = innerBundle;
                break;
            }
        }
        Assert.IsNotNull(bundle);

        // Select correct navigation button
        Button button;
        if (!chooseLevel)
        {
            button = bundle.button;
        } else
        {
            LevelBundle levelBundle = bundle.levels;
            button = levelBundle.buttons[levelBundle.buttons.Length - 1];
        }

        // Set Navigation options
        Navigation backNav = backButton.navigation;
        Navigation selectNav = weaponSelectButton.navigation;
        backNav.selectOnUp = button;
        selectNav.selectOnUp = button;
        backButton.navigation = backNav;
        weaponSelectButton.navigation = selectNav;
    }

    private void EnableWeaponSelect(bool enabled)
    {
        weaponSelectButton.interactable = enabled;
    }

    #endregion

    #region Types (Public)

    public enum Theme
    {
        COMMON_AREA,
        GREENHOUSE,
        LABORATORY,
        ENGINEERING
    }

    [Serializable]
    public class ThemeBundle
    {
        public Theme theme;
        public LevelBundle levels;
        public Button button;
    }

    [Serializable]
    public class LevelBundle
    {
        public Animator controller;
        public Button[] buttons;
    }

    #endregion
}
