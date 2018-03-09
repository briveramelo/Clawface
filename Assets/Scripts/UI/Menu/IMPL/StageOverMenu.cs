﻿/**
*  @author Cornelia Schultz
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageOverMenu : Menu
{
    #region Public Fields

    public override Selectable InitialSelection
    {
        get
        {
            return initialButton;
        }
    }
    public bool IsDisplaying { get; private set; }
    #endregion

    #region Serialized Unity Fields

    [SerializeField]
    private Button initialButton;

    [SerializeField]
    private Text score;

    [SerializeField]
    private Text combo;

    [SerializeField]
    private Text title;    

    [SerializeField]
    private float popUpDelay = 2.0f;

    [SerializeField] private Button testLevelButton;
    [SerializeField] private Button weaponSelectButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    #endregion

    #region Private Fields

    private Action onExitTest;

    #endregion

    #region Unity Lifecycle

    protected override void Start()
    {
        base.Start();
        if (EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, LevelCompleteStart);
            EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, PlayerDeathStart);
        }        
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, LevelCompleteStart);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, PlayerDeathStart);
        }
    }

    #endregion  

    #region Public Interface

    public StageOverMenu() : base(Strings.MenuStrings.STAGE_OVER)
    {
    }

    public void DefineNavigation(Action i_act) {
        onExitTest = i_act;
    }

    public void ExitTestAction()
    {
        if (onExitTest != null)
        {
            onExitTest();
        }
                
    }

    public void QuitAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_QUIT, SceneTracker.CurrentSceneName, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());
        loadMenu.SetNavigation(Strings.Scenes.ScenePaths.MainMenu);
        loadMenu.Fast = true;
        ObjectPool.Instance.ResetPools();
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void RestartAction()
    {
        Action callRestartEventAction = () =>
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, SceneTracker.CurrentSceneName, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());
        };
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.SetNavigation(SceneTracker.CurrentSceneName, callRestartEventAction);

        PauseMenu p = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        p.CanPause = true;

        ObjectPool.Instance.ResetPools();        
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void WeaponSelectAction()
    {
        
        WeaponSelectMenu weaponMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
        weaponMenu.DefineNavigation(Strings.Scenes.ScenePaths.MainMenu, Strings.MenuStrings.LOAD);
        LoadMenu lm = (LoadMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        lm.SetNavigation(SceneTracker.CurrentSceneName);

        EventSystem.Instance.TriggerEvent(Strings.Events.WEAPONS_SELECT_FROM_STAGE_OVER);

        MenuManager.Instance.DoTransition(weaponMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void NextLevelAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.SetNavigation(SceneTracker.CurrentSceneName);

        PauseMenu pauseMenu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        pauseMenu.CanPause = true;
        //TODO: What is the "next" level, the next one in the build index? 

        //EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, SceneTracker.CurrentSceneName, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());


        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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

    protected override void ShowStarted()
    {
        base.ShowStarted();
        IsDisplaying = true;
        UpdateScores();
        SetButtonStates();
    }

    protected override void ShowComplete()
    {
        base.ShowComplete();
    }

    protected override void HideComplete()
    {
        base.HideComplete();
        IsDisplaying = false;
    }

    #endregion

    #region Private Interface

    private void UpdateScores()
    {
        score.text = ScoreManager.Instance.GetScore().ToString();
        combo.text = ScoreManager.Instance.GetHighestCombo().ToString();
    }

    IEnumerator DoLevelComplete(params object[] parameter)
    {
        PauseMenu m = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        m.CanPause = false;

        yield return new WaitForSeconds((float)parameter[0]);
        
        title.text = Strings.TextStrings.STAGE_OVER_TEXT;
        MenuManager.Instance.DoTransition(Strings.MenuStrings.STAGE_OVER,
            Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    private void LevelCompleteStart(params object[] parameter)
    {
        StartCoroutine(DoLevelComplete(popUpDelay));
    }

    IEnumerator DoPlayerDeath(params object[] parameter)
    {
        PauseMenu m = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        m.CanPause = false;

        yield return new WaitForSeconds((float)parameter[0]);

        title.text = Strings.TextStrings.GAME_OVER_TEXT;
        MenuManager.Instance.DoTransition(Strings.MenuStrings.STAGE_OVER,
            Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    private void PlayerDeathStart(params object[] parameter)
    {
        StartCoroutine(DoPlayerDeath(popUpDelay));
    }

    private void SetButtonStates()
    {

        testLevelButton.gameObject.SetActive(false);
        weaponSelectButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        if(SceneTracker.IsCurrentSceneEditor)
        {
            testLevelButton.gameObject.SetActive(true);
        }
        else
        {
            weaponSelectButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }

    }

    #endregion
}