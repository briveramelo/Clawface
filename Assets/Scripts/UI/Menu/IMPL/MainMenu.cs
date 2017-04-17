using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public MainMenu(string name) : base(name)
    {
    }

    public override void DoTransition(Transition transition, Effect[] effects)
    {
        throw new NotImplementedException();
    }

    public static void StartGame()
    {
        //call pertinent menu manager stuff
    }

    public static void FadeInMenu()
    {
    }
}
