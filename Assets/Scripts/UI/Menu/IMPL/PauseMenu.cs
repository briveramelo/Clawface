/**
*  @author Cornelia Schultz
*/
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : Menu
{

	#region Public Fields

	public override Button InitialSelection {
		get {
			return restartButton;
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

	[SerializeField]
	private Button restartButton;

	#endregion

	#region Private Fields

	private bool paused = false;
	private bool canPause = true;
    // used to indicate the game is in a level and "can pause"

    #endregion

    #region Unity Lifecycle Methods

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
		Scene scene = SceneManager.GetActiveScene ();
		loadMenu.TargetScene = scene.name;

        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_RESTARTED, scene.name, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());


        MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
	}

	public void quitAction ()
	{
		canPause = false;
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_QUIT, SceneManager.GetActiveScene().name, AnalyticsManager.Instance.GetCurrentWave(), ScoreManager.Instance.GetScore());
        LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.TargetScene = Strings.Scenes.MainMenu;
		loadMenu.Fast = true;
		MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
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

	#endregion
}
