using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;
public class RoutineRunner : MonoBehaviour {

    protected string coroutineName { get { return GetHashCode().ToString(); } }
    protected virtual void OnDisable() {
        Timing.KillCoroutines(coroutineName);
    }

    protected IEnumerator<float> DelayAction(Action action, float seconds = 0f) {
        yield return 0f;
        yield return Timing.WaitForSeconds(seconds);
        action();
    }
    protected IEnumerator<float> DelayAction(Action action, int frames)
    {
        for (int i = 0; i < frames; i++) {
            yield return 0f;
        }
        action();
    }
}

public class RoutineRunnerNonMono {

    public string coroutineName { get { return GetHashCode().ToString(); } }
    protected virtual void OnDisable() {
        Timing.KillCoroutines(coroutineName);
    }

    protected IEnumerator<float> DelayAction(Action action, float seconds = 0f) {
        yield return 0f;
        yield return Timing.WaitForSeconds(seconds);
        action();
    }
}