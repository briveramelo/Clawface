using System;
using ModMan;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectMenu : Menu
{
	#region Accessors (Menu)

	public override Button InitialSelection {
		get {
			return null;
		}
	}

	#endregion

	#region Fields (Unity Serialization)

	[SerializeField]
	private Button startButton = null;

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
    private bool wasLeft;
    private bool wasRight;

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

        //for dealing with the custom menu selection flow
        selectingPlayerLeft = false;
        selectingPlayerRight = true;
        startButton.image.sprite = unselectedButtonSprite;
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
        string level = Strings.Scenes.Arena;
        Debug.LogWarning("Target level is currently using String.Scenes constant.  Fix later when adding LevelSelectMenu back.");

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

    private void HandleSelectionFlow()
    {
        Vector2 axesState = InputManager.Instance.QueryAxes(Strings.Input.Axes.MOVEMENT);
        //Vector2 horizontal = InputManager.Instance.QueryAxes(Strings.Input.UI.HORIZONTAL);
        bool isLeft = axesState.x.AboutEqual(-1);
        bool isRight= axesState.x.AboutEqual(1);
        

        if (selectingPlayerRight)
        {
            HandleMovement(rightArm, isLeft, isRight);
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
            HandleMovement(leftArm, isLeft, isRight);
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
                startButton.image.sprite = pressedButtonSprite;
                StartAction();
            }

            //if you want to "back" out to the second arm selection

            else if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                startButton.image.sprite = unselectedButtonSprite;
                selectingPlayerLeft = true;
                leftArm.GlowControl.Reset();
                leftArm.ResetArrows();
            }
        }

        wasLeft = isLeft;
        wasRight = isRight;
    }

    void HandleMovement(WeaponLineup lineup, bool isLeft, bool isRight)
    {
        bool moved = false;
        if ((isLeft || isRight) && !(wasRight || wasLeft))
        {
            moved = MoveLineup(lineup, isLeft, isRight);
            if (moved)
            {
                queryActionTimer = queryActionEverySeconds;
            }
        }

        queryActionTimer -= Time.fixedDeltaTime;
        if (queryActionTimer < 0)
        {
            
            MoveLineup(lineup, isLeft, isRight);
            queryActionTimer = queryActionEverySeconds;
        }
    }

    bool MoveLineup(WeaponLineup lineup, bool isLeft, bool isRight)
    {
        if (isLeft)
        {
            //go left
            lineup.MoveLeft();
            return true;
        }
        else if (isRight)
        {
            lineup.MoveRight();
            return true;
        }
        return false;
    }

    private void LockInRightAction()
    {
        ModManager.rightArmOnLoad = rightArm.GetSelection();
        selectingPlayerRight = false;
        selectingPlayerLeft = true;
    }

    private void LockInLeftAction()
    {
        startButton.image.sprite = selectedButtonSprite;
        ModManager.leftArmOnLoad = leftArm.GetSelection();
        selectingPlayerLeft = false;
    }

    #endregion
}
