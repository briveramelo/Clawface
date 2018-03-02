//Garin

using ModMan;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponSelectMenu : Menu
{
	#region Accessors (Menu)

	public override Selectable InitialSelection
    {
		get
        {
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

    [SerializeField]
    private Sprite unselectedButtonSprite;

    [SerializeField]
    private Sprite selectedButtonSprite;

    [SerializeField]
    private Sprite pressedButtonSprite;

    [SerializeField]
    private float queryActionEverySeconds = .75f;

    [SerializeField]
    private Text weaponNameText;

    [SerializeField]
    private Text weaponDescriptionText;

    [SerializeField]
    private Image weaponGraph;

    [SerializeField]
    private List<WeaponInfo> weaponInfos;

    [SerializeField] Animator leftArmAnimator;
    [SerializeField] Animator rightArmAnimator;

    #endregion

    #region Fields (Internal)

    internal string backMenuTarget = null;
    internal string forwardMenuTarget = null;

    #endregion

    #region Fields (Private)

    private bool inputGuard = false;
    private GameObject previousGameObject;
    private Camera previousCamera;
    
    private bool selectingPlayerRight = false;
    private bool selectingPlayerLeft = false;

    private float queryActionTimer = 0.0f;
    private bool wasLeft;
    private bool wasRight;

    #endregion

    #region Constructors (Public)

    public WeaponSelectMenu () : base (Strings.MenuStrings.WEAPON_SELECT)
	{}

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

    protected override void ShowStarted() {
        base.ShowStarted();

        ResetMenu();

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
		    MenuManager.Instance.DoTransition (backMenuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        });
    }

	public void StartAction ()
	{
	    ModManager.assignFromPool = false;

		// Acquire Pause Menu
		Menu menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.PAUSE);
		PauseMenu pauseMenu = (PauseMenu)menu;
		pauseMenu.CanPause = true;

		// Acquire LoadMenu and set target.
		menu = MenuManager.Instance.GetMenuByName (Strings.MenuStrings.LOAD);
		LoadMenu loadMenu = (LoadMenu)menu;
		loadMenu.TargetScene = forwardMenuTarget;

        // Trigger level started event
        ModManager.rightArmOnLoad = rightArm.GetSelection();
        ModManager.leftArmOnLoad = leftArm.GetSelection();
        EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, loadMenu.TargetScene, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());

		// Make it happen.
		MenuManager.Instance.DoTransition (loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
	}

    #endregion

    #region Interface (Private)

    private void ResetMenu()
    {
        selectingPlayerLeft = false;
        selectingPlayerRight = true;

        startButton.image.sprite = unselectedButtonSprite;
        queryActionTimer = queryActionEverySeconds;

        leftArm.GlowControl.SetUnselected();
        rightArm.GlowControl.Reset();

        leftArm.ResetArrows();
        rightArm.ResetArrows();

        ChangeWeaponTextPanel(rightArm);
    }

    private void BackButtonBehaviour ()
    {
        BackAction();
    }    

    private void HandleSelectionFlow()
    {
        Vector2 navigation = InputManager.Instance.QueryAxes(Strings.Input.UI.NAVIGATION);
        bool isLeft = navigation.x.AboutEqual(-1);
        bool isRight = navigation.x.AboutEqual(1);

        if (selectingPlayerRight)
        {
            HandleMovement(rightArm, isLeft, isRight);
            if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN))
            {
                LockInRightAction();
                ChangeWeaponTextPanel(leftArm);
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
                ResetMenu();
                rightArmAnimator.SetTrigger("DoUnlock");
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
                ChangeWeaponTextPanel(leftArm);
                leftArmAnimator.SetTrigger("DoUnlock");
            }
        }

        wasLeft = isLeft;
        wasRight = isRight;
    }

    private void HandleMovement(WeaponLineup lineup, bool isLeft, bool isRight)
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

        if (isLeft || isRight) {
            queryActionTimer -= Time.deltaTime;
        }
        else {
            queryActionTimer = queryActionEverySeconds;
        }
        if (queryActionTimer < 0)
        {
            MoveLineup(lineup, isLeft, isRight);
            queryActionTimer = queryActionEverySeconds;
        }
    }

    private bool MoveLineup(WeaponLineup lineup, bool isLeft, bool isRight)
    {
        if (isLeft)
        {
            lineup.MoveLeft();
            ChangeWeaponTextPanel(lineup);
            return true;
        }
        else if (isRight)
        {
            lineup.MoveRight();
            ChangeWeaponTextPanel(lineup);
            return true;
        }
        return false;
    }

    private void LockInRightAction()
    {
        ModManager.rightArmOnLoad = rightArm.GetSelection();
        selectingPlayerRight = false;
        selectingPlayerLeft = true;
        rightArmAnimator.SetTrigger("DoLock");
    }

    private void LockInLeftAction()
    {
        startButton.image.sprite = selectedButtonSprite;
        ModManager.leftArmOnLoad = leftArm.GetSelection();
        selectingPlayerLeft = false;
        leftArmAnimator.SetTrigger("DoLock");
    }

    private void ChangeWeaponTextPanel(WeaponLineup lineup)
    {
        ModType type = lineup.GetSelection();

        foreach (WeaponInfo info in weaponInfos)
        {
            if (info.weaponType == type)
            {
                weaponNameText.text = info.weaponName;
                weaponDescriptionText.text = info.weaponDescription;
                weaponGraph.sprite = info.weaponGraph;
                return;
            }
        }
    }

    #endregion
}
