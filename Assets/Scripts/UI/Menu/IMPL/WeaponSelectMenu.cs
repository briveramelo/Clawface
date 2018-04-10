//Garin

using ModMan;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Turing.VFX;

public class WeaponSelectMenu : Menu
{

    public bool isReset;
    public bool isLeftBack;
    public bool isBothBack;


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

	[SerializeField] private Button startButton = null;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private CanvasFader fader;
    [SerializeField] private Animator leftArmAnimator;
    [SerializeField] private Animator rightArmAnimator;
    [SerializeField] private PlayerFaceController playerFaceController;
    [SerializeField] private WeaponLineup leftArm;
    [SerializeField] private WeaponLineup rightArm;
    [SerializeField] private Text weaponNameText;
    [SerializeField] private Image weaponGraphic;
    [SerializeField] private Image damageBar;
    [SerializeField] private Image rangeBar;
    [SerializeField] private Image rofBar;
    [SerializeField] private Image difficultyBar;
    [SerializeField] private Sprite unselectedButtonSprite;
    [SerializeField] private Sprite selectedButtonSprite;
    [SerializeField] private Sprite pressedButtonSprite;
    [SerializeField] private List<WeaponInfo> weaponInfos;

    [SerializeField] private float fadeDuration = 0.25F;
    [SerializeField] private float queryActionEverySeconds = .75f;

    #endregion

    #region Fields (Public)
    private bool UseLoadMenu { get { return forwardMenuTarget == Strings.MenuStrings.LOAD;} }
    private Action onCompleteSceneLoad;
    private Action onStartAction;
    private Action onReturnFromPLE;
    #endregion

    #region Fields (Internal)

    internal string backMenuTarget = null;
    internal string forwardMenuTarget = null;

    #endregion

    #region Fields (Private)

