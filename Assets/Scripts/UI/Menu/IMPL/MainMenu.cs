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

    //[SerializeField]
    //Button startButton;

    [SerializeField]
    VideoPlayer projector;

    [SerializeField]
    VideoClip staticLoop;

    [SerializeField]
    CameraTrack track;
    
  

    #region Private Fields
    private bool displayed = false;
    #endregion


    
    public MainMenu() : base(Strings.MenuStrings.MAIN)
    {
    }



    public void ShowMenu()
    {
        
        if (!menuShowing)
        {
            menuShowing = true;
            StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup, EnableES));
            
        }
    }

    private void EnableES()
    {
        EventSystem.current.GetComponent<StandaloneInputModule>().ActivateModule();
        EventSystem.current.SetSelectedGameObject(mainDefaultSelectedButton.gameObject);
    }


    private void Start()
    {
        creditsCanvasGroup.gameObject.SetActive(false);
        EventSystem.current.GetComponent<StandaloneInputModule>().DeactivateModule();


    }

    private void Update()
    {
        if (Input.anyKey && !menuShowing)
        {
            SkipToMenuHide();
            //ShowMenu();
        }
        
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

 
    public void StartGame()
    {
        //fade out self
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, FadeOut));
        //DoTransition(Transition.HIDE, new Effect[] { });
    }

    public void FireCredits()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, ShowCredits));
    }
    
    

    void FadeOut()
    {
        ////fade out to black
        fadeCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, fadeCanvasGroup, LoadLevelOne));
    }
    void LoadLevelOne()
    {
        MusicManager.Instance.Stop(MusicType.MainMenu_Track);
        
        Menu pMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.PAUSE);
        PauseMenu pauseMenu = (PauseMenu)pMenu;
        pauseMenu.CanPause = true;
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu) menu;
        loadMenu.TargetScene = "Scenes/Gucci_V1.1";
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void CloseEyes()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 0.25f, fadeCanvasGroup,OpenEyes));
    }

    public void OpenEyes()
    {
        bloodAndGore.SetActive(true);
        miloTheRobot.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 0.25f, fadeCanvasGroup, null));
    }
    public void ShowCredits()
    {
        creditsCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, creditsCanvasGroup, HideSelf));
    }

    void HideSelf()
    {
        EventSystem.current.SetSelectedGameObject(creditsDefaultSelectedButton.gameObject);
        gameObject.SetActive(false);
    }

    public void KillScreen()
    {
        projector.clip = staticLoop;
        projector.isLooping = true;
    }

    public void SkipToMenuHide()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, fadeCanvasGroup, SkipToMenuShow));
    }

    public void SkipToMenuShow()
    {
        CloseEyes();
        track.JumpToPosition(3);
        ShowMenu();
        KillScreen();

        //StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, fadeCanvasGroup, null));
    }
    
}
