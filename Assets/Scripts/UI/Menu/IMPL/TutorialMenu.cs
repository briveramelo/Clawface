using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : Menu {

    #region Serialized Unity Fields
    [SerializeField]
    private List<Sprite> sequence0;
    [SerializeField]
    private List<Sprite> sequence1;
    [SerializeField]
    private List<Sprite> sequence2;
    [SerializeField]
    private List<Sprite> sequence3;
    [SerializeField]
    private List<Sprite> sequence4;
    [SerializeField]
    private Image tutImage;
    #endregion

    #region Private Fields
    private bool displayed = false;
    private int currentSequence = 0;
    private int currentImage = 0;
    private bool transitioning = false;
    private bool advancing = false;
    private bool showing = false;
    #endregion

    public TutorialMenu(string name) : base(Strings.MenuStrings.TUTORIAL)
    {
    }

    #region Public Fields
    public override bool Displayed
    {
        get
        {
            return displayed;
        }
    }

    #endregion

    #region Public Interface

    public override void DoTransition(Transition transition, Effect[] effects)
    {

        switch (transition)
        {
            case Transition.SHOW:
                if (displayed) return;
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup, () => { displayed = true; transitioning = false; advancing = false; }));
                break;
            case Transition.HIDE:
                if (!displayed) return;
                StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, () => { displayed = false; transitioning = false; advancing = false; }));
                break;
            case Transition.TOGGLE:
                DoTransition(displayed ? Transition.HIDE : Transition.SHOW, effects);
                return;
                
        }
    }

    public void ShowImages(int seq)
    {
        showing = true;
        ResetTriggers();
        currentSequence = seq;
        
        transitioning = true;
        SetImage();
        DoTransition(Transition.SHOW, new Effect[] { });
    }


    #endregion


    #region Unity Lifecycle Methods

    private void Update()
    {
        if(Input.anyKey && !transitioning && !advancing && showing)
        {
            advancing = true;
            AdvanceImage();
        }
    }

    #endregion

    #region Private Interface
    
    void ResetTriggers()
    {
        currentImage = 0;
        transitioning = false;
        advancing = false;
        displayed = false;
    }

    bool SetImage()
    {
        Sprite spriteToSet = null;

        switch (currentSequence)
        {
            case 0:
                if (currentImage < sequence0.Count)
                    spriteToSet = sequence0[currentImage];
                break;
            case 1:
                if (currentImage < sequence1.Count)
                    spriteToSet = sequence1[currentImage];
                break;
            case 2:
                if (currentImage < sequence2.Count)
                    spriteToSet = sequence2[currentImage];
                break;
            case 3:
                if (currentImage < sequence3.Count)
                    spriteToSet = sequence3[currentImage];
                break;
            case 4:
                if (currentImage < sequence4.Count)
                    spriteToSet = sequence4[currentImage];
                break;
        }

        //oh shit we found something
        if (spriteToSet != null)
        {
            tutImage.sprite = spriteToSet;
            currentImage++;
            return true;
        }else
        {
            return false;
        }
    }

    void AdvanceImage()
    {
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 1.0f, canvasGroup, SetAndAdvance));
    }



    void SetAndAdvance()
    {
        Sprite spriteToSet = null;

        switch (currentSequence)
        {
            case 0:
                if(currentImage < sequence0.Count)
                    spriteToSet = sequence0[currentImage];
                break;
            case 1:
                if (currentImage < sequence1.Count)
                    spriteToSet = sequence1[currentImage];
                break;
            case 2:
                if (currentImage < sequence2.Count)
                    spriteToSet = sequence2[currentImage];
                break;
            case 3:
                if (currentImage < sequence3.Count)
                    spriteToSet = sequence3[currentImage];
                break;
            case 4:
                if (currentImage < sequence4.Count)
                    spriteToSet = sequence4[currentImage];
                break;
        }

        //oh shit we found something
        if (spriteToSet != null)
        {
            tutImage.sprite = spriteToSet;
            currentImage++;
            StartCoroutine(MenuTransitionsCommon.FadeCoroutine(0.0f, 1.0f, 1.0f, canvasGroup, () => { advancing = false; transitioning = false; }));
            return;
        }
        showing = false;
       
    }
    #endregion
}