    private bool BothArmsSelected { get { return !selectingPlayerRight && !selectingPlayerLeft; } }
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
        if (allowInput) {
            HandleSelectionFlow();

            if (isLeftBack) {
                isLeftBack = false;
                GoBackFromLeft();
            }
            if (isBothBack) {
                isBothBack = false;
                GoBackFromBothSelected();
            }
            if (isReset) {
                isReset = false;
                ResetMenu();
            }
	    }

	}

    #endregion

    #region Interface (Menu)
    public override MenuType ThisMenuType { get { return MenuType.WeaponSelect; } }
    protected override void Fade(Transition transition, Effect[] effects)
    {
        fader.DoShow(fadeDuration, () =>
        {
            base.Fade(transition, effects);
        });
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        ResetMenu();
        previousCamera = Camera.main;
        previousGameObject = previousCamera.gameObject;
    }

    protected override void ShowComplete()
    {
        base.ShowComplete();
        playerFaceController.SetTemporaryEmotion(PlayerFaceController.Emotion.Happy, .3f);
        previousCamera.enabled = false;
        menuCamera.enabled = true;
        fader.DoHide(fadeDuration, null);
    }

    protected override void HideStarted()
    {
        base.HideStarted();
    }

    protected override void HideComplete()
    {
        base.HideComplete();
        menuCamera.enabled = false;
        if (previousCamera){
            previousCamera.enabled = true;
        }
        previousCamera = null;
    }

    #endregion

    #region Interface (Public)

    public void BackAction ()
	{
        if (backMenuTarget != null) {
            fader.DoShow(fadeDuration, () => {
                MenuManager.Instance.DoTransition(backMenuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
            });
        }
        if(onReturnFromPLE!=null) {
            onReturnFromPLE();
        }
    }
    public void DefineNavigation(string backMenuTarget, string forwardMenuTarget, Action onStartAction=null, Action onCompleteSceneLoad=null, Action onReturnFromPLE=null) {
        this.backMenuTarget = backMenuTarget;
        this.forwardMenuTarget = forwardMenuTarget;
        this.onStartAction = onStartAction;
        this.onCompleteSceneLoad = onCompleteSceneLoad;
        this.onReturnFromPLE = onReturnFromPLE;
    }

	public void StartAction ()
	{
        playerFaceController.SetTemporaryEmotion(PlayerFaceController.Emotion.Happy, .3f);

        //Set up ModManager
        ModManager.assignFromPool = false;
        ModManager.rightArmOnLoad = rightArm.GetModType;
        ModManager.leftArmOnLoad = leftArm.GetModType;                

        // Acquire LoadMenu and set target.
        string targetSceneName = SceneTracker.CurrentSceneName;
        Menu menu = MenuManager.Instance.GetMenuByName(forwardMenuTarget);
        if (forwardMenuTarget != null) {
            MenuManager.Instance.DoTransition(menu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
        else {
            MenuManager.Instance.ClearMenus();
        }

        if (UseLoadMenu) {
            LoadMenu loadMenu = (LoadMenu)menu;
            targetSceneName = loadMenu.TargetSceneName;
            Action onLoadMenuCompleteSceneLoad = () => {                
                if (SceneTracker.IsSceneArena(targetSceneName)) {
                    PauseMenu pauseMenu = (PauseMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
                    pauseMenu.CanPause = true;
                    EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, SceneTracker.CurrentSceneName, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());
                }
                if (onCompleteSceneLoad!=null) {
                    onCompleteSceneLoad();
                }
            };
            loadMenu.SetNavigation(targetSceneName, onLoadMenuCompleteSceneLoad);
        }
        else if (SceneTracker.IsSceneArena(targetSceneName)) {
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_STARTED, SceneTracker.CurrentSceneName, ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString());
        }
        
        if (onStartAction!=null) {
            onStartAction();
        }
	}

    #endregion

    #region Interface (Private)

    private void ResetMenu()
    {
        queryActionTimer = queryActionEverySeconds;

        ChangeWeaponTextPanel(rightArm);

        GoBackFromLeft();
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

        bool isSubmitDown = InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN);
        bool isCancelDown = InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN);

        if (selectingPlayerRight)
        {
            HandleMovingWeaponLineup(rightArm, isLeft, isRight);
            if (isSubmitDown)
            {
                LockInRightAction();                
            }
            else if (isCancelDown)
            {
                BackButtonBehaviour();
            }
        }
        else if (selectingPlayerLeft)
        {
            HandleMovingWeaponLineup(leftArm, isLeft, isRight);
            if (isSubmitDown)
            {
                LockInLeftAction();
            }
            else if (isCancelDown)
            {
                GoBackFromLeft();
            }
        }
        else if (BothArmsSelected)
        {
            if (isSubmitDown)
            {
                StartAction();
            }
            else if (isCancelDown)
            {
                GoBackFromBothSelected();
            }
        }

        wasLeft = isLeft;
        wasRight = isRight;
    }

    private void GoBackFromLeft() {
        selectingPlayerLeft = false;
        selectingPlayerRight = true;

        leftArm.UnselectArrows();
        rightArm.SelectArrows();
        rightArm.GlowControl.SetState(WeaponSelectState.Highlighted);
        leftArm.GlowControl.SetState(WeaponSelectState.Unselected);
        rightArmAnimator.SetTrigger("DoUnlock");
    }

    private void GoBackFromBothSelected() {
        selectingPlayerLeft = true;
        selectingPlayerRight = false;

        leftArm.SelectArrows();
        rightArm.UnselectArrows();
        leftArm.GlowControl.SetState(WeaponSelectState.Highlighted);
        ChangeWeaponTextPanel(leftArm);
        leftArmAnimator.SetTrigger("DoUnlock");
        CurrentEventSystem.SetSelectedGameObject(null);
    }

    private void HandleMovingWeaponLineup(WeaponLineup lineup, bool isLeft, bool isRight)
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
        ModManager.rightArmOnLoad = rightArm.GetModType;
        selectingPlayerRight = false;
        selectingPlayerLeft = true;
        rightArmAnimator.SetTrigger("DoLock");
        ChangeWeaponTextPanel(leftArm);

        leftArm.SelectArrows();
        rightArm.UnselectArrows();
        leftArm.GlowControl.SetState(WeaponSelectState.Highlighted);
        rightArm.GlowControl.SetState(WeaponSelectState.Confirmed);
        playerFaceController.SetTemporaryEmotion(PlayerFaceController.Emotion.Happy, .4f);
        SFXManager.Instance.Play(SFXType.LandmarkBlastShort, transform.position);
    }

    private void LockInLeftAction()
    {
        ModManager.leftArmOnLoad = leftArm.GetModType;
        selectingPlayerRight = false;
        selectingPlayerLeft = false;
        leftArm.UnselectArrows();
        leftArm.GlowControl.SetState(WeaponSelectState.Confirmed);        
        leftArmAnimator.SetTrigger("DoLock");
        playerFaceController.SetTemporaryEmotion(PlayerFaceController.Emotion.Happy, .4f);
        SFXManager.Instance.Play(SFXType.LandmarkBlastShort, transform.position);

        startButton.Select();
        CurrentEventSystem.SetSelectedGameObject(startButton.gameObject);
    }

    public void ChangeWeaponTextPanel(WeaponLineup lineup)
    {
        ModType type = lineup.GetModType;

        foreach (WeaponInfo info in weaponInfos)
        {
            if (info.weaponType == type)
            {
                weaponNameText.text = info.weaponName;
                weaponGraphic.sprite = info.weaponImage;


                LeanTween.value(damageBar.fillAmount, info.damageValue, 0.3f).setOnUpdate((float val) => { damageBar.fillAmount = val; }) ;
                LeanTween.value(rangeBar.fillAmount, info.rangeValue, 0.3f).setOnUpdate((float val) => { rangeBar.fillAmount = val; });
                LeanTween.value(rofBar.fillAmount, info.rofValue, 0.3f).setOnUpdate((float val) => { rofBar.fillAmount = val; });
                LeanTween.value(difficultyBar.fillAmount, info.difficultyValue, 0.3f).setOnUpdate((float val) => { difficultyBar.fillAmount = val; });
                return;
            }
        }
    }

    #endregion
}
