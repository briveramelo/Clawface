using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMenu : Menu {

    #region Private Fields
    private bool displayed = false;
    #endregion  

    public FadeMenu(string name) : base(Strings.MenuStrings.FADE)
    {
    }

    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        switch (transition)
        {
            case Transition.HIDE:
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 3.0f, canvasGroup,
                    () => { displayed = false; canvasGroup.blocksRaycasts = false; }));
                break;
            case Transition.SHOW:
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup,
                    () => { displayed = true; canvasGroup.blocksRaycasts = true; }));
                break;
        }
    }

    // Use this for initialization
    void Start () {
        DoTransition(Transition.HIDE, new Effect[] { });	
	}
}
