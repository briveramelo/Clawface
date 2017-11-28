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
	private Button initialButton = null;

	[SerializeField]
	private Button startButton = null;

	[SerializeField]
	private ButtonBundle[] weapons = null;

    #endregion

    #region Fields (Internal)

    internal string menuTarget = null;

    #endregion

    #region Fields (Private)

    ModType[] types = new ModType[2];
    bool inputGuard = false;

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

        if (inputGuard && InputManager.Instance.QueryAction (Strings.Input.UI.CANCEL, ButtonMode.DOWN))
        {
            BackButtonBehaviour();
        }
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
        types[0] = ModManager.leftArmOnLoad;
        types[1] = ModManager.rightArmOnLoad;
        UpdateWeaponsStatus();
        UpdateDisplay ();
	}

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

    #region Interface (Public)

    public void BackAction ()
	{
		MenuManager.Instance.DoTransition (menuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });

	}

	public void StartAction ()
	{
        // Set Mod Types
        ModManager.leftArmOnLoad = types[0];
        ModManager.rightArmOnLoad = types[1];

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

        // Trigger level started event
        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, loadMenu.TargetScene, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());

		// Make it happen.
		MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW,
			new Effect[] { Effect.EXCLUSIVE });
	}

    public void ActionSelectWeapon ()
    {
        bool leftSelected = types[0] != ModType.None;
        bool rightSelected = types[1] != ModType.None;
        if (!leftSelected)
        {
            IterateWeapons(true);
            if (rightSelected)
            {
                startButton.Select();
            }
        } else if (!rightSelected)
        {
            IterateWeapons(false);
            startButton.Select();
        }
    }

    #endregion

    #region Interface (Private)

    private void UpdateWeaponsStatus()
    {
        SetWeaponStatus(ModType.Blaster, SaveState.Instance.GetBool(Strings.PlayerPrefStrings.BLASTER_ENABLED, true));
        SetWeaponStatus(ModType.Boomerang, SaveState.Instance.GetBool(Strings.PlayerPrefStrings.BOOMERANG_ENABLED, false));
        SetWeaponStatus(ModType.Dice, SaveState.Instance.GetBool(Strings.PlayerPrefStrings.DICE_GUN_ENABLED, false));
        SetWeaponStatus(ModType.Geyser, SaveState.Instance.GetBool(Strings.PlayerPrefStrings.GEYSER_GUN_ENABLED, false));
        SetWeaponStatus(ModType.LightningGun, SaveState.Instance.GetBool(Strings.PlayerPrefStrings.LIGHTNING_GUN_ENABLED, false));
        SetWeaponStatus(ModType.SpreadGun, SaveState.Instance.GetBool(Strings.PlayerPrefStrings.SPREAD_GUN_ENABLED, false));
    }

    private void SetWeaponStatus(ModType modType, bool enabled)
    {
        foreach(ButtonBundle weapon in weapons)
        {
            if(weapon.type == modType)
            {
                weapon.button.interactable = enabled;
                break;
            }
        }
    }

    private void PollWeaponSelect ()
	{
		ButtonMode leftMode = InputManager.Instance.QueryAction (Strings.Input.Actions.FIRE_LEFT);
		ButtonMode rightMode = InputManager.Instance.QueryAction (Strings.Input.Actions.FIRE_RIGHT);

		// Fast-fail
		if (!(leftMode == ButtonMode.DOWN || rightMode == ButtonMode.DOWN))
			return;

        IterateWeapons(leftMode == ButtonMode.DOWN);
	}

    private void IterateWeapons (bool isLeftArm)
    {
        // Iterate through and identify selected "button"
        GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        foreach (ButtonBundle bundle in weapons)
        {
            // Check to see if this weapon is selected
            if (bundle.button.gameObject == selected)
            {
                SetSelectedWeapon(bundle.type, isLeftArm);
                UpdateDisplay();
            }
        }
    }

	private void SetSelectedWeapon (ModType type, bool isLeftArm)
	{
		if (isLeftArm) {
			types[0] = type;
		} else {
			types[1] = type;
		}
	}

	private void UpdateDisplay ()
	{
		foreach (ButtonBundle bundle in weapons) {
			bundle.leftHand.SetActive (bundle.type == types[0]);
			bundle.rightHand.SetActive (bundle.type == types[1]);
		}

		bool canStart = (types[0] != ModType.None &&
		                types[1] != ModType.None);
		startButton.interactable = canStart;
	}

    private void BackButtonBehaviour ()
    {
        if (types[1] != ModType.None)
        {
            types[1] = ModType.None;
        } else if (types[0] != ModType.None)
        {
            types[0] = ModType.None;
        } else
        {
            BackAction();
        }
        
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == startButton.gameObject)
        {
            initialButton.Select();
        }
        UpdateDisplay();
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
