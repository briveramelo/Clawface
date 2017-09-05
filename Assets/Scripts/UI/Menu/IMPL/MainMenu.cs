using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : Menu
{

    #region Public Fields
    public override bool Displayed
    {
        get
        {
            return Displayed;
        }
    }
    #endregion

    #region Serialized Unity Fields
    private bool menuShowing = false;

    [SerializeField]
    CanvasGroup creditsCanvasGroup;

    [SerializeField]
    CanvasGroup fadeCanvasGroup;

    [SerializeField]
    Button creditsDefaultSelectedButton;

    [SerializeField]
    Button mainDefaultSelectedButton;

    [SerializeField]
    GameObject bloodAndGore;

    [SerializeField]
    GameObject miloTheRobot;
    
    [SerializeField]
    VideoPlayer projector;

    [SerializeField]
    VideoClip staticLoop;

    [SerializeField]
    CameraTrack track;

    #endregion

    #region Private Fields
    private bool displayed = false;
    private bool disableTrackFade = false;
    #endregion

    #region Unity Lifecycle Methods

    protected override void Start()
    {
        creditsCanvasGroup.gameObject.SetActive(false);
        MenuManager.Instance.EnableEventSystem(false);


    }

    private void Update()
    {
        if (Input.anyKey && !menuShowing)
        {
            menuShowing = true;
            SkipToMenuHide();
            disableTrackFade = true;
        }

    }


    #endregion

    #region Public Interface

    public void KillScreen()
    {
        projector.clip = staticLoop;
        projector.isLooping = true;
    }

    public void SkipToMenuHide()
    {
        if (disableTrackFade)
        {
            disableTrackFade = false;
            return;
        }
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, fadeCanvasGroup, OpenEyes));
    }

    public void SkipToMenuShow()
    {
        track.JumpToPosition(4);
        KillScreen();
        ShowMenu();
    }

    public void CloseEyes()
    {
        //if(MathfadeCanvasGroup.alpha)
        if (!Mathf.Equals(1.0f, fadeCanvasGroup.alpha))
        {
            StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 0.25f, fadeCanvasGroup, SkipToMenuShow));
        }
        else
        {
            SkipToMenuShow();
        }
    }

    public void OpenEyes()
    {
        SkipToMenuShow();
        bloodAndGore.SetActive(true);
        miloTheRobot.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, fadeCanvasGroup, null));
    }
    public void ShowCredits()
    {
        creditsCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, creditsCanvasGroup, HideSelf));
    }

    public void PlayMenuMusic()
    {
        MusicManager.Instance.PlayMusic(MusicType.MainMenu_Track, gameObject.transform.position);
    }

    public override void DoTransition(Transition transition, Effect[] effects)
    {

        switch (transition)
        {
            case Transition.HIDE:
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup,
                    () => { displayed = false; }));
                break;
            case Transition.SHOW:
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 2.0f, canvasGroup,
                    () => { displayed = true; /*startButton.Select();*/ }));
                break;
        }
    }

    public void StartArena()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, FadeOutToArena));
    }

    public void FireCredits()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, ShowCredits));
    }

    public void ShowMenu()
    {
        
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup, EnableES));
        
    }


    #endregion

    #region Private Interface
    private void EnableES()
    {
        MenuManager.Instance.EnableEventSystem(true);
        mainDefaultSelectedButton.Select();
    }

    private void FadeOutToArena() {
        fadeCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, fadeCanvasGroup, LoadArena));
    }

    private void LoadArena()
    {
        MusicManager.Instance.Stop(MusicType.MainMenu_Track);

        Menu pMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        PauseMenu pauseMenu = (PauseMenu)pMenu;
        pauseMenu.CanPause = true;
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu)menu;
        loadMenu.TargetScene = Strings.Scenes.Arena;
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    private void HideSelf()
    {
        creditsDefaultSelectedButton.Select();
        gameObject.SetActive(false);
    }

    #endregion

    public MainMenu() : base(Strings.MenuStrings.MAIN)
    {
    }



    
}
