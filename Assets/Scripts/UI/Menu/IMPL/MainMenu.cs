using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenu : Menu
{

    bool menuShowing = false;
    
    CanvasGroup creditsCanvasGroup;

    [SerializeField]
    GameObject creditsCanvasGameObject;

    [SerializeField]
    GameObject creditsDefaultSelected;

    [SerializeField]
    GameObject fadeCanvasGameObject;

    CanvasGroup fadeCanvasGroup;

    private void Update()
    {
        if (Input.anyKey && !menuShowing)
        {
            menuShowing = true;
            DoTransition(Transition.SHOW, new Effect[] { });
        }
    }
    public MainMenu() : base(Strings.MenuStrings.MAIN)
    {
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

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.HIDE:
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, null));
                break;
            case Transition.SHOW:
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup, null));
                break;
        }
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
        SceneManager.LoadScene(1);
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
