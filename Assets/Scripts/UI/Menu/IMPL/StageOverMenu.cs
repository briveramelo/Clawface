/**
*  @author Cornelia Schultz
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ModMan;

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

    [SerializeField] private Text score, combo, title, bonus, difficulty;    

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

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions { get {
        return new Dictionary<string, FunctionPrototype>() {
            { Strings.Events.LEVEL_COMPLETED, LevelCompleteStart},
            { Strings.Events.PLAYER_KILLED, PlayerDeathStart },
        };
    } }
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Interface

    public StageOverMenu() : base(Strings.MenuStrings.STAGE_OVER)
    {
    }

    public void DefineExitTestAction(Action onExitTest) {
        this.onExitTest = onExitTest;
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
        ObjectPool.Instance.ResetPools(); //already done in load menu?
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
        weaponMenu.DefineNavigation(null, Strings.MenuStrings.LOAD);
        LoadMenu lm = (LoadMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        lm.SetNavigation(SceneTracker.CurrentSceneName);

        EventSystem.Instance.TriggerEvent(Strings.Events.WEAPONS_SELECT_FROM_STAGE_OVER);

        MenuManager.Instance.DoTransition(weaponMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void KillAllTransitions() {
        StopAllCoroutines();
    }

    #endregion

    #region Protected Interface
    public override MenuType ThisMenuType { get { return MenuType.StageOver; } }

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
        score.text = ScoreManager.Instance.GetScore().ToCommaSeparated();
        combo.text = ScoreManager.Instance.GetHighestCombo().ToCommaSeparated();
        bonus.text = ScoreManager.Instance.GetMaxComboScore().ToCommaSeparated();
        difficulty.text = "x"+ScoreManager.Instance.GetDifficultyMultiplier().ToSimplestForm();
    }

    private void LevelCompleteStart(params object[] parameter) {
        StartCoroutine(DoLevelComplete(popUpDelay, Strings.TextStrings.STAGE_OVER_TEXT));
    }

    private void PlayerDeathStart(params object[] parameter) {
        StartCoroutine(DoLevelComplete(popUpDelay, Strings.TextStrings.GAME_OVER_TEXT));
    }

    IEnumerator DoLevelComplete(float waitTime, string titleText)
    {
        PauseMenu m = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        m.CanPause = false;

        yield return new WaitForSeconds(waitTime);
        
        title.text = titleText;
        MenuManager.Instance.DoTransition(this, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    private void SetButtonStates()
    {
        testLevelButton.gameObject.SetActive(false);
        weaponSelectButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        if(SceneTracker.IsCurrentSceneEditor) {
            testLevelButton.gameObject.SetActive(true);
            CurrentEventSystem.SetSelectedGameObject(testLevelButton.gameObject);
        }
        else {
            weaponSelectButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);

            CurrentEventSystem.SetSelectedGameObject(weaponSelectButton.gameObject);
        }
    }

    #endregion
}