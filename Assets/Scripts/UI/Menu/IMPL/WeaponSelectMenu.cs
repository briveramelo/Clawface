using System;
using ModMan;
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
    private WeaponLineup leftArm;

    [SerializeField]
    private WeaponLineup rightArm;

    [SerializeField]
    private Camera menuCamera;

    [SerializeField]
    private CanvasFader fader;

    [SerializeField]
    private float fadeDuration = 0.25F;

    [SerializeField] private Sprite unselectedButtonSprite;
    [SerializeField] private Sprite selectedButtonSprite;
    [SerializeField] private Sprite pressedButtonSprite;
    

    private bool selectingPlayerRight = false;
    private bool selectingPlayerLeft = false;

    private float queryActionEverySeconds = 1.0f;
    private float queryActionTimer = 0.0f;


    #endregion

    #region Fields (Internal)

    internal string menuTarget = null;

    #endregion

    #region Fields (Private)

    bool inputGuard = false;
    private GameObject previousGameObject;
    private Camera previousCamera;

    #endregion

    #region Constructors (Public)

    public WeaponSelectMenu () : base (Strings.MenuStrings.WEAPON_SELECT)
	{
	}

    #endregion

    #region Interface (Unity Lifecycle)
    
    private void Update ()
	{
	    if (inputGuard)
        {
            HandleSelectionFlow();
	    }

	}



    #endregion

    #region Interface (Menu)

    protected override void Fade(Transition transition, Effect[] effects)
    {
        fader.DoShow(fadeDuration, () =>
        {
            base.Fade(transition, effects);
        });
    }


    protected override void DefaultHide (Transition transition, Effect[] effects)
	{
		Fade (transition, effects);
	}

	protected override void DefaultShow (Transition transition, Effect[] effects)
	{
		Fade (transition, effects);
	}

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
        previousCamera = Camera.main;
        previousGameObject = previousCamera.gameObject;
        previousCamera.enabled = false;
        menuCamera.enabled = true;
        fader.DoHide(fadeDuration, null);

        selectingPlayerLeft = false;
        selectingPlayerRight = true;
        initialButton.enabled = false;
        queryActionTimer = queryActionEverySeconds;
        leftArm.GlowControl.SetUnselected();
        rightArm.GlowControl.Reset();
        leftArm.ResetArrows();
        rightArm.ResetArrows();
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
    }

    protected override void HideComplete()
    {
        base.HideComplete();
        menuCamera.enabled = false;
        if (!previousGameObject.IsDestroyed())
        {
            previousCamera.enabled = true;
        }
        previousCamera = null;
    }

    #endregion

    #region Interface (Public)

    public void BackAction ()
	{
        fader.DoShow(fadeDuration, () => {
		    MenuManager.Instance.DoTransition (menuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        });
    }

	public void StartAction ()
	{
	    ModManager.assignFromPool = false;

		// Acquire target level.
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LEVEL_SELECT);
        string level = "Arena 1";
        Debug.LogWarning("Target level is currently hardcoded.  Fix later when adding LevelSelectMenu back.");

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

    #endregion

    #region Interface (Private)

    private void BackButtonBehaviour () {
        BackAction();
    }

    private void LockInRightAction()
    {
        ModManager.rightArmOnLoad = rightArm.GetSelection();
        selectingPlayerRight = false;
        selectingPlayerLeft = true;
    }

    private void LockInLeftAction()
    {
        initialButton.image.sprite = selectedButtonSprite;
        ModManager.leftArmOnLoad = leftArm.GetSelection();
        selectingPlayerLeft = false;
    }

    private void HandleSelectionFlow()
    {
        if (selectingPlayerRight)
        {
            queryActionTimer -= Time.fixedDeltaTime;
            if (queryActionTimer < 0)
            {
                Vector2 axesState = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);

                if (axesState.x == -1)
                {
                    //go left
                    rightArm.MoveLeft();

                }
                else if (axesState.x == 1)
                {
                    rightArm.MoveRight();
                }

                queryActionTimer = queryActionEverySeconds;
            }
            if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN))
            {
                LockInRightAction();
                leftArm.GlowControl.Reset();
            }
            else if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                BackButtonBehaviour();
            }
        }
        else if (selectingPlayerLeft)
        {
            queryActionTimer -= Time.fixedDeltaTime;
            if (queryActionTimer < 0)
            {
                Vector2 axesState = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);

                if (axesState.x == -1)
                {
                    //go left
                    leftArm.MoveLeft();

                }
                else if (axesState.x == 1)
                {
                    leftArm.MoveRight();
                }

                queryActionTimer = queryActionEverySeconds;
            }
            if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN))
            {
                LockInLeftAction();
            }
            else if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                
                selectingPlayerRight = true;
                selectingPlayerLeft = false;
                leftArm.GlowControl.SetUnselected();
                rightArm.GlowControl.Reset();
                leftArm.ResetArrows();
            }
        }
        else if (!selectingPlayerRight && !selectingPlayerLeft)
        {
            //check to see if confirm
            if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN))
            {
                InitialSelection.image.sprite = pressedButtonSprite;
                StartAction();
            }

            //if you want to "back" out to the second arm selection

            else if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                InitialSelection.image.sprite = unselectedButtonSprite;
                selectingPlayerLeft = true;
                leftArm.GlowControl.Reset();
                leftArm.ResetArrows();
            }
        }
    }

    #endregion
}
