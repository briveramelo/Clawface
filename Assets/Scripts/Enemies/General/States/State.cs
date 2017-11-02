//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : RoutineRunnerNonMono
{
    public string stateName;
    public abstract void OnEnter();
    public abstract void Update();
    public abstract void OnExit();

}
