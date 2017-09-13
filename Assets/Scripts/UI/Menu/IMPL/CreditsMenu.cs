using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsMenu : Menu
{

    #region Public Fields
    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }

    public override Button InitialSelection // these menus probably won't use this
    {
        get
        {
            return null;
        }
    }
    #endregion

    [SerializeField]
    GameObject mainMenuCanvasGameObject;

    CanvasGroup mainMenuCanvasGroup;
    Button mainDefaultSelected;

    #region Private Fields
    private bool displayed = false;
    #endregion

    public CreditsMenu() : base(Strings.MenuStrings.CREDITS)
    {
    }

    private void Start()
    {
        mainMenuCanvasGroup = mainMenuCanvasGameObject.GetComponent<CanvasGroup>();
        mainDefaultSelected = mainMenuCanvasGameObject.GetComponentInChildren<Button>();
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
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup,
                    () => { displayed = true; }));
                break;
        }
    }

    public void FireBack()
    {
        //turn off event system
        MenuManager.Instance.EnableEventSystem(false);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, ShowMain));
    }

    void ShowMain()
    {
        mainMenuCanvasGameObject.SetActive(true);
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, mainMenuCanvasGroup, null));
        mainDefaultSelected.Select();
        //turn on event system
        MenuManager.Instance.EnableEventSystem(true);
    }
}
