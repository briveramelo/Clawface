/**
*  @author Cornelia Schultz
*/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : Menu {

    #region Public Fields

    public override Selectable InitialSelection {
        get {
            return initiallySelected;
        }
    }

    public bool CanPause {
        get {
            return canPause;
        }
        set {
            canPause = value;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private Button weaponSelectButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    #endregion

    #region Private Fields
    public bool IsPaused{ get { return paused; } }
	private bool paused = false;
	private bool canPause = true;
    // used to indicate the game is in a level and "can pause"

    #endregion

    #region Unity Lifecycle Methods
    private new void Start()
    {
        base.Start();
        EventSystem.Instance.RegisterEvent(Strings.Events.GAME_CAN_PAUSE, GameCanPause);
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.GAME_CAN_PAUSE, GameCanPause);
        }
    }

    void Update ()
	{
		if (canPause && InputManager.Instance.QueryAction (Strings.Input.Actions.PAUSE,
			          ButtonMode.DOWN)) {
			if (!paused && !Displayed) {
				MenuManager.Instance.DoTransition (this, Transition.TOGGLE, new Effect[] { });
			} else if (Displayed) {
				MenuManager.Instance.ClearMenus ();
			}
		}
	}

	#endregion

	#region Public Interface

	public PauseMenu () : base (Strings.MenuStrings.PAUSE)
	{
	}

	public void restartAction ()
	{
        Action callRestartEventAction = () =>
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, SceneTracker.CurrentSceneName, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());
        };
        Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
		LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.SetNavigation(SceneTracker.CurrentSceneName, callRestartEventAction);
        ObjectPool.Instance.ResetPools();

        


        MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
	}

	public void quitAction ()
	{
		canPause = false;
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_QUIT, SceneManager.GetActiveScene().name, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());
        LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.SetNavigation(Strings.Scenes.ScenePaths.MainMenu);
		loadMenu.Fast = true;
        ObjectPool.Instance.ResetPools();
        MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
	}

    public void ResumeAction()
    {
        MenuManager.Instance.ClearMenus();
    }

    public void WeaponSelectAction()
    {
        Action OnYesAction = () =>
        {
            string currentScene = SceneTracker.CurrentSceneName;
           

            Action LoadWeaponSelectAction = () =>
            {
                
                WeaponSelectMenu weaponMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
                weaponMenu.DefineNavigation(Strings.MenuStrings.MAIN, Strings.MenuStrings.LOAD);
                LoadMenu lm = (LoadMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
                lm.SetNavigation(currentScene);
                
                MenuManager.Instance.DoTransition(weaponMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
            };

            MenuManager.Instance.DoTransition(MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM), Transition.HIDE, new Effect[] { });
            LoadMenu loadMenu = (LoadMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
            loadMenu.SetNavigation(Strings.Scenes.ScenePaths.MainMenu,LoadWeaponSelectAction);

            MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });

        };

        Action OnNoAction = () =>
        {
            MenuManager.Instance.DoTransition(MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM), Transition.HIDE, new Effect[] { });
            this.SelectInitialButton();
        };

        ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);

        confirmMenu.DefineActions("This will end your current game are you sure?",OnYesAction,OnNoAction);

        MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] {  });
    }

    #endregion

    #region Protected Interface

    protected override void DefaultShow (Transition transition, Effect[] effects)
	{
		Fade (transition, effects);
	}

	protected override void DefaultHide (Transition transition, Effect[] effects)
	{
		Fade (transition, effects);
	}

	protected override void ShowStarted ()
	{
		base.ShowStarted ();
        SetButtonStates();

        SetPaused (true);
	}

	protected override void HideComplete ()
	{
		base.HideComplete ();
		SetPaused (false);
    }

	#endregion

	#region Private Interface

	private void SetPaused (bool paused)
	{
		this.paused = paused;
		Time.timeScale = paused ? 0 : 1;
	}

    private void GameCanPause(object[] parameters)
    {
        if (parameters.Length > 0) {            
            canPause = (bool) parameters[0];
        }
    }

    private void SetButtonStates()
    {
        restartButton.gameObject.SetActive(false);
        weaponSelectButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        if(!SceneTracker.IsCurrentSceneEditor)
        {
            restartButton.gameObject.SetActive(true);
            weaponSelectButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }
    }
    
    #endregion
}
