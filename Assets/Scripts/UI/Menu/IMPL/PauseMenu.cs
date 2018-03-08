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
        
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
		LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.SetNavigation(SceneTracker.CurrentSceneName);
        ObjectPool.Instance.ResetPools();

        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, SceneTracker.CurrentSceneName, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());


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
        //string loadMenuName = Strings.MenuStrings.LOAD;
        WeaponSelectMenu weaponSelectMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
        weaponSelectMenu.DefineNavigation(Strings.MenuStrings.PAUSE, Strings.MenuStrings.LOAD);

        LoadMenu loadMenu = (LoadMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        loadMenu.SetNavigation(SceneTracker.CurrentSceneName);

        MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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

        //disable weapon select if in level editor
        //if(!SceneTracker.IsCurrentScene80sShit)
        //{
        //    weaponSelectButton.gameObject.SetActive(false);
        //}
        
		SetPaused (true);
	}

	protected override void HideComplete ()
	{
		base.HideComplete ();
		SetPaused (false);
        //weaponSelectButton.gameObject.SetActive(true);
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
    
    #endregion
}
