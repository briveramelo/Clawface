/**
*  @author Cornelia Schultz
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageOverMenu : Menu
{
	#region Public Fields

	public override Button InitialSelection {
		get {
			return initialButton;
		}
	}

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
    private Button nextLevelButton;

    #endregion

    #region Unity Lifecycle

    protected override void Start()
    {
        base.Start();
        EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_COMPLETED, DoLevelComplete);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLAYER_KILLED, DoPlayerDeath);
        nextLevelButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_COMPLETED, DoLevelComplete);
        EventSystem.Instance.UnRegisterEvent(Strings.Events.PLAYER_KILLED, DoPlayerDeath);
    }

    #endregion  

    #region Public Interface

    public StageOverMenu () : base (Strings.MenuStrings.STAGE_OVER)
	{
	}

	public void QuitAction ()
	{
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
		LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.TargetScene = Strings.Scenes.MainMenu;
		loadMenu.Fast = true;
		MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
	}

    public void RestartAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        Scene scene = SceneManager.GetActiveScene();
        loadMenu.TargetScene = scene.name;

        PauseMenu p = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        p.CanPause = true;

        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, scene.name, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());


        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void WeaponSelectAction()
    {
        //Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
        //LoadMenu loadMenu = (LoadMenu)menu;
        //Scene scene = SceneManager.GetActiveScene();
        //loadMenu.TargetScene = scene.name;

        ////EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, scene.name, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());


        //MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        

        MenuManager.Instance.DoTransition(Strings.MenuStrings.WEAPON_SELECT, Transition.SHOW,
       new Effect[] { Effect.EXCLUSIVE });

    }

    public void NextLevelAction()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        Scene scene = SceneManager.GetActiveScene();
        loadMenu.TargetScene = scene.name;

        PauseMenu p = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        p.CanPause = true;
        //TODO: What is the "next" level, the next one in the build index? 

        //EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, scene.name, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());


        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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
		UpdateScores ();
	}

	protected override void ShowComplete ()
	{
		base.ShowComplete ();
	}

	protected override void HideComplete ()
	{
		base.HideComplete ();
	}

	#endregion

	#region Private Interface

	private void UpdateScores ()
	{
		score.text = ScoreManager.Instance.GetScore ().ToString ();
		combo.text = ScoreManager.Instance.GetHighestCombo ().ToString ();
	}

    private void DoLevelComplete(params object[] parameter)
    {
        PauseMenu m = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        m.CanPause = false;
        nextLevelButton.gameObject.SetActive(true);
        title.text = Strings.MenuStrings.STAGE_OVER_TEXT;
        MenuManager.Instance.DoTransition(Strings.MenuStrings.STAGE_OVER,
    Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    private void DoPlayerDeath(params object[] parameter)
    {
        PauseMenu m = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        m.CanPause = false;
        title.text = Strings.MenuStrings.GAME_OVER_TEXT;
        MenuManager.Instance.DoTransition(Strings.MenuStrings.STAGE_OVER,
Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

	#endregion
}