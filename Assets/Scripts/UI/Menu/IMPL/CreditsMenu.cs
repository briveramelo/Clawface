using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsMenu : Menu
{
    [SerializeField]
    GameObject mainMenuCanvasGameObject;

    CanvasGroup mainMenuCanvasGroup;
    GameObject mainDefaultSelected;
    public CreditsMenu() : base(Strings.MenuStrings.CREDITS)
    {
    }

    private void Awake()
    {
        mainMenuCanvasGroup = mainMenuCanvasGameObject.GetComponent<CanvasGroup>();
        mainDefaultSelected = EventSystem.current.firstSelectedGameObject;
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

    public void FireBack()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, ShowMain));
    }

    void ShowMain()
    {
        mainMenuCanvasGameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, mainMenuCanvasGroup, null));
        EventSystem.current.SetSelectedGameObject(mainDefaultSelected);
    }
}
