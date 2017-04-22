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

    bool menuShowing = false;
    
    CanvasGroup creditsCanvasGroup;

    [SerializeField]
    GameObject creditsCanvasGameObject;

    [SerializeField]
    GameObject creditsDefaultSelected;

    [SerializeField]
    GameObject fadeCanvasGameObject;

    [SerializeField]
    Button startButton;

    [SerializeField]
    VideoPlayer projector;

    [SerializeField]
    VideoClip staticLoop;

    CanvasGroup fadeCanvasGroup;
  

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
            DoTransition(Transition.SHOW, new Effect[] { });
            menuShowing = true;
        }
    }

    private void Awake()
    {
        creditsCanvasGroup = creditsCanvasGameObject.GetComponent<CanvasGroup>();
        fadeCanvasGroup = fadeCanvasGameObject.GetComponent<CanvasGroup>();
    }


    private void Start()
    {
        creditsCanvasGameObject.SetActive(false);

       
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            ShowMenu();
        }
    }


    public void PlayMenuMusic()
    {
        MusicManager.Instance.PlayMusic(MusicType.MainMenu_Track, gameObject.transform.position);
    }
    
    public void KillVideo()
    {
        projector.gameObject.SetActive(false);
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
                    () => { displayed = true; startButton.Select(); }));
                break;
        }
    }

    public void PlayUIEnter()
    {
        SFXManager.Instance.Play(SFXType.UI_Hover, gameObject.transform.position);
    }

    public void PlayUIConfirm()
    {
        SFXManager.Instance.Play(SFXType.UI_Click, gameObject.transform.position);
    }

    public void PlayUIBack()
    {
        SFXManager.Instance.Play(SFXType.UI_Back, gameObject.transform.position);

    }
    public void StartGame()
    {
        //fade out self
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, FadeOut));
    }

    public void FireCredits()
    {
        
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, ShowCredits));
        //MenuManager.Instance.DoTransition(Strings.MenuStrings.MAIN, Transition.HIDE, new Effect[] { });
       
    }
    
    

    void FadeOut()
    {
        //fade out to black
        fadeCanvasGameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, fadeCanvasGroup, LoadLevelOne));
    }
    void LoadLevelOne()
    {
        Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
        LoadMenu loadMenu = (LoadMenu) menu;
        loadMenu.TargetScene = "Scenes/Gucci_V1.1";
        MenuManager.Instance.DoTransition(loadMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void ShowCredits()
    {
        creditsCanvasGameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, creditsCanvasGroup, HideSelf));
    }

    void HideSelf()
    {
        EventSystem.current.SetSelectedGameObject(creditsDefaultSelected);
        gameObject.SetActive(false);
    }
    
}
