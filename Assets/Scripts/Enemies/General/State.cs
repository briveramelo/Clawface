//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {

    public abstract void OnEnter();
    public abstract void Update();
    public abstract void OnExit();

    public bool Equals<T>(T type) {
        return GetType().ToString() == type.GetType().ToString();
    }
}
