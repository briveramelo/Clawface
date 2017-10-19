using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectMenu : Menu
{
	#region Accessors (Menu)

	public override Button InitialSelection {
		get {
			return initialButton;
		}
	}

	#endregion

	#region Fields (Unity Serialization)

	[SerializeField]
	private Button initialButton;

	[SerializeField]
	private Button startButton;

	[SerializeField]
	private ButtonBundle[] weapons;

	#endregion

	#region Constructors (Public)

	public WeaponSelectMenu () : base (Strings.MenuStrings.WEAPON_SELECT)
	{
	}

	#endregion

	#region Interface (Unity Lifecycle)

	private void Update ()
	{
		PollWeaponSelect ();
	}

	#endregion

	#region Interface (Menu)

	protected override void DefaultHide (Transition transition, Effect[] effects)
	{
		Fade (transition, effects);
	}

	protected override void DefaultShow (Transition transition, Effect[] effects)
	{
		Fade (transition, effects);
	}

	protected override void ShowStarted ()
	{
		base.ShowStarted ();
		UpdateDisplay ();
	}

	#endregion

	#region Interface (Public)

	public void BackAction ()
	{
		MenuManager.Instance.DoTransition (Strings.MenuStrings.LEVEL_SELECT,
			Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
	}

	public void StartAction ()
	{
		// Acquire target level.
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LEVEL_SELECT);
		LevelSelectMenu levelMenu = (LevelSelectMenu)menu;
		string level = levelMenu.SelectedLevel;

		// Acquire Pause Menu
		menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.PAUSE);
		PauseMenu pauseMenu = (PauseMenu)menu;
		pauseMenu.CanPause = true;

		// Acquire LoadMenu and set target.
		menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
		LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.TargetScene = level;

		// Make it happen.
		MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW,
			new Effect[] { Effect.EXCLUSIVE });
	}

	#endregion

	#region Interface (Private)

	private void PollWeaponSelect ()
	{
		ButtonMode leftMode = InputManager.Instance.QueryAction (Strings.Input.Actions.FIRE_LEFT);
		ButtonMode rightMode = InputManager.Instance.QueryAction (Strings.Input.Actions.FIRE_RIGHT);

		// Fast-fail
		if (!(leftMode == ButtonMode.DOWN || rightMode == ButtonMode.DOWN))
			return;

		// Iterate through and identify selected "button"
		GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
		foreach (ButtonBundle bundle in weapons) {
			// Check to see if this weapon is selected
			if (bundle.button.gameObject == selected) {
				SetSelectedWeapon (bundle.type, leftMode == ButtonMode.DOWN);
				UpdateDisplay ();
			}
		}
	}

	private void SetSelectedWeapon (ModType type, bool isLeftArm)
	{
		if (isLeftArm) {
			ModManager.leftArmOnLoad = type;
		} else {
			ModManager.rightArmOnLoad = type;
		}
	}

	private void UpdateDisplay ()
	{
		foreach (ButtonBundle bundle in weapons) {
			bundle.leftHand.SetActive (bundle.type == ModManager.leftArmOnLoad);
			bundle.rightHand.SetActive (bundle.type == ModManager.rightArmOnLoad);
		}

		bool canStart = (ModManager.leftArmOnLoad != ModType.None &&
		                ModManager.rightArmOnLoad != ModType.None);
		startButton.interactable = canStart;
	}

	#endregion

	#region Types (Public)

	[Serializable]
	public class ButtonBundle
	{
		public Button button;
		public ModType type;
		public GameObject leftHand;
		public GameObject rightHand;
	}

	#endregion
}
