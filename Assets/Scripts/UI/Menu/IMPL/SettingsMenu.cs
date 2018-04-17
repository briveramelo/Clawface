using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu {

    #region Accessors (Menu)

    public override Selectable InitialSelection
    {
        get
        {
            return graphics;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    [Header("Camera Manipulation")]
    [SerializeField]
    private float offRotation = 0F;

    [SerializeField]
    private float onRotation = -10F;

    [Header("Panel Buttons")]
    [SerializeField]
    private Button graphics = null;

    [SerializeField]
    private new Button audio = null;

    [SerializeField]
    private Button controls = null;

    [SerializeField]
    private Button other = null;

    [Header("Action Buttons")]
    [SerializeField]
    private Button back = null;

    [SerializeField]
    private Button _default = null;

    [Header("Settings Objects - Graphics")]
    [SerializeField]
    private TextSlider quality = null;

    [SerializeField]
    private TextSlider resolution = null;

    [SerializeField]
    private TextSlider goreDetail = null;

    [SerializeField]
    private Toggle fullscreen = null;

    [Header("Settings Objects - Audio")]
    [SerializeField]
    private Slider music = null;

    [SerializeField]
    private Slider ui = null;

    [Header("Settings Objects - Controls")]
    [SerializeField]
    private TextSlider fireMode = null;

    [SerializeField]
    private TextSlider mouseAimMode = null;

    [SerializeField]
    private Toggle snapLook = null;

    [SerializeField]
    private Toggle vibration = null;

    [SerializeField]
    private Toggle cursorLock = null;

    [Header("Settings Objects - GamePlay")]
    [SerializeField]
    private TextSlider difficulty = null;

    [SerializeField]
    private Toggle tutorial = null;

    [SerializeField]
    private SelectorToggleGroup selectorToggleGroup = null;
    #endregion

    #region Fields (Internal)

    internal bool shouldInitialize = true;

    #endregion

    #region Fields (Private)
    private int selectedFilterToggle;
    private int SelectedFilterToggle {
        get { return Mathf.Clamp(selectedFilterToggle, 0, selectorToggleGroup.SelectorTogglesCount); }
        set { selectedFilterToggle = (int)Mathf.Repeat(value, selectorToggleGroup.SelectorTogglesCount); }
    }
    #endregion

    #region Constructors (Public)

    public SettingsMenu() : base(Strings.MenuStrings.SETTINGS) {}

    #endregion
    
    #region Unity Lifecycle

    private void Update() {
        if (allowInput) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN)) {
                ButtonBack();
            }
            CheckToMoveFilter();
        }
    }

    #endregion

    #region Interface (Public)

    public void RewireForGraphics()
    {
        LinkPanelButtonsTo(quality);
        LinkActionButtonsTo(fullscreen);
    }

    public void RewireForAudio()
    {
        LinkPanelButtonsTo(music);
        LinkActionButtonsTo(ui);
    }

    public void RewireForControls()
    {
        LinkPanelButtonsTo(fireMode);
        LinkActionButtonsTo(cursorLock);
    }

    public void RewireForGamePlay()
    {
        LinkPanelButtonsTo(difficulty);
        LinkActionButtonsTo(tutorial);
    }

    public void ButtonBack()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });

        // Save OnExit Settings
        TransferOnExitSettingsToManager();
        SettingsManager.Instance.ApplyChanges();

        // Save ControlMapper Data
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONTROLS);
        ControlMapperMenu cMenu = menu as ControlMapperMenu;
        if (cMenu != null)
        {
            cMenu.UserDataStore.Save();
        }
    }

    public void ButtonDefault()
    {
        SettingsManager.Instance.SetDefault();
        TransferSettingsFromManager();
    }

    public void ButtonRemap()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.CONTROLS, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void CallbackRealtimeSettingValueChanged()
    {
        TransferRealtimeSettingsToManager();
        SettingsManager.Instance.ApplyChanges();
    }

    #endregion

    #region Interface (Protected)
    public override MenuType ThisMenuType { get { return MenuType.Settings; } }
    protected override void ShowStarted()
    {
        base.ShowStarted();
        selectorToggleGroup.HandleGroupSelection(0);
        RewireForGraphics();
        TransferSettingsFromManager();
        StartCoroutine(RotateCamera(offRotation, onRotation));
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        StartCoroutine(RotateCamera(onRotation, offRotation));
    }

    #endregion

    #region Interface (Private)

    private void CheckToMoveFilter()
    {
        bool leftButtonPressed = InputManager.Instance.QueryAction(Strings.Input.UI.TAB_LEFT, ButtonMode.DOWN);
        bool rightBumperPressed = InputManager.Instance.QueryAction(Strings.Input.UI.TAB_RIGHT, ButtonMode.DOWN);
        bool mouseClicked = Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT) || Input.GetMouseButtonDown(MouseButtons.MIDDLE);
        if (!mouseClicked && (leftButtonPressed || rightBumperPressed))
        {
            if (leftButtonPressed)
            {
                SelectedFilterToggle--;
            }
            else
            {
                SelectedFilterToggle++;
            }
            CurrentEventSystem.SetSelectedGameObject(RewireSubMenu().gameObject);
        }
    }

    private Selectable RewireSubMenu()
    {
        switch ((SettingsMenuSubType)SelectedFilterToggle)
        {
            default:
            case SettingsMenuSubType.Graphics: graphics.onClick.Invoke(); return graphics;
            case SettingsMenuSubType.Audio: audio.onClick.Invoke(); return audio;
            case SettingsMenuSubType.Controls: controls.onClick.Invoke(); return controls;
            case SettingsMenuSubType.Gameplay: other.onClick.Invoke(); return other;
        }
    }

    private void LinkPanelButtonsTo(Selectable item)
    {
        foreach (Selectable selectable in new Selectable[] { graphics, audio, controls, other })
        {
            Navigation nav = selectable.navigation;
            nav.selectOnDown = item;
            selectable.navigation = nav;
        }
    }

    private void LinkActionButtonsTo(Selectable item)
    {
        foreach (Selectable selectable in new Selectable[] { back, _default })
        {
            Navigation nav = selectable.navigation;
            nav.selectOnUp = item;
            selectable.navigation = nav;
        }
    }

    private void TransferSettingsFromManager()
    {
        if (!shouldInitialize)
        {
            shouldInitialize = true;
            return;
        }

        graphics.onClick.Invoke();
        SettingsManager manager = SettingsManager.Instance;

        // Graphics
        quality.DataSource.ForceUpdate();
        resolution.DataSource.ForceUpdate();
        goreDetail.DataSource.ForceUpdate();
        fullscreen.isOn = manager.FullScreen;

        // Audio
        music.value = manager.MusicVolume;
        ui.value = manager.SFXVolume;

        // Controls
        fireMode.DataSource.ForceUpdate();
        mouseAimMode.DataSource.ForceUpdate();
        snapLook.isOn = manager.SnapLook;
        vibration.isOn = manager.Vibration;
        cursorLock.isOn = manager.CursorLock;

        // GamePlay
        difficulty.DataSource.ForceUpdate();
        tutorial.isOn = manager.Tutorial;
    }

    private void TransferRealtimeSettingsToManager()
    {
        SettingsManager manager = SettingsManager.Instance;

        // Audio
        manager.MusicVolume = music.value;
        manager.SFXVolume = ui.value;
    }

    private void TransferOnExitSettingsToManager()
    {
        SettingsManager manager = SettingsManager.Instance;

        // Graphics
        manager.QualityLevel = (int)quality.DataSource.Value;
        manager.Resolution = (Resolution)resolution.DataSource.Value;
        manager.GoreDetail = (int)goreDetail.DataSource.Value;
        manager.FullScreen = fullscreen.isOn;

        // Controls
        manager.FireMode = (FireMode)fireMode.DataSource.Value;
        manager.MouseAimMode = (MouseAimMode)mouseAimMode.DataSource.Value;
        manager.SnapLook = snapLook.isOn;
        manager.Vibration = vibration.isOn;
        manager.CursorLock = cursorLock.isOn;

        // GamePlay
        manager.Difficulty = (Difficulty)difficulty.DataSource.Value;
        manager.Tutorial = tutorial.isOn;
    }

    private IEnumerator RotateCamera(float start, float end)
    {
        Transform camera = Camera.main.transform;
        Vector3 rotation = camera.localEulerAngles;

        float elapsed = 0F;
        while (elapsed < FaderDuration)
        {
            rotation.y = Mathf.Lerp(start, end, elapsed / FaderDuration);
            camera.localEulerAngles = rotation;
            yield return null;
            elapsed += Time.deltaTime;
        }

        rotation.y = end;
        camera.localEulerAngles = rotation;
    }

    #endregion
}

public enum SettingsMenuSubType {
    Graphics=0,
    Audio,
    Controls,
    Gameplay,
}
