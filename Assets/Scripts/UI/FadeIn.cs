﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : Menu {

    public FadeIn(string name) : base(name)
    {
    }

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        throw new NotImplementedException();
    }

    private void Awake()
    {
        canvasGroup.alpha = 1f;
        StartCoroutine(MenuTransitionsCommon.FadeCoroutine(1.0f, 0.0f, 3.0f, canvasGroup, DisableFadeScreen));
    }

    void DisableFadeScreen()
    {
        gameObject.SetActive(false);
    }
}