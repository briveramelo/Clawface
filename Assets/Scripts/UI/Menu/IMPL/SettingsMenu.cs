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

    [SerializeField]
    private Button apply = null;

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

    [Header("Settings Objects - GamePlay")]
    [SerializeField]
    private TextSlider difficulty = null;

    [SerializeField]
    private Toggle tutorial = null;

    #endregion

    #region Fields (Internal)

    internal bool shouldInitialize = true;

    #endregion

    #region Fields (Private)
    private bool allowInput;
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
        LinkActionButtonsTo(snapLook);
    }

    public void RewireForGamePlay()
    {
        LinkPanelButtonsTo(difficulty);
        LinkActionButtonsTo(tutorial);
    }

    public void ButtonBack()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });

        // Load ControlMapper Data
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONTROLS);
        ControlMapperMenu cMenu = menu as ControlMapperMenu;
        cMenu.UserDataStore.Load();
    }

    public void ButtonDefault()
    {
        SettingsManager.Instance.SetDefault();
        TransferSettingsFromManager();
    }

    public void ButtonApply()
    {
        TransferSettingsToManager();
        SettingsManager.Instance.ApplyChanges();

        // Apply ControlMapper Data
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONTROLS);
        ControlMapperMenu cMenu = menu as ControlMapperMenu;
        cMenu.UserDataStore.Save();
    }

    public void ButtonRemap()
    {
        MenuManager.Instance.DoTransition(Strings.MenuStrings.CONTROLS, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    #endregion

    #region Interface (Protected)

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void ShowStarted()
    {
        base.ShowStarted();
        allowInput = true;
        TransferSettingsFromManager();
        StartCoroutine(RotateCamera(offRotation, onRotation));
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        allowInput = false;
        StartCoroutine(RotateCamera(onRotation, offRotation));
    }

    #endregion

    #region Interface (Private)

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
        foreach (Selectable selectable in new Selectable[] { back, _default, apply })
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

        // GamePlay
        difficulty.DataSource.ForceUpdate();
        tutorial.isOn = manager.Tutorial;
    }

    private void TransferSettingsToManager()
    {
        SettingsManager manager = SettingsManager.Instance;

        // Graphics
        manager.QualityLevel = (int)quality.DataSource.Value;
        manager.Resolution = (Resolution)resolution.DataSource.Value;
        manager.GoreDetail = (int)goreDetail.DataSource.Value;
        manager.FullScreen = fullscreen.isOn;

        // Audio
        manager.MusicVolume = music.value;
        manager.SFXVolume = ui.value;

        // Controls
        manager.FireMode = (FireMode)fireMode.DataSource.Value;
        manager.MouseAimMode = (MouseAimMode)mouseAimMode.DataSource.Value;
        manager.SnapLook = snapLook.isOn;

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
